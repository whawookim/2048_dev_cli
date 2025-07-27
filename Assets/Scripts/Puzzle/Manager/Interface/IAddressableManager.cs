using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Addressable을 로드하고 Release하는 매니저
/// </summary>
public interface IAddressableManager
{
    Task LoadAllAsync();
    void Release();
    void AddLoadedObject(GameObject obj);
}
