using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

public class AddressableCDNRedirector : MonoBehaviour
{
    [SerializeField]
    private string cdnDomain = "https://d38kwlpirpvyt.cloudfront.net";

    private void Awake()
    {
        string platform = GetPlatformFolder();
        string baseUrl = $"{cdnDomain}/{platform}";

        Debug.Log($"[Addressables] CDN Override → {baseUrl}");

        Addressables.ResourceManager.InternalIdTransformFunc = (location) =>
        {
            string origin = location.InternalId;
            Debug.Log($"[Addressables] Original URL: {origin}");

            if (origin.StartsWith("http") || origin.StartsWith("https"))
            {
                int idx = origin.IndexOf("/Android");
                if (idx < 0) idx = origin.IndexOf("/iOS");
                if (idx < 0) idx = origin.IndexOf("/StandaloneWindows");
                if (idx < 0) idx = origin.IndexOf("/WebGL");
                if (idx < 0)
                {
                    Debug.LogWarning("[Addressables] URL에서 플랫폼 경로를 찾지 못함. 원본 URL 사용.");
                    return origin;
                }

                string path = origin.Substring(idx);
                string newUrl = $"{baseUrl}{path}";

                Debug.Log($"[Addressables] Override: {origin} → {newUrl}");
                return newUrl;
            }

            return origin;
        };
    }

    private string GetPlatformFolder()
    {
#if UNITY_ANDROID
        return "Android";
#elif UNITY_IOS
        return "iOS";
#elif UNITY_WEBGL
        return "WebGL";
#elif UNITY_STANDALONE_WIN
        return "StandaloneWindows";
#elif UNITY_STANDALONE_OSX
        return "StandaloneOSX";
#else
        return "UnknownPlatform";
#endif
    }
}