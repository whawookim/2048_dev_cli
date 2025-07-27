public class BlockMoveEvent : Events
{
	public MoveDirection Direction { get; private set; }

	private static readonly TinyObjectPool<BlockMoveEvent> Pool = new TinyObjectPool<BlockMoveEvent>();

	public override string ToString()
	{
		return $"{nameof(BlockMoveEvent)}";
	}

	public override void Dispose()
	{
		Pool.Return(this);
	}

	public static BlockMoveEvent Create(MoveDirection direction)
	{
		var e = Pool.GetOrCreate();
		e.Direction = direction;

		return e;
	}
}
