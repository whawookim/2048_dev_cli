using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Api 통신용 매니저 클래스
/// </summary>
public class ApiManager
{
    private static ApiManager instance;
    
    public static ApiManager Instance => instance ??= new ApiManager();
    
    /// <summary>
    /// 서버에 HTTP REST API 통신
    /// </summary>
    public IEnumerator SendPostRequest(string url, string jsonData)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
        if (request.result == UnityWebRequest.Result.Success)
#else
        if (!request.isNetworkError && !request.isHttpError)
#endif
        {
            Debug.Log("Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }
}
