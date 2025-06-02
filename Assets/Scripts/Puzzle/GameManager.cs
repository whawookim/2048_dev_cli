using System.Collections;
using System.Collections.Generic;
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
			ChangeScene("Lobby");
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
		public void ChangeScene(string sceneName)
		{
			StartCoroutine(ChangeSceneAsync(sceneName));
		}

		/// <summary>
		/// 씬 이동 Async
		/// </summary>
		public IEnumerator ChangeSceneAsync(string sceneName)
		{
			UI.LoadingScreen.Instance.SetEnabled(true);

			// 1) 모든 Tween 중단
			DG.Tweening.DOTween.KillAll();

			// 2) 새 씬 로드 (이전 씬 자동 언로드)
			yield return SceneManager.LoadSceneAsync(sceneName);
			
			// 3) 사용되지 않는 에셋 해제
			Resources.UnloadUnusedAssets();

			// 4) (선택) 가비지 컬렉션
			System.GC.Collect();

			// 5) 씬 전환시 기존에 등록한 AddressableManager 전체 해제
			ReleaseAll();

			if (sceneName == "Lobby")
			{
				yield return LobbyManager.Instance.LoadAsync();
			}
			else if (sceneName == "Stage")
			{
				yield return StageManager.Instance.LoadAsync();
				
				if (Stages.Instance != null)
				{ 
					Stages.Instance.InitBoard(StageManager.Instance.OriginBoardObj,
						StageManager.Instance.OriginBlockObj);
					Stages.Instance.StartGame();
				}
			}
			
			UI.LoadingScreen.Instance.SetDisabled(true);
		}
	}
}
