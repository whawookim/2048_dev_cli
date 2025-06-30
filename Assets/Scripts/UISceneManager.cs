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

		public void SetTransition()
		{

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
			else
			{
				Debug.LogError("Cannot recognize a type of the UI asset: " + assetInstance);
			}
		}
	}
}
