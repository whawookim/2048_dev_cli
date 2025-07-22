using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using Puzzle.Stage;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Puzzle
{
    /// <summary>
    /// 스테이지 매니저
    /// </summary>
    public class StageManager : IAddressableManager
    {
        private static StageManager _instance;
        public static StageManager Instance => _instance ??= new StageManager();

        private AsyncOperationHandle<GameObject> _stageHandle;

        private AsyncOperationHandle<GameObject> _boardHandle;

        private AsyncOperationHandle<GameObject> _blockHandle;
        
        public GameObject OriginBoardObj => _boardHandle.Result;
        
        public GameObject OriginBlockObj => _blockHandle.Result;
        
        public readonly StageStatusController StatusController = new ();
        
        public IEnumerator LoadAllAsync()
        {
            _stageHandle = Addressables.InstantiateAsync(nameof(UI.Stages));
            yield return _stageHandle;

            if (_stageHandle.Status == AsyncOperationStatus.Succeeded)
            {
                MyDebug.Log("Stage Loaded!");
                
                _boardHandle = Addressables.InstantiateAsync(nameof(UI.Board));
                yield return _boardHandle;

                if (_boardHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    MyDebug.Log("Board Loaded!");
                }
                else
                {
                    MyDebug.LogError("Board Load Failed!");
                }
                
                _blockHandle = Addressables.InstantiateAsync(nameof(UI.Block));
                yield return _blockHandle;

                if (_blockHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    MyDebug.Log("Block Loaded!");
                }
                else
                {
                    MyDebug.LogError("Block Load Failed!");
                }
            }
            else
            {
                MyDebug.LogError("Stage Load Failed!");
            }
            
            // 매니저 등록
            GameManager.Instance.RegisterManger(this);
        }

        public void Release()
        {
            if (_stageHandle.IsValid())
            {
                Addressables.ReleaseInstance(_stageHandle);
                MyDebug.Log("Stage Released!");
            }
            
            if (_boardHandle.IsValid())
            {
                Addressables.ReleaseInstance(_boardHandle);
                MyDebug.Log("Board Released!");
            }
            
            if (_blockHandle.IsValid())
            {
                Addressables.ReleaseInstance(_blockHandle);
                MyDebug.Log("Block Released!");
            }
        }

        public async Task<bool> ClearGameAsync(StageMode stageMode, int score = -1, int clearTime = -1, int moveCount = -1)
        {
            try
            {
                // 서버에 Ranking 요청
                var request = ApiConnection.EndStage(User.Me, stageMode, score, clearTime,
                    moveCount);
                while (!request.IsDone)
                    await Task.Yield();

                if (request.Ok)
                {
                    MyDebug.Log("Stage Clear!");
                    // TODO: 후처리?

                    return true;
                }
                else
                {
                    MyDebug.LogError($"Clear Game Request Failed: {request.Response?.error?.code}, message {request.Response?.error?.message}");
                }
            }
            catch (System.Exception ex)
            {
                MyDebug.LogError($"Clear Game Failed: {ex.Message}");
            }
            
            return false;
        }

        public void RestartGame()
        {
            CollectGC();
            StatusController.RestartGame();
        }

        public void GoToLobby()
        {
            CollectGC();
            GameManager.Instance.ChangeScene(UnityScene.Lobby, new UI.UITransition()
            {
                NextScene = UI.LobbyMain.Instance,
                NextSceneType = typeof(UI.LobbyMain),
                TransitionType = UI.UITransitionType.Push,
                SavedState = new UI.LobbyMainState()
                {
                    CurrentStageMode = StatusController.CurrentStageMode
                }
            });
        }

        public void CollectGC()
        {
            System.GC.Collect();
        }
    }
}
