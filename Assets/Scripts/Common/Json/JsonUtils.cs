using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JSONObject = System.Collections.Generic.Dictionary<string, object>;
using System;
using System.Collections;

/// <summary>
/// JSON 관련 유틸리티 함수 모음
/// - JObject → Dictionary 변환
/// - JsonObject에서 다양한 타입의 값 안전 추출
/// </summary>
public static class JsonUtils
{
    /// <summary>
    /// JSON 문자열을 Dictionary[string, object]로 파싱
    /// </summary>
    public static Dictionary<string, object> ToDictionary(string json)
    {
        var jObject = JsonConvert.DeserializeObject<JObject>(json);
        return ToDictionary(jObject);
    }

    /// <summary>
    /// JObject를 Dictionary[string, object]로 변환
    /// </summary>
    public static Dictionary<string, object> ToDictionary(JObject jObject)
    {
        var dict = new Dictionary<string, object>();
        foreach (var property in jObject.Properties())
        {
            dict[property.Name] = ConvertJToken(property.Value);
        }
        return dict;
    }

    /// <summary>
    /// JToken을 알맞은 C# 타입으로 변환
    /// </summary>
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
