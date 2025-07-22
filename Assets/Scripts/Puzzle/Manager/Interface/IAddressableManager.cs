using System.Collections;
using UnityEngine;

/// <summary>
/// Addressable을 로드하고 Release하는 매니저
/// </summary>
public interface IAddressableManager
{
    IEnumerator LoadAllAsync();
    void Release();
    void AddLoadedObject(GameObject obj);
}
