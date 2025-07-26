public class UpdateGameScoreEvent : Events
{
	public int Value { get; private set; }

	private static readonly TinyObjectPool<UpdateGameScoreEvent> Pool = new TinyObjectPool<UpdateGameScoreEvent>();

	public override string ToString()
	{
		return $"{nameof(UpdateGameScoreEvent)} {Value}";
	}

	public override void Dispose()
	{
		Pool.Return(this);
	}

	public static UpdateGameScoreEvent Create(int value)
	{
		var e = Pool.GetOrCreate();
		e.Value = value;

		return e;
	}
}
