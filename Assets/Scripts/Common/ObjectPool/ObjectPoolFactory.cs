using System;
using UnityEngine;

/// <summary>
/// ObjectPool 기반으로 MonoBehaviour 오브젝트를 관리하는 팩토리 클래스
/// - 생성 시 초기 수량만큼 풀을 구성
/// - Create로 오브젝트 요청, Release로 반납
/// </summary>
public class ObjectPoolFactory<T> : IPoolFactory<T> where T : MonoBehaviour
{
    private ObjectPool<T> _pool;
    private readonly GameObject _originPrefab;
    private readonly Transform _parent;
    private readonly Action<T> _onCreate;

    /// <summary>
    /// 팩토리 초기화
    /// </summary>
    /// <param name="originPrefab">풀링할 원본 프리팹</param>
    /// <param name="parent">오브젝트 부모 트랜스폼</param>
    /// <param name="initCount">초기 생성 수량</param>
    /// <param name="onCreate">생성 후 후처리 콜백</param>
    public ObjectPoolFactory(GameObject originPrefab, Transform parent, int initCount, Action<T> onCreate = null)
    {
        _originPrefab = originPrefab;
        _parent = parent;
        _onCreate = onCreate;

        _pool = new ObjectPool<T>();
        _pool.Init(_originPrefab.GetComponent<T>(), _parent, initCount);
    }

    /// <summary>
    /// 오브젝트 생성 요청
    /// </summary>
    public T Create()
    {
        var obj = _pool.GetOrCreate();
        if (obj == null)
        {
            MyDebug.LogError($"ObjectPoolFactory<{typeof(T).Name}>: Failed to get object.");
            return null;
        }

        _onCreate?.Invoke(obj);
        return obj;
    }

    /// <summary>
    /// 오브젝트 반납
    /// </summary>
    public void Release(T obj)
    {
        if (obj == null) return;
        obj.gameObject.SetActive(false);
        _pool.Find()?.Add(obj); // 안전하게 리스트 접근
    }

    /// <summary>
    /// 풀 클리어
    /// </summary>
    public void Clear()
    {
        _pool.Dispose();
    }

    /// <summary>
    /// 해당 오브젝트가 현재 풀에 포함되어 있는지 확인
    /// </summary>
    public bool Contains(T obj)
    {
        var list = _pool.Find();
        return list != null && list.Contains(obj);
    }

    /// <summary>
    /// 기존 풀을 제거하고 다시 초기화
    /// </summary>
    public void Reset(int count)
    {
        _pool.Dispose(); // 기존 오브젝트 제거
        _pool.Init(_originPrefab.GetComponent<T>(), _parent, count);
    }

    /// <summary>
    /// 리소스 해제
    /// </summary>
    public void Dispose()
    {
        _pool.Dispose();
        _pool = null;
    }
}
