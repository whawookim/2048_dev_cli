using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Network;
using System.Linq;

public static class ApiClient
{
    /// <summary>
    /// TODO: 클라 하드코딩이 맞을지 고민
    /// </summary>
    private const string BaseUrl = "https://whawoo.xyz";

    public static async Task<ApiResponse> SendAsync(string endpoint, Dictionary<string, object> body = null,
        string method = UnityWebRequest.kHttpVerbPOST)
    {
        string url = $"{BaseUrl}{endpoint}";
    
        if (method == UnityWebRequest.kHttpVerbGET && body != null && body.Count > 0)
        {
            var query = string.Join("&", body.Select(kvp =>
                $"{UnityWebRequest.EscapeURL(kvp.Key)}={UnityWebRequest.EscapeURL(kvp.Value.ToString())}"));
            url += "?" + query;
        }

        using var request = new UnityWebRequest(url, method);

        if (method != UnityWebRequest.kHttpVerbGET)
        {
            var json = JsonConvert.SerializeObject(body ?? new Dictionary<string, object>());
            var bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.SetRequestHeader("Content-Type", "application/json");
        }

        request.downloadHandler = new DownloadHandlerBuffer();

        await request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            throw new Exception($"API 요청 실패: {request.error}");

        var response = JsonConvert.DeserializeObject<ApiResponse>(request.downloadHandler.text);

        ApiDeltaDispatcher.ApplyAuto(response.data);

        return response;
    }
}