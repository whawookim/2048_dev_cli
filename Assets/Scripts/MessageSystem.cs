using System.Collections.Generic;

public class MessageSystem
{
	public static readonly MessageSystem Instance = new MessageSystem();

	public delegate bool PublishEvent(Events e);

	private readonly Dictionary<string, PublishEvent> publishDict = new Dictionary<string, PublishEvent>();

	public bool Publish(Events e)
	{
		var name = e.GetType().ToString();

		return publishDict.ContainsKey(name) && publishDict[name].Invoke(e);
	}

	public void Subscribe<T>(PublishEvent e) where T : Events
	{
		var name = typeof(T).ToString();

		if (!publishDict.ContainsKey(name))
		{
			publishDict.Add(name, e);
		}
		else
		{
			publishDict[name] += e;
		}
	}

	public void Unsubscribe<T>(PublishEvent e, bool deleteKey = false) where T : Events
	{
		var name = typeof(T).ToString();

		if (publishDict.ContainsKey(name))
		{
			publishDict[name] -= e;
			
			// 더이상 key 자체를 안 쓸 거 같다 싶으면 날려버린다
			if (deleteKey)
			{
				publishDict.Remove(name);
			}
		}
	}
}
