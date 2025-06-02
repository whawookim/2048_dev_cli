using Puzzle.Stage;

public class ChangeGameStateEvent : Events
{
	public StageState State { get; private set; }

	private static TinyObjectPool<ChangeGameStateEvent> pool = new TinyObjectPool<ChangeGameStateEvent>();

	public override string ToString()
	{
		return $"{nameof(ChangeGameStateEvent)}";
	}

	public override void Dispose()
	{
		pool.Return(this);
	}

	public static ChangeGameStateEvent Create(StageState state)
	{
		var e = pool.GetOrCreate();
		e.State = state;

		return e;
	}
}
