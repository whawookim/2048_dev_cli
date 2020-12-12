using UnityEngine;

namespace Stage
{
	public class StageEventController : MonoBehaviour
	{
		private Vector2? startPos;

		void Update()
		{
#if UNITY_EDITOR
			if (Input.GetMouseButtonDown(0))
			{
				var mouseDragPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
				var worldObjectPos = Camera.main.ScreenToWorldPoint(mouseDragPos);
				startPos = worldObjectPos;
			}
			else if (Input.GetMouseButton(0))
			{
				if (startPos == null) return;
				
				var mouseDragPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
				var worldObjectPos = Camera.main.ScreenToWorldPoint(mouseDragPos);
				var dragPos = new Vector2(worldObjectPos.x, worldObjectPos.y);
				var moveVec = dragPos - startPos.Value;

				if (moveVec.magnitude < 1.5f) return;

				MessageSystem.Instance.Publish(new BlockMoveEvent(DirectionUtil.GetDirection(moveVec)));
				
				startPos = null;
			}
#else
			if (Input.touchCount == 0) return;
			
			var touch = Input.GetTouch(0);
			var phase = touch.phase;

			if (phase == TouchPhase.Began)
			{
				startPos = touch.position;
			}
			else if (phase == TouchPhase.Moved)
			{
				if (startPos == null) return;

				var moveVec = touch.deltaPosition;
				
				if (moveVec.magnitude < 1.5f) return;
				
				MessageSystem.Instance.Publish(new BlockMoveEvent(DirectionUtil.GetDirection(moveVec)));
				
				startPos = null;
			}
#endif
		}
	}
}
