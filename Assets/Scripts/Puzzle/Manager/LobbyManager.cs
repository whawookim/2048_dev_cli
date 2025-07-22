using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Puzzle
{
    /// <summary>
    /// 로비 매니저
    /// </summary>
    public class LobbyManager : IAddressableManager
    {
        private static LobbyManager _instance;
        public static LobbyManager Instance => _instance ??= new LobbyManager();
        
        private List<GameObject> loadedObjectList = new List<GameObject>();

        public IEnumerator LoadAllAsync()
        {
            // 매니저 등록 (등록 이미 된 경우 거름)
            GameManager.Instance.RegisterManger(this);

            yield break;
        }
        
        public void AddLoadedObject(GameObject obj)
        {
            loadedObjectList.Add(obj);
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
    }
}
