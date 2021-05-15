using UnityEngine;

namespace Puzzle.Stage
{
	public enum StageState
	{
		Start,
		Pause,
		Clear,
		Fail
	}

	public class StageEventController : MonoBehaviour
	{
		private bool isDragging;

		void OnPress(bool pressed)
		{
			if (!pressed && isDragging)
			{
				isDragging = false;
			}
		}


		void OnDrag(Vector2 delta)
		{
			if (isDragging) return;

			if (delta.magnitude < Constants.DragThreshold) return;

			isDragging = true;

			MessageSystem.Instance.Publish(BlockMoveEvent.Create(DirectionUtil.GetDirection(delta)));
		}
	}
}
