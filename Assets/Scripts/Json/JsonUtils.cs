using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JSONObject = System.Collections.Generic.Dictionary<string, object>;
using System;
using System.Collections;

public static class JsonUtils
{
    public static Dictionary<string, object> ToDictionary(string json)
    {
        var jObject = JsonConvert.DeserializeObject<JObject>(json);
        return ToDictionary(jObject);
    }

    public static Dictionary<string, object> ToDictionary(JObject jObject)
    {
        var dict = new Dictionary<string, object>();
        foreach (var property in jObject.Properties())
        {
            dict[property.Name] = ConvertJToken(property.Value);
        }
        return dict;
    }

    private static object ConvertJToken(JToken token)
    {
        return token.Type switch
        {
            JTokenType.Object => ToDictionary((JObject)token),
            JTokenType.Array => token.ToObject<List<object>>(),
            _ => ((JValue)token).Value
        };
    }
    
    public static int Populate(this JSONObject jsonObject, string name, int existingValue)
	{
		if (jsonObject.TryGetValue(name, out var value))
		{
			return Convert.ToInt32(value);
		}

		return existingValue;
	}

	public static double Populate(this JSONObject jsonObject, string name, double existingValue)
	{
		if (jsonObject.TryGetValue(name, out var value))
		{
			return Convert.ToDouble(value);
		}

		return existingValue;
	}

	public static int? Populate(this JSONObject jsonObject, string name, int? existingValue)
	{
		if (jsonObject.TryGetValue(name, out var value))
		{
			return value != null ? Convert.ToInt32(value) : (int?) null;
		}

		return existingValue;
	}

	public static long? Populate(this JSONObject jsonObject, string name, long? existingValue)
	{
		if (jsonObject.TryGetValue(name, out var value))
		{
			return value != null ? Convert.ToInt64(value) : (long?) null;
		}

		return existingValue;
	}

	public static double? Populate(this JSONObject jsonObject, string name, double? existingValue)
	{
		if (jsonObject.TryGetValue(name, out var value))
		{
			return value != null ? Convert.ToDouble(value) : (double?) null;
		}

		return existingValue;
	}

	public static bool Populate(this JSONObject jsonObject, string name, bool existingValue)
	{
		if (jsonObject.TryGetValue(name, out var value))
		{
			return Convert.ToBoolean(value);
		}

		return existingValue;
	}

	public static string Populate(this JSONObject jsonObject, string name, string existingValue)
	{
		if (jsonObject.TryGetValue(name, out var value))
		{
			return value as string;
		}

		return existingValue;
	}

	public static DateTimeOffset Populate(this JSONObject jsonObject, string name, DateTimeOffset existingValue)
	{
		if (jsonObject.TryGetValue(name, out var value))
		{
			return value != null ? DateTimeOffset.Parse(value as string) : DateTimeOffset.UtcNow;
		}

		return existingValue;
	}

	public static DateTimeOffset? Populate(this JSONObject jsonObject, string name, DateTimeOffset? existingValue)
	{
		if (jsonObject.TryGetValue(name, out var value))
		{
			return value != null ? DateTimeOffset.Parse(value as string) : (DateTimeOffset?) null;
		}

		return existingValue;
	}

	public static BitArray Populate(this JSONObject jsonObject, string name, BitArray existingValue)
	{
		if (jsonObject.TryGetValue(name, out var value))
		{
			return value is string str ? new BitArray(Convert.FromBase64String(str)) : null;
		}

		return existingValue;
	}
}
