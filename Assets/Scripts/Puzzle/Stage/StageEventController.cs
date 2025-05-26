using UnityEngine;
using UnityEngine.EventSystems; // UGUI 이벤트 인터페이스를 위해 추가

namespace Puzzle.Stage
{
	public enum StageState
	{
		Start,
		Pause,
		Clear,
		Fail
	}

	public class StageEventController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		private bool isDragging;

		public void OnBeginDrag(PointerEventData eventData)
		{
			// Drag 시작
			isDragging = false;
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (isDragging) return;

			if (eventData.delta.magnitude < Constants.DragThreshold) return;

			isDragging = true;

			var direction = DirectionUtil.GetDirection(eventData.delta);

			if (direction == MoveDirection.None) return;

			MessageSystem.Instance.Publish(BlockMoveEvent.Create(direction));

		}
		
		public void OnEndDrag(PointerEventData eventData)
		{
			isDragging = false;
		}
	}
}
