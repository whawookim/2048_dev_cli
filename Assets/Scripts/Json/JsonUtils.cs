using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
}
