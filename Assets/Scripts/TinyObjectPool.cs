using System.Collections.Generic;

public class TinyObjectPool<T> where T : new()
{
	private static List<Stack<T>> poolList = new List<Stack<T>>();

	public static void Clear()
	{
		poolList.Clear();
	}

	private Stack<T> Find()
	{
		return poolList.Find((value) => value.Peek().GetType() == typeof(T));
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
		poolList.Add(stack);
		return stack.Peek();
	}

	private T Get(Stack<T> stack)
	{
		return stack.Peek();
	}
}
