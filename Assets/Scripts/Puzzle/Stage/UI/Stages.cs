using System.Collections;
using UnityEngine;

namespace Puzzle.UI
{
	public class StagesState
	{
		public StageMode CurrentStageMode { get; set; }
	}
	
	/// <summary>
	/// 스테이지 메인 씬
	/// </summary>
	public class Stages : MonoBehaviour, IUIScene
	{
		public static Stages Instance { get; private set; }

		[SerializeField]
		private BoardUI boardUI;

		[SerializeField]
		private StageUI stageUI;

		private StagesState states;

		public string AddressableName => nameof(Stages);

#region MonoBehaviour
		private void Awake()
		{
			Debug.Assert(Instance == null);

			Instance = this;
		}

		private void OnDestroy()
		{
			// 데이터 날리기용
			Dispose();
					
			Debug.Assert(Instance == this);

			Instance = null;
		}
#endregion
		
#region IUIScene
        
		string IUIScene.Name => nameof(Stages);
		public UISceneManager UISceneManager { get; set; }
		IEnumerator IUIScene.Load(object savedState)
		{
			states = savedState as StagesState;

			StageManager.Instance.StatusController.SetStageMode(states.CurrentStageMode);
			
			InitBoard(StageManager.Instance.OriginBoardObj,
				StageManager.Instance.OriginBlockObj);
			yield break;
		}

		void IUIScene.Begin()
		{
			stageUI.SubscribeEvent();
			StageManager.Instance.StatusController.StartGame();
			boardUI.Init(states.CurrentStageMode);
		}

		void IUIScene.Resume(object result)
		{
			GameManager.Instance.InputManager.EscapeCallback += stageUI.OnClickLobby;
		}

		void IUIScene.Pause()
		{
			GameManager.Instance.InputManager.EscapeCallback -= stageUI.OnClickLobby;
		}

		void IUIScene.Finish()
		{
			stageUI.UnsubscribeEvent(true);
		}

		object IUIScene.GetState()
		{
			return null;
		}

#endregion

		public void Dispose()
		{
			boardUI.Dispose();
		}

		public void InitBoard(GameObject originBoard, GameObject originBlock)
		{
			boardUI.InitOriginResource(originBoard, originBlock);
		}
		
		/// <summary>
		/// x, y 인덱스(zero-based)로 찾은 board 위치
		/// </summary>
		public Vector3 GetBoardPosition(int index)
		{
			return boardUI.GetBoardPosition(index);
		}
	}
}
