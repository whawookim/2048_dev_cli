using System;
using System.Collections.Generic;

public static class JsonHelper
{
    public static T Get<T>(this Dictionary<string, object> dict, string key, T defaultValue = default)
    {
        if (dict.TryGetValue(key, out var value) && value is T typed)
            return typed;
        if (dict.TryGetValue(key, out var raw))
            return (T)Convert.ChangeType(raw, typeof(T));
        return defaultValue;
    }
}
