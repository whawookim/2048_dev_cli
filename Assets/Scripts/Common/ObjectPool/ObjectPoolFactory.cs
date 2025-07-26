using System;
using UnityEngine;

public class ObjectPoolFactory<T> : IPoolFactory<T> where T : MonoBehaviour
{
    private ObjectPool<T> _pool;
    private readonly GameObject _originPrefab;
    private readonly Transform _parent;
    private readonly Action<T> _onCreate;

    public ObjectPoolFactory(GameObject originPrefab, Transform parent, int initCount, Action<T> onCreate = null)
    {
        _originPrefab = originPrefab;
        _parent = parent;
        _onCreate = onCreate;

        _pool = new ObjectPool<T>();
        _pool.Init(_originPrefab.GetComponent<T>(), _parent, initCount);
    }

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

    public void Release(T obj)
    {
        if (obj == null) return;
        obj.gameObject.SetActive(false);
        _pool.Find()?.Add(obj); // 안전하게 리스트 접근
    }

    public void Clear()
    {
        _pool.Dispose();
    }

    public bool Contains(T obj)
    {
        var list = _pool.Find();
        return list != null && list.Contains(obj);
    }

    public void Reset(int count)
    {
        _pool.Dispose(); // 기존 오브젝트 제거
        _pool.Init(_originPrefab.GetComponent<T>(), _parent, count);
    }

    public void Dispose()
    {
        _pool.Dispose();
        _pool = null;
    }
}
