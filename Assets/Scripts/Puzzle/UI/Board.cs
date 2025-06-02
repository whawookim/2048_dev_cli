using UnityEngine;

namespace Puzzle.UI
{
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
