using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Puzzle
{
	public class GameManager : MonoBehaviour
	{
		public static GameManager Instance { get; private set; }

		private readonly List<IAddressableManager> addressableManagers = new List<IAddressableManager>();

		public readonly InputManager InputManager = new ();
		
		#region MonoBehaviour
		
		private void Awake()
		{
			Instance = this;
			
			DontDestroyOnLoad(gameObject);
		}

		private void OnDestroy()
		{
			Instance = null;
		}

		/// <summary>
		/// 게임의 시작 부분
		/// </summary>
		private void Start()
		{
			_ = InitAsync();
		}

		#endregion

		private async Task InitAsync()
		{
			UI.LoadingScreen.Instance.SetEnabled(true);

			// inputManager UpdateFrame 추가
			UpdateFrameManager.Instance.AddUpdatable(InputManager);
			
			// Firebase RemoteConfig에서 설정한 LogLevel로 MyDebug CurrentLevel 제어
			// Unity 내부 Debug.Exception이나 Assert를 Crashlytics로 호출될 수 있게 초기화.
			// 무조건 한번만 불리게 하자.
			await MyDebug.InitializeAsync();
			
			// Firebase Analytics Init
			await FirebaseManager.InitializeAsync();

			// 광고 초기화
			AdManager.Instance.Init();
			
			// 시작 씬 이동
			ChangeScene(UnityScene.Lobby, new UI.Scene.UITransition()
			{
				NextScene = UI.Scene.TitleScreen.Instance,
				NextSceneType = typeof(UI.Scene.TitleScreen),
				TransitionType = UI.Scene.UITransitionType.Push,
			});
		}

		/// <summary>
		/// AddressableManager 등록
		/// </summary>
		/// <remarks>현재 씬 Addressable 관리자 등록</remarks>
		public void RegisterManger(IAddressableManager manager)
		{
			if (!addressableManagers.Contains(manager))
			{
				addressableManagers.Add(manager);
			}
		}

		/// <summary>
		/// Addressable 매니저들 해제
		/// </summary>
		public void ReleaseAll()
		{
			foreach (var manager in addressableManagers)
			{
				manager.Release();
			}

			addressableManagers.Clear();
		}

		/// <summary>
		/// 게임 종료 함수
		/// </summary>
		public void QuitGame()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}

		/// <summary>
		/// 씬 이동
		/// </summary>
		public void ChangeScene(UnityScene unitySceneEnum, UI.Scene.UITransition transition)
		{
			CoroutineManager.Instance.Run(ChangeSceneAsync(unitySceneEnum, transition));
		}

		/// <summary>
		/// 씬 이동 Async
		/// </summary>
		public IEnumerator ChangeSceneAsync(UnityScene unitySceneEnum, UI.Scene.UITransition transition)
		{
			UI.LoadingScreen.Instance.SetEnabled(true);

			// 1) 모든 Tween 중단
			DG.Tweening.DOTween.KillAll();

			// 2) 현재 씬 스택 날리면서 Pause, Finish 호출 날리기
			UI.Flow.UIFlowManager.Instance.ClearStackScenes();

			// 3) 씬 전환시 기존에 등록한 AddressableManager 전체 해제
			ReleaseAll();
			
			// 4) 새 씬 로드 (이전 씬 자동 언로드)
			yield return SceneManager.LoadSceneAsync(unitySceneEnum.ToString());
			
			// 5) 사용되지 않는 에셋 해제
			Resources.UnloadUnusedAssets();

			// 6) (선택) 가비지 컬렉션
			System.GC.Collect();
			
			// 7) Event용으로 쓰였던 ObjectPool 해제
			TinyObjectPool.ClearAll();
			
			// 8) Addressable 로드할 것들 로드 (씬은 SetTransition 과정에서 로드함. 그 외의 것들 할 것)
			var addressableManager = unitySceneEnum.GetAddressableManager();

			if (addressableManager != null)
			{
				yield return addressableManager.LoadAllAsync();
			}

			yield return UI.Flow.UIFlowManager.Instance.SetTransitionAsync(transition);

			// 씬 트랜지션 끝날때까지 대기
			yield return new WaitUntil(() => UI.Flow.UIFlowManager.Instance.CurrentTransition == null);
			
			UI.LoadingScreen.Instance.SetDisabled(true);
		}
	}
}
