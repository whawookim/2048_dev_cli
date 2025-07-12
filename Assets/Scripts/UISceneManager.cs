using System;
using System.Collections;
using System.Collections.Generic;
using Puzzle.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Puzzle
{
	public class UISceneManager
	{
		public static UISceneManager Instance = new UISceneManager();
       
		private Stack<IUIOverlay> overlayStack = new Stack<IUIOverlay>();
		
		/// <summary>
		/// 현재 가장 위에 보여지는 오버레이
		/// </summary>
		public IUIOverlay CurrentOverlay
		{
			get
			{
				if (overlayStack.Count == 0)
					return null;

				return overlayStack.Peek();
			}
		}
		
		/// <summary>
		/// 오버레이가 닫힐 때마다 발생되는 이벤트
		/// </summary>
		public event Action OverlayPopped;
		
		private Stack<KeyValuePair<IUIScene, object>> sceneStack = new Stack<KeyValuePair<IUIScene, object>>();

		private IUIScene currentScene;

		public IUIScene CurrentScene
		{
			get => currentScene;
			private set
			{
				currentScene = value;
			}
		}

		public UITransition CurrentTransition { get; private set; }
		
		private readonly object nullSavedState = new object();

		public void PushOverlay(IUIOverlay overlay, object state = null, Type overlayType = null)
		{
			if (overlayStack.Contains(overlay))
			{
				Debug.LogWarning("Overlay Already Exists");
				return;
			}
			
			CoroutineManager.Instance.Run(PushOverlayProcess(overlay, state, overlayType));
		}

		public IEnumerator PushOverlayProcess(IUIOverlay overlay, object state = null, Type overlayType = null)
		{
			UIBlocker.Instance.SetEnabled();

			// Addressable에서 로드
			if (overlay == null)
			{
				Debug.Assert(overlayType != null);
				
				yield return LoadUIAsset(overlayType);
				
				overlay = overlayType.GetProperty("Instance")?.GetValue(null) as IUIOverlay;

				// 이건 로드가 실패한 것이다.
				Debug.Assert(overlay != null);
			}
			
			overlayStack.Push(overlay);

			overlay.Begin(state);
			
			(overlay as MonoBehaviour)?.gameObject.SetActive(true);

			yield return overlay.OpenAnimation();
			
			UIBlocker.Instance.SetDisabled();
		}

		public void PopOverlay()
		{
			CoroutineManager.Instance.Run(PopOverlayProcess());
		}

		public IEnumerator PopOverlayProcess()
		{
			// 쌓인 오버레이가 없는 경우 팝 안됨
			if (CurrentOverlay == null || overlayStack.Count == 0)
			{
				Debug.LogWarning("No stacked overlay");
				yield break;
			}
			
			UIBlocker.Instance.SetEnabled();
			
			var popOverlay = CurrentOverlay;

			// 쌓인 오버레이에서 제거
			overlayStack.Pop();

			// 닫아주는 애니메이션 실행
			yield return popOverlay.CloseAnimation();

			popOverlay.Finish();
			
			(popOverlay as MonoBehaviour)?.gameObject.SetActive(false);
			
			UIBlocker.Instance.SetDisabled();
			
			// 오버레이 닫힘 이벤트
			OverlayPopped?.Invoke();
		}
		
		/// <summary>
		/// 오픈되어 있는 오버레이팝업들을 전부 강제로 닫음
		/// </summary>
		public void ClearAllOverlayList()
		{
			if (overlayStack.Count > 0)
			{
				while (overlayStack.Count > 0)
				{
					CurrentOverlay.Finish();
					(CurrentOverlay as MonoBehaviour)?.gameObject.SetActive(false);
					overlayStack.Pop();
				}
			}

			overlayStack.Clear();
		}

		public void SetTransition(UITransition transition)
		{
			if (CurrentTransition != null)
			{
				return;
			}
			
			Debug.Assert(transition != null);
			
			// Fade In/Out 애니메이션
			CoroutineManager.Instance.Run(SetFadeTransitionAsync(transition));
		}

		public IEnumerator SetFadeTransitionAsync(UITransition transition)
		{
			if (CurrentTransition != null)
			{
				yield break;
			}
			
			// 팝할 때에는 직접 씬이나 상태를 지정할 수 없음
			if (transition.TransitionType == UITransitionType.Pop)
			{
				Debug.Assert(transition.NextScene == null && transition.SavedState == null);

				// 팝인 경우 트랜지션에 다음에 올 씬 정보를 채워줌
				if (sceneStack.Count > 0)
				{
					var kv = sceneStack.Peek();
					transition.NextScene = kv.Key;
					transition.SavedState = kv.Value;
				}
			}

			// 트랜지션 시작
			CurrentTransition = transition;
			
			// TODO: FADE 한 연출로 바꾸기
			LoadingScreen.Instance.SetEnabled(true);
			
			// 현재 씬 일시 정지
			// Finish는 애니메이션이 모두 끝난 후에 호출할 것임
			currentScene?.Pause();
			
			if (transition.NextScene == null)
			{
				Debug.Assert(transition.NextSceneType != null);
				
				yield return LoadUIAsset(transition.NextSceneType);
			}
			
			var nextScene = transition.NextScene;
			var savedState = transition.SavedState;

			if (transition.TransitionType == UITransitionType.Push && currentScene != null)
			{
				sceneStack.Push(new KeyValuePair<IUIScene, object>(currentScene, nullSavedState));
			}
			else if (transition.TransitionType == UITransitionType.Pop && sceneStack.Count > 0)
			{
				// 트랜지션 타입이 팝이라면 마지막에 삽입했던 씬으로 갈 것임
				var kv = sceneStack.Pop();
				nextScene = kv.Key;
				savedState = kv.Value;
			}

			if (nextScene != null)
			{
				// TODO: 씬이 stack에 이미 들어 있는 경우 처리 필요
				// savedState == nullSavedState 이라면 nextScene이 Pause()만 된 상태이므로 로드 및 비긴 필요 없음
				// 트랜지션 타입이 팝인 경우에만 nullSavedState 일 수 있음
				if (savedState != nullSavedState)
				{
					yield return nextScene.Load(savedState);
				}
			}
			
			(nextScene as MonoBehaviour)?.gameObject.SetActive(true);
			
			// SetActive(true) 이후에 비긴 및 리줌을 부름
			if (nextScene != null)
			{
				// savedState == nullSavedState 이면 바로 이전 씬 팝 된 것이라 비긴할 필요가 없음
				// stackedTransition 으로 들어와서 Load는 되었으나 Begin 이 안 된 씬이라면 Begin 이 되어야한다.
				if (savedState != nullSavedState)
				{
					nextScene.Begin();
				}

				nextScene.Resume(transition.Result);
			}
			
			// 트랜지션 타입이 푸시가 아닌 경우 원래 보고 있던 씬을 종료
			if (transition.TransitionType != UITransitionType.Push)
			{
				currentScene?.Finish();
			}

			if (currentScene != null)
			{
				(currentScene as MonoBehaviour)?.gameObject.SetActive(false);
			}

			CurrentScene = nextScene;
			
			transition.RaiseTransitionEnded();
			
			yield return null;
			
			// TODO: FadeIn 연출로 바꾸기
			LoadingScreen.Instance.SetDisabled(true);

			CurrentTransition = null;
		}
		
		private IEnumerator LoadUIAsset(Type assetType)
		{
			var addressableName = assetType.GetProperty("AddressableName")?.GetValue(null);

			if (addressableName == null)
			{
				Debug.LogError($"{assetType} does not have the 'AddressableName' property.");

				yield break;
			}
			
			var handle = Addressables.InstantiateAsync(addressableName);
			yield return handle;

			if (handle.Status != AsyncOperationStatus.Succeeded)
			{
				Debug.Log("UI Load Failed!");
			}

			var prefab = handle.Result;

			if (prefab == null)
			{
				Debug.LogError($"{assetType} does not have the 'Prefab' property.");
				yield break;
			}
			
			var assetInstance = assetType.GetProperty("Instance")?.GetValue(null);

			if (assetInstance is IUIOverlay uiOverlay)
			{
				uiOverlay.UISceneManager = this;
			}
			else if (assetInstance is IUIScene uiScene)
			{
				uiScene.UISceneManager = this;
			}
			else
			{
				Debug.LogError("Cannot recognize a type of the UI asset: " + assetInstance);
			}
		}

		public void ClearStackScenes()
		{
			while (sceneStack.Count > 0)
			{
				var kv = sceneStack.Pop();

				if (kv.Key != null)
				{
					kv.Key.Pause();
					kv.Key.Finish();
				}
			}

			CurrentScene = null;
		}
	}
}
