using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Network;

public static class ApiClient
{
    private const string BaseUrl = "https://whawoo.xyz"; // 실제 서버 주소로 변경 필요

    public static async Task<ApiResponse> SendAsync(string endpoint, Dictionary<string, object> body = null)
    {
        using var request = new UnityWebRequest($"{BaseUrl}{endpoint}", UnityWebRequest.kHttpVerbPOST);
        var json = JsonConvert.SerializeObject(body ?? new Dictionary<string, object>());
        Debug.Log($"[API 요청] {endpoint}: {json}");
        var bodyRaw = Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        await request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            throw new Exception($"API 요청 실패: {request.error}");

        Debug.Log($"[API 응답] {request.downloadHandler.text}");
        
        var response = JsonConvert.DeserializeObject<ApiResponse>(request.downloadHandler.text);

        // 응답 내부 자동 반영
        ApiDeltaDispatcher.ApplyAuto(response.data); // 자동 적용 처리 추가

        return response;
    }
}