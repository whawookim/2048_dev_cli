using UnityEngine;

namespace Puzzle.UI
{
	public class Board : MonoBehaviour, IPooledObject
	{
		[SerializeField]
		private UIWidget widget;

		public void Set(int size, string widgetName)
		{
			widget.width = size;
			widget.height = size;
			widget.cachedGameObject.name = widgetName;
		}
	}
}
