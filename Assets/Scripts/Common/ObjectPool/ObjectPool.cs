using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 특정 타입의 MonoBehaviour 오브젝트를 풀링하는 클래스
/// - 오브젝트 재사용을 통해 GC 및 성능 최적화
/// </summary>
public class ObjectPool<T> where T : MonoBehaviour
{
	private static readonly List<List<T>> PoolList = new List<List<T>>();

	private T _origin;

	private Transform _parent;

    /// <summary>
    /// 오브젝트 풀 초기화
    /// </summary>
    /// <param name="original">복제할 원본 오브젝트</param>
    /// <param name="parentTransform">부모 트랜스폼</param>
    /// <param name="initCount">초기 복제 개수</param>
    /// <returns>초기화 성공 여부</returns>
	public bool Init(T original, Transform parentTransform, int initCount)
	{
		_origin = original;
		_parent = parentTransform;

		if (Find() != null) return false;

		var type = _origin.GetComponent<T>();

		if (type == null || _parent == null || initCount < 0) return false;

		PoolList.Add(InstantiateObj(initCount));

		original.gameObject.SetActive(false);

		return true;
	}

    /// <summary>
    /// 오브젝트들을 count 만큼 생성하여 리스트로 반환
    /// </summary>
	private List<T> InstantiateObj(int count)
	{
		var objList = new List<T>();

		if (_origin == null || _parent == null) return objList;

		for (var i = 0; i < count; i++)
		{
			var copied = GameObject.Instantiate(_origin, Vector3.zero, Quaternion.identity, _parent);
			copied.gameObject.SetActive(false);
			objList.Add(copied);
		}

		return objList;
	}

    /// <summary>
    /// 현재 타입의 풀 리스트를 찾음
    /// </summary>
	public List<T> Find()
	{
		return PoolList.Find((list) => list is { Count: > 0 } && list[0].GetType() == typeof(T));
	}

    /// <summary>
    /// 풀 전체 삭제
    /// </summary>
	public void Dispose()
	{
		Find()?.Clear();
	}
    
    /// <summary>
    /// 비활성화 상태의 오브젝트를 가져오거나 없으면 새로 생성
    /// </summary>
	public T GetOrCreate()
	{
		var objList = Find();

		if (objList == null)
		{
			var list = InstantiateObj(1);

			if (list.Count <= 0) return null;

			PoolList.Add(list);
			return list[0];
		}

		if (objList.Count == 0)
		{
			var list = InstantiateObj(1);

			if (list.Count <= 0) return null;

			objList.AddRange(list);
			return objList[0];
		}

		return Get(objList);
	}

    /// <summary>
    /// 비활성화된 오브젝트 하나를 반환
    /// </summary>
	private T Get(List<T> list)
	{
		foreach (var obj in list)
		{
			if (!obj.gameObject.activeSelf)
				return obj;
		}

		return null;
	}
}
