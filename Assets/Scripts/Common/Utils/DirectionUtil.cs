using UnityEngine;

public enum MoveDirection { None, Left, Right, Up, Down }

public static class DirectionUtil
{
	/// <summary>
	/// 이동 벡터를 보고 방향 가져오기
	/// </summary>
	public static MoveDirection GetDirection(Vector2 moveVec)
	{
		if (moveVec.x > 0 && moveVec.y > 0)
		{
			return moveVec.x > moveVec.y ? MoveDirection.Right : MoveDirection.Up;
		}

		if (moveVec.x > 0 && moveVec.y <= 0)
		{
			return moveVec.x > Mathf.Abs(moveVec.y) ? MoveDirection.Right : MoveDirection.Down;
		}

		if (moveVec.x <= 0 && moveVec.y > 0)
		{
			return Mathf.Abs(moveVec.x) > moveVec.y ? MoveDirection.Left : MoveDirection.Up;
		}

		if (moveVec.x <= 0 && moveVec.y <= 0)
		{
			return Mathf.Abs(moveVec.x) > Mathf.Abs(moveVec.y) ? MoveDirection.Left : MoveDirection.Down;
		}

		return MoveDirection.None;
	}
}
