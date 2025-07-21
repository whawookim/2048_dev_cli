using System;
using System.Collections.Generic;

public static class ApiDeltaDispatcher
{
    /// <summary>
    /// API 통신 결과로 패치가 되는 것들 구현
    /// </summary>
    private static readonly Dictionary<string, IPatchable> _patchTargets = new()
    {
        { nameof(User), User.Me },
    };
    
    public static void ApplyAuto(Dictionary<string, object> responseData)
    {
        foreach (var kv in responseData)
        {
            string key = kv.Key;

            // delta patch 대응용
            if (key.EndsWith('.'))
            {
                key = key.Substring(0, key.Length - 1);
            }

            if (_patchTargets.TryGetValue(key, out var patchTarget))
            {
                // "User": { UserId: "...", Nickname: ... }
                if (kv.Value is Newtonsoft.Json.Linq.JObject obj)
                {
                    var dict = obj.ToObject<Dictionary<string, object>>();
                    patchTarget.ApplyPatch(dict);
                }
            }
        }
    }
}
