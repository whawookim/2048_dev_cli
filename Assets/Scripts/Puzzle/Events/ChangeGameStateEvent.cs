using Puzzle.Stage;

public class ChangeGameStateEvent : Events
{
	public StageState State { get; private set; }

	private static readonly TinyObjectPool<ChangeGameStateEvent> Pool = new TinyObjectPool<ChangeGameStateEvent>();

	public override string ToString()
	{
		return $"{nameof(ChangeGameStateEvent)}";
	}

	public override void Dispose()
	{
		Pool.Return(this);
	}

	public static ChangeGameStateEvent Create(StageState state)
	{
		var e = Pool.GetOrCreate();
		e.State = state;

		return e;
	}
}
