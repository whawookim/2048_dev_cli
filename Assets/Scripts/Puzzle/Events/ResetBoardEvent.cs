public class ResetBoardEvent : Events
{
    private static TinyObjectPool<ResetBoardEvent> pool = new TinyObjectPool<ResetBoardEvent>();

    public override string ToString()
    {
        return $"{nameof(ResetBoardEvent)}";
    }
    
    public override void Dispose()
    {
        pool.Return(this);
    }

    public static ResetBoardEvent Create()
    {
        var e = pool.GetOrCreate();
        return e;
    }
}
