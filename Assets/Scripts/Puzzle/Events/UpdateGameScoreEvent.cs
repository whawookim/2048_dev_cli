public class UpdateGameScoreEvent : Events
{
	public int Value { get; private set; }

	private static TinyObjectPool<UpdateGameScoreEvent> pool = new TinyObjectPool<UpdateGameScoreEvent>();

	public override string ToString()
	{
		return $"{nameof(UpdateGameScoreEvent)} {Value}";
	}

	public override void Dispose()
	{
		pool.Return(this);
	}

	public static UpdateGameScoreEvent Create(int value)
	{
		var e = pool.GetOrCreate();
		e.Value = value;

		return e;
	}
}
