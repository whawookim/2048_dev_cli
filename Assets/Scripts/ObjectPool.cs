using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 말 그대로 게임 오브젝트를 풀링
/// </summary>
public class ObjectPool<T> where T : MonoBehaviour, new()
{
	private static readonly List<List<T>> poolList = new List<List<T>>();

	private T origin;

	private Transform parent;

	/// <summary>
	/// 원본 오브젝트, 생성될 부모 트랜스폼, 초기 수량 세팅
	/// </summary>
	public bool Init(T original, Transform parentTransform, int initCount)
	{
		origin = original;
		parent = parentTransform;

		if (Find() != null) return false;

		var type = origin.GetComponent<T>();

		if (type == null || parent == null || initCount < 0) return false;

		poolList.Add(InstantiateObj(initCount));

		original.gameObject.SetActive(false);

		return true;
	}

	private List<T> InstantiateObj(int count)
	{
		var objList = new List<T>();

		if (origin == null || parent == null) return objList;

		for (var i = 0; i < count; i++)
		{
			var copied = GameObject.Instantiate(origin, Vector3.zero, Quaternion.identity, parent);
			copied.gameObject.SetActive(false);
			objList.Add(copied);
		}

		return objList;
	}

	private List<T> Find()
	{
		return poolList.Find((list) => list is { Count: > 0 } && list[0].GetType() == typeof(T));
	}

	public void Dispose()
	{
		Find()?.Clear();
	}

	public T GetOrCreate()
	{
		var objList = Find();

		if (objList == null)
		{
			var list = InstantiateObj(1);

			if (list.Count <= 0) return null;

			poolList.Add(list);
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
	/// 현재 비활성화 되어있는 오브젝트 가져오기
	/// </summary>
	private T Get(List<T> list)
	{
		foreach (var obj in list)
		{
			if (!obj.gameObject.activeSelf)
			{
				return obj;
			}
		}

		return null;
	}
}
