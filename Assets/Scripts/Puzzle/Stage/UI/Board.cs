using UnityEngine;

namespace Puzzle.UI
{
	/// <summary>
	/// 배경이 되는 보드 칸
	/// </summary>
	public class Board : MonoBehaviour, IPooledObject
	{
		[SerializeField]
		private RectTransform rect;

		public Vector3 GetPosition()
		{
			return rect.transform.position;
		}

		public void Set(string objName)
		{
			gameObject.name = objName;
		}
		
		public void SetSize(int size)
		{
			rect.sizeDelta = new Vector2(size, size);
		}
	}
}
