using UnityEngine.EventSystems; // UGUI 이벤트 인터페이스를 위해 추가

namespace Puzzle.Stage
{
    /// <summary>
    /// 퍼즐 스테이지에서 유저의 드래그 입력을 감지하여
    /// 드래그 방향 이벤트(BlockMoveEvent)를 발행하는 컨트롤러.
    /// </summary>
	public class StageEventController : EventTrigger
	{
        /// <summary>
        /// 현재 드래그 상태 여부
        /// </summary>
		public bool IsDragging { get; private set; }

        /// <summary>
        /// 드래그 시작 시 호출
        /// </summary>
		public override void OnBeginDrag(PointerEventData eventData)
		{
			base.OnBeginDrag(eventData);
			// Drag 시작
			IsDragging = false;
		}

        /// <summary>
        /// 드래그 중일 때 호출됨. 일정 임계값 이상 이동 시 방향 판별 후 이벤트 발행.
        /// </summary>
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
		
        /// <summary>
        /// 드래그 종료 시 호출
        /// </summary>
		public override void OnEndDrag(PointerEventData eventData)
		{
			base.OnEndDrag(eventData);

			IsDragging = false;
		}
	}
}
