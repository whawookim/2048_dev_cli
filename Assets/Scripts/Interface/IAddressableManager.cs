using System.Collections;

/// <summary>
/// Addressable을 로드하고 Release하는 매니저
/// </summary>
public interface IAddressableManager
{
    IEnumerator LoadAsync();
    void Release();
}