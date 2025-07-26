using UnityEngine;

public interface IPoolFactory<T> where T : MonoBehaviour
{
    T Create();
    void Release(T obj);
    void Clear();
    bool Contains(T obj);
    void Reset(int count);
    void Dispose();
}
