using System.Collections;
using System.Collections.Generic;
using Puzzle.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Puzzle
{
	public class GameManager : MonoBehaviour
	{
		public static GameManager Instance { get; private set; }

		[field: SerializeField]
		public StageMode CurrentStage { get; private set; } = StageMode.Stage3x3;

		private readonly List<IAddressableManager> addressableManagers = new List<IAddressableManager>();

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
		void Start()
		{
			// 광고 초기화
			AdManager.Instance.Init();

			// Firebase Analytics Init
			FirebaseManager.Instance.Init();
			
			// 시작 씬 이동
			ChangeScene("Lobby", nameof(TitleScreen));
		}
		
		#endregion

		/// <summary>
		/// AddressableManager 등록
		/// </summary>
		/// <remarks>현재 씬 Addressable 관리자 등록</remarks>
		public void RegisterManger(IAddressableManager manager)
		{
			if (!addressableManagers.Contains(manager))
				addressableManagers.Add(manager);
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
		/// 현재 선택한 스테이지 변경
		/// </summary>
		public void ChangeStage(StageMode mode)
		{
			CurrentStage = mode;
		}

		/// <summary>
		/// 씬 이동
		/// </summary>
		public void ChangeScene(string sceneName, string uiSceneName = null)
		{
			CoroutineManager.Instance.Run(ChangeSceneAsync(sceneName, uiSceneName));
		}

		/// <summary>
		/// 씬 이동 Async
		/// </summary>
		public IEnumerator ChangeSceneAsync(string unitySceneName, string uiSceneName)
		{
			UI.LoadingScreen.Instance.SetEnabled(true);

			// 1) 모든 Tween 중단
			DG.Tweening.DOTween.KillAll();

			// 2) 현재 씬 스택 날리면서 Pause, Finish 호출 날리기
			UISceneManager.Instance.ClearStackScenes();

			// 3) 씬 전환시 기존에 등록한 AddressableManager 전체 해제
			ReleaseAll();
			
			// 4) 새 씬 로드 (이전 씬 자동 언로드)
			yield return SceneManager.LoadSceneAsync(unitySceneName);
			
			// 5) 사용되지 않는 에셋 해제
			Resources.UnloadUnusedAssets();

			// 6) (선택) 가비지 컬렉션
			System.GC.Collect();
			
			// 7) Event용으로 쓰였던 ObjectPool 해제
			TinyObjectPool.ClearAll();

			if (unitySceneName == "Lobby")
			{
				yield return LobbyManager.Instance.LoadAllAsync();

				if (uiSceneName == nameof(TitleScreen))
				{
					UISceneManager.Instance.SetTransition(new UITransition()
					{
						NextScene = TitleScreen.Instance,
						NextSceneType = typeof(TitleScreen),
						TransitionType = UITransitionType.Push,
					});
				}
				else if (uiSceneName == nameof(LobbyMain))
				{
					UISceneManager.Instance.SetTransition(new UITransition()
					{
						NextScene = LobbyMain.Instance,
						NextSceneType = typeof(LobbyMain),
						TransitionType = UITransitionType.Push,
					});
				}
			}
			else if (unitySceneName == "Stage")
			{
				yield return StageManager.Instance.LoadAllAsync();

				if (uiSceneName == nameof(Stages))
				{
					UISceneManager.Instance.SetTransition(new UITransition()
					{
						NextScene = Stages.Instance,
						NextSceneType = typeof(Stages),
						TransitionType = UITransitionType.Push,
					});
				}
			}

			// 씬 트랜지션 끝날때까지 대기
			yield return new WaitUntil(() => UISceneManager.Instance.CurrentTransition == null);
			
			UI.LoadingScreen.Instance.SetDisabled(true);
		}
	}
}
