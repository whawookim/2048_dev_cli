using UnityEngine;
using System.Collections;
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
        
        public IEnumerator LoadAsync()
        {
            _stageHandle = Addressables.InstantiateAsync(nameof(Stages));
            yield return _stageHandle;

            if (_stageHandle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log("Stage Loaded!");
                
                _boardHandle = Addressables.InstantiateAsync(nameof(UI.Board));
                yield return _boardHandle;

                if (_boardHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log("Board Loaded!");
                }
                else
                {
                    Debug.LogError("Board Load Failed!");
                }
                
                _blockHandle = Addressables.InstantiateAsync(nameof(UI.Block));
                yield return _blockHandle;

                if (_blockHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log("Block Loaded!");
                }
                else
                {
                    Debug.LogError("Block Load Failed!");
                }
            }
            else
            {
                Debug.LogError("Stage Load Failed!");
            }
            
            // 매니저 등록
            GameManager.Instance.RegisterManger(this);
        }

        public void Release()
        {
            if (_stageHandle.IsValid())
            {
                Addressables.ReleaseInstance(_stageHandle);
                Debug.Log("Stage Released!");
            }
            
            if (_boardHandle.IsValid())
            {
                Addressables.ReleaseInstance(_boardHandle);
                Debug.Log("Board Released!");
            }
            
            if (_blockHandle.IsValid())
            {
                Addressables.ReleaseInstance(_blockHandle);
                Debug.Log("Block Released!");
            }
        }
    }
}
