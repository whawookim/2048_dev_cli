using System.Collections;
using Puzzle.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

namespace Puzzle
{
    /// <summary>
    /// 로비 매니저
    /// </summary>
    public class LobbyManager : IAddressableManager
    {
        private static LobbyManager _instance;
        public static LobbyManager Instance => _instance ??= new LobbyManager();
        
        private List<GameObject> _lobbyObjectList = new List<GameObject>();

        public IEnumerator LoadAllAsync()
        {
            // 매니저 등록
            GameManager.Instance.RegisterManger(this);

            var loadList = new List<AsyncOperationHandle<GameObject>>()
            {
                Addressables.InstantiateAsync(nameof(TitleScreen)),
                Addressables.InstantiateAsync(nameof(LobbyMain)),
            };

            foreach (var handle in loadList)
            {
                yield return handle;
                
                if (handle.Result != null)
                {
                    _lobbyObjectList.Add(handle.Result);
                    handle.Result.SetActive(false);
                }
            }
        }
        
        /// <summary>
        /// Lobby 로드
        /// </summary>
        public IEnumerator LoadAsync(string key)
        {
            // Addressable 로드
            var handle = Addressables.InstantiateAsync(key);
            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
                MyDebug.Log($"{key} Loaded!");
            else
                MyDebug.LogError($"{key} Load Failed!");
        }
        
        public void Release()
        {
            foreach (var handleObj in _lobbyObjectList)
            {
                if (handleObj != null)
                {
                    MyDebug.Log($"{handleObj.name} Released");
                    Addressables.ReleaseInstance(handleObj);
                }
            }
            
            _lobbyObjectList.Clear();
        }
    }
}
