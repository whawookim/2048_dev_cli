public class BlockMoveEvent : Events
{
	public MoveDirection Direction { get; private set; }

	private static TinyObjectPool<BlockMoveEvent> pool = new TinyObjectPool<BlockMoveEvent>();

	public override string ToString()
	{
		return $"{nameof(BlockMoveEvent)}";
	}

	public override void Dispose()
	{
		pool.Return(this);
	}

	public static BlockMoveEvent Create(MoveDirection direction)
	{
		var e = pool.GetOrCreate();
		e.Direction = direction;

		return e;
	}
}
