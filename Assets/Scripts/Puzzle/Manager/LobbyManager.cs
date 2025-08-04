using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
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
        
        private readonly List<GameObject> _loadedObjectList = new List<GameObject>();

        public Task LoadAllAsync()
        {
            // 매니저 등록 (등록 이미 된 경우 거름)
            GameManager.Instance.RegisterManger(this);
            return Task.CompletedTask;
        }
        
        public void AddLoadedObject(GameObject obj)
        {
            _loadedObjectList.Add(obj);
        }
        
        public void Release()
        {
            foreach (var handleObj in _loadedObjectList)
            {
                if (handleObj != null)
                {
                    MyDebug.Log($"{handleObj.name} Released");
                    Addressables.ReleaseInstance(handleObj);
                }
            }
            
            _loadedObjectList.Clear();
        }
    }
}
