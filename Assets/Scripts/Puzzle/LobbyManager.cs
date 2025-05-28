using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Puzzle
{
    public class LobbyManager : MonoBehaviour
    {
        /// <summary>
        /// Lobby 로드
        /// </summary>
        public static IEnumerator LoadAsync()
        {
            // Addressable 로드
            var handle = Addressables.LoadAssetAsync<GameObject>("Lobby Main");
            yield return handle;
            
            if (handle.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError("LobbyMain Prefab Load Failed");
                yield break;
            }

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Instantiate(handle.Result);
            }
            
            Addressables.Release(handle);
        }
    }
}
