using System;

public class GameTime
{
    public static GameTime Instance = new GameTime();

    public DateTime CurrentTime => DateTime.UtcNow;
}