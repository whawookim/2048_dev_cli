public enum MoveDirection { None, Left, Right, Up, Down }

public class BlockMoveEvent : Events
{
	public MoveDirection Direction { get; private set; }

	public BlockMoveEvent(MoveDirection direction)
	{
		Direction = direction;
	}
}
