using System.Collections.Generic;

public class TinyObjectPool<T> where T : new()
{
	private static readonly List<Stack<T>> PoolList = new List<Stack<T>>();

	public static void Clear()
	{
		PoolList.Clear();
	}

	private Stack<T> Find()
	{
		return PoolList.Find((value) => value.Peek().GetType() == typeof(T));
	}

	public void Dispose()
	{
		Find()?.Clear();
	}

	public T GetOrCreate()
	{
		var stack = Find();

		return (stack == null) ? Create() : Get(stack);
	}

	private T Create()
	{
		var item = new T();
		var stack = new Stack<T>();
		stack.Push(item);
		PoolList.Add(stack);
		return stack.Peek();
	}

	private T Get(Stack<T> stack)
	{
		return stack.Peek();
	}
}
