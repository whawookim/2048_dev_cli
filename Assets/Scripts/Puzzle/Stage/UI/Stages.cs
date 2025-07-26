using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Puzzle.UI.Scene
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
		public Flow.UISceneManager UISceneManager { get; set; }
		Task IUIScene.LoadAsync(object savedState)
		{
			states = savedState as StagesState;

			StageManager.Instance.StatusController.SetStageMode(states.CurrentStageMode);

            boardUI.InitOriginResource(StageManager.Instance.OriginBoardObj, StageManager.Instance.OriginBlockObj);
            
			return Task.CompletedTask;
		}

		void IUIScene.Begin()
		{
			stageUI.SubscribeEvent();
            boardUI.SubscribeEvent();
			StageManager.Instance.StatusController.StartGame();
			boardUI.Init(states.CurrentStageMode);
		}

		void IUIScene.Resume(object result)
		{
		}

		void IUIScene.Pause()
		{
		}

		void IUIScene.Finish()
		{
			stageUI.UnsubscribeEvent(true);
            boardUI.UnsubscribeEvent(true);
		}

		object IUIScene.GetState()
		{
			return null;
		}

        public void OnClickBackButton()
        {
            stageUI.OnClickLobby();
        }

        #endregion

		public void Dispose()
		{
			boardUI.Dispose();
		}
		
		/// <summary>
		/// x, y 인덱스(zero-based)로 찾은 board 위치
		/// </summary>
		public Vector3 GetBoardPosition(Vector2Int pos)
		{
			return boardUI.GetBoardPosition(pos);
		}
	}
}
