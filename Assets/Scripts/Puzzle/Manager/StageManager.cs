using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
        
        private List<GameObject> loadedObjectList = new List<GameObject>();
        
        public GameObject OriginBoardObj { get; private set; }
        
        public GameObject OriginBlockObj { get; private set; }
        
        public readonly StageStatusController StatusController = new ();

        public StageBoardController BoardController { get; private set; }

        public StageBoardModel BoardModel { get; private set; }
        
        public async Task LoadAllAsync()
        {
            // 매니저 등록
            GameManager.Instance.RegisterManger(this);
            
            var boardHandle = Addressables.InstantiateAsync(nameof(UI.Board));
            await boardHandle.Task;

            if (boardHandle.Status == AsyncOperationStatus.Succeeded)
            {
                MyDebug.Log("Board Loaded!");
                loadedObjectList.Add(boardHandle.Result);
                OriginBoardObj = boardHandle.Result;
            }
            else
            {
                MyDebug.LogError("Board Load Failed!");
            }
                
            var blockHandle = Addressables.InstantiateAsync(nameof(UI.Block));
            await blockHandle.Task;

            if (blockHandle.Status == AsyncOperationStatus.Succeeded)
            {
                MyDebug.Log("Block Loaded!");
                loadedObjectList.Add(blockHandle.Result);
                OriginBlockObj = blockHandle.Result;
            }
            else
            {
                MyDebug.LogError("Block Load Failed!");
            }
        }

        public void Release()
        {
            foreach (var handleObj in loadedObjectList)
            {
                if (handleObj != null)
                {
                    MyDebug.Log($"{handleObj.name} Released");
                    Addressables.ReleaseInstance(handleObj);
                }
            }
            
            loadedObjectList.Clear();
        }

        public void AddLoadedObject(GameObject obj)
        {
            loadedObjectList.Add(obj);
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
            GameManager.Instance.ChangeScene(UnityScene.Lobby, new UI.Scene.UITransition()
            {
                NextScene = UI.Scene.LobbyMain.Instance,
                NextSceneType = typeof(UI.Scene.LobbyMain),
                TransitionType = UI.Scene.UITransitionType.Push,
                SavedState = new UI.Scene.LobbyMainState()
                {
                    CurrentStageMode = StatusController.CurrentStageMode
                }
            });
        }

        public void CollectGC()
        {
            System.GC.Collect();
        }

        public void Dispose()
        {
            BoardController?.Dispose();
            BoardController = null;
            
            UndoHistory.Clear();
        }
        
        /// <summary>
        /// 현재 StageMode에 맞춰 보드와 블록을 세팅하고 게임판을 초기화
        /// </summary>
        public void LoadStage(StageMode mode, Transform blockParentTransform, Transform boardParentTransform)
        {
            var board = Instance.OriginBoardObj;
            var block = Instance.OriginBlockObj;
            
            var maxSize = mode.GetBoardSize();
            var maxNum = mode.GetBlockMaxNum();

            BoardModel = new StageBoardModel(maxSize, maxNum);

            var blockFactory = new ObjectPoolFactory<UI.Block>(
                block,
                blockParentTransform,
                maxSize * maxSize,
                b => {
                    b.gameObject.SetActive(true);
                    b.transform.localScale = Vector3.one;
                });

            var boardFactory = new ObjectPoolFactory<UI.Board>(
                board,
                boardParentTransform,
                maxSize * maxSize,
                b => {
                    b.gameObject.SetActive(true);
                    b.transform.localScale = Vector3.one;
                });

            BoardController = new StageBoardController(mode, BoardModel, blockFactory, boardFactory);
            BoardController.Initialize();
        }
        

        public void UndoLastCommand()
        {
            var snapshot = UndoHistory.Pop();
            if (snapshot != null)
            {
                RestoreSnapshot(snapshot);
            }
        }

        public StageSnapshot CreateSnapshot()
        {
            return new StageSnapshot
            {
                Blocks = BoardController.GetBlockSnapshot(),
                Score = StatusController.CurrentScore
            };
        }
        
        private void RestoreSnapshot(StageSnapshot snapshot)
        {
            BoardController.RestoreBlockSnapshot(snapshot.Blocks);
            StatusController.SetScore(snapshot.Score);
        }
    }
}
