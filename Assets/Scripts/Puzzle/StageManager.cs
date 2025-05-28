using UnityEngine;
using System.Collections;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Puzzle
{
    public class StageManager : MonoBehaviour
    {
        /// <summary>
        /// Stage 로드
        /// </summary>
        public static IEnumerator LoadAsync()
        {
            // Addressable 로드
            var handle = Addressables.LoadAssetAsync<GameObject>("Stages");
            yield return handle;

            if (handle.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError("Stages Prefab Load Failed");
                yield break;
            }
            
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Instantiate(handle.Result);

                if (Stages.Instance != null)
                {
                    yield return Stages.Instance.LoadAsync();

                    Stages.Instance.Init();
                    Stages.Instance.StartGame();
                }
            }
            
            Addressables.Release(handle);
        }
    }
}
