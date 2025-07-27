using UnityEngine;

/// <summary>
/// 풀링 오브젝트 생성을 위한 팩토리 인터페이스
/// - 생성(Create), 반납(Release), 초기화 등 기본 풀링 기능 제공
/// </summary>
public interface IPoolFactory<T> where T : MonoBehaviour
{
    /// 오브젝트 생성
    T Create();
    /// 오브젝트 풀에 반납
    void Release(T obj);
    ///  전체 풀 제거
    void Clear();
    /// 특정 오브젝트가 풀에 포함되어 있는지 확인
    bool Contains(T obj);
    /// 초기화 및 오브젝트 재생성
    void Reset(int count);
    /// 리소스 정리
    void Dispose();
}
