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

		public void SetName(string objName)
		{
			gameObject.name = objName;
		}
		
		public void SetSize(int size)
		{
			rect.sizeDelta = new Vector2(size, size);
		}

        public Vector3 GetPosition()
        {
            return rect.anchoredPosition;
        }
        
        /// <summary>
        /// 보드 칸의 화면 위치를 설정한다.
        /// </summary>
        public void SetPosition(Vector3 worldPos)
        {
            rect.anchoredPosition = worldPos;
        }
	}
}
