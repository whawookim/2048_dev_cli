public class ResetBoardEvent : Events
{
    private static readonly TinyObjectPool<ResetBoardEvent> Pool = new TinyObjectPool<ResetBoardEvent>();

    public override string ToString()
    {
        return $"{nameof(ResetBoardEvent)}";
    }
    
    public override void Dispose()
    {
        Pool.Return(this);
    }

    public static ResetBoardEvent Create()
    {
        var e = Pool.GetOrCreate();
        return e;
    }
}
