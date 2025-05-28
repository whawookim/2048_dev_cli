using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace Puzzle
{
	public class Game : MonoBehaviour
	{
		public static Game Instance { get; private set; }

		[field: SerializeField]
		public StageMode CurrentStage { get; private set; } = StageMode.Stage3x3;

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

			if (sceneName == "Lobby")
			{
				yield return LobbyManager.LoadAsync();
			}
			else if (sceneName == "Stage")
			{
				yield return StageManager.LoadAsync();
			}
			
			UI.LoadingScreen.Instance.SetDisabled(true);
		}
	}
}
