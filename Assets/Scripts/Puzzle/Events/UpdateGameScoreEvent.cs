public enum UpdateGameScoreType
{
	Set,
	Add
}

public class UpdateGameScoreEvent : Events
{
	public UpdateGameScoreType Type { get; private set; }

	public int Value { get; private set; }

	private static TinyObjectPool<UpdateGameScoreEvent> pool = new TinyObjectPool<UpdateGameScoreEvent>();

	public override string ToString()
	{
		return $"{nameof(UpdateGameScoreEvent)}";
	}

	public override void Dispose()
	{
		pool.Dispose();
	}

	public static UpdateGameScoreEvent Create(UpdateGameScoreType type, int value)
	{
		var e = pool.GetOrCreate();
		e.Type = type;
		e.Value = value;

		return e;
	}
}
