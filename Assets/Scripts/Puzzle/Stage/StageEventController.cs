using UnityEngine.EventSystems; // UGUI 이벤트 인터페이스를 위해 추가

namespace Puzzle.Stage
{
	public class StageEventController : EventTrigger
	{
		public bool IsDragging { get; private set; }

		public override void OnBeginDrag(PointerEventData eventData)
		{
			base.OnBeginDrag(eventData);
			// Drag 시작
			IsDragging = false;
		}

		public override void OnDrag(PointerEventData eventData)
		{
			base.OnDrag(eventData);

			if (IsDragging) return;

			if (eventData.delta.magnitude < Constants.DragThreshold) return;

			IsDragging = true;

			var direction = DirectionUtil.GetDirection(eventData.delta);

			if (direction == MoveDirection.None) return;

			MessageSystem.Instance.Publish(BlockMoveEvent.Create(direction));
		}
		
		public override void OnEndDrag(PointerEventData eventData)
		{
			base.OnEndDrag(eventData);

			IsDragging = false;
		}
	}
}
