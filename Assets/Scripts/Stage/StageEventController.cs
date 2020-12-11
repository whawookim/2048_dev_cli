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
				Debug.Log("Mouse Down : " + worldObjectPos);
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
				
				Debug.Log("moveVec.magnitude: " + moveVec.magnitude);

				MessageSystem.Instance.Publish(new BlockMoveEvent(DirectionUtil.GetDirection(moveVec)));
				
				startPos = null;
			}
#endif
			if (Input.GetKeyDown(KeyCode.A))
			{
				Debug.LogError("Test");
			}
			
			if (Input.touchCount == 0) return;

			var touch = Input.GetTouch(0);
			var phase = touch.phase;

			Debug.Log(phase);
		}
	}
}
