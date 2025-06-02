using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TinyObjectPool 원형
/// </summary>
public abstract class TinyObjectPool
{
	/// <summary>
	/// 생성된 TinyObjectPool 전체 리스트
	/// </summary>
	/// <remarks>타이니 오브젝트 풀은 보통 static으로 사용될 것이므로 List.Remove는 쓰지 않을 것임</remarks>
	protected static List<TinyObjectPool> tinyObjectPools = new List<TinyObjectPool>();

	public abstract int PoolCount { get; }
	
	public static void ClearAll()
	{
#if UNITY_EDITOR
		Debug.Log($"{nameof(TinyObjectPool)}.{nameof(ClearAll)}"
		          + $" tinyObjectPools.Count: {tinyObjectPools.Count}");
#endif
		for (int i = 0; i < tinyObjectPools.Count; i++)
		{
			tinyObjectPools[i].Clear();
		}
	}
	
	/// <summary>
	/// TinyObjectPool 대기 목록 청소
	/// </summary>
	public abstract void Clear();
}

public class TinyObjectPool<T> : TinyObjectPool where T : class, new()
{
	/// <summary>
	/// 사용 가능한 오브젝트 인스턴스 목록
	/// </summary>
	private Stack<T> pool = new Stack<T>();
	
	public override int PoolCount => pool.Count;

	public TinyObjectPool()
	{
		// 생성될 때 전체 리스트에 추가
		tinyObjectPools.Add(this);
	}

	/// <summary>
	/// 풀에서 오브젝트 인스턴스 얻기(없으면 하나 생성)
	/// </summary>
	public T GetOrCreate()
	{
		if (pool.Count > 0)
		{
			return pool.Pop();
		}
		else
		{
			return new T();
		}
	}

	public void Return(T obj)
	{
#if UNITY_EDITOR
		if (pool.Contains(obj))
		{
			// 풀에 같은 오브젝트가 두 번 들어가면 안 됨.
			UnityEngine.Debug.LogError(
				$"Pushing an object {obj} already exists in TinyObjectPool<{typeof(T)}>. It might cause a serious problem!");

			return;
		}
#endif
		
		pool.Push(obj);
	}

	public override void Clear()
	{
		pool.Clear();
		pool.TrimExcess();
	}
}
