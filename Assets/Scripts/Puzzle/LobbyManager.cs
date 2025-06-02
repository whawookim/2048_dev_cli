using System.Collections;
using Puzzle.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Puzzle
{
    /// <summary>
    /// 로비 매니저
    /// </summary>
    public class LobbyManager : IAddressableManager
    {
        private static LobbyManager _instance;
        public static LobbyManager Instance => _instance ??= new LobbyManager();
        
        private AsyncOperationHandle<GameObject> _lobbyHandle;
        
        /// <summary>
        /// Lobby 로드
        /// </summary>
        public IEnumerator LoadAsync()
        {
            // Addressable 로드
            _lobbyHandle = Addressables.InstantiateAsync(nameof(LobbyMain));
            yield return _lobbyHandle;

            if (_lobbyHandle.Status == AsyncOperationStatus.Succeeded)
                Debug.Log("LobbyMain Loaded!");
            else
                Debug.LogError("LobbyMain Load Failed!");
            
            // 매니저 등록
            GameManager.Instance.RegisterManger(this);
        }
        
        public void Release()
        {
            if (_lobbyHandle.IsValid())
            {
                Addressables.ReleaseInstance(_lobbyHandle);
                Debug.Log("LobbyMain Released!");
            }
        }
    }
}
