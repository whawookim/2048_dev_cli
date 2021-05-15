using UnityEngine;
using UnityEngine.SceneManagement;

namespace Puzzle
{
	public class Game : MonoBehaviour
	{
		public static Game Instance { get; private set; }

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
			// TODO: 로비 씬 이동하기
			SceneManager.LoadScene(sceneName);
		}
	}
}
