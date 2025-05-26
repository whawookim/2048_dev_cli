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

		public void Set(int size, string objName)
		{
			rect.sizeDelta = new Vector2(size, size);
			gameObject.name = objName;
		}
	}
}
