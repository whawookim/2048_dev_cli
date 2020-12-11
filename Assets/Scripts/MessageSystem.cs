public class MessageSystem
{
	public static MessageSystem Instance = new MessageSystem();

	public delegate bool PublishEvent(Events e);
	
	public static event PublishEvent Publisher;

	public bool Publish(Events e)
	{
		if (Publisher == null) return false;
		
		return Publisher.Invoke(e);
	}

	public void Subscribe(PublishEvent e)
	{
		Publisher += e;
	}

	public void Unsubscribe(PublishEvent e)
	{
		Publisher -= e;
	}
}
