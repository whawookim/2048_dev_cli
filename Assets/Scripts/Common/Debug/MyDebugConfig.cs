using Firebase.RemoteConfig;
using UnityEngine;
using System;

public static class MyDebugConfig
{
    private const string RemoteKeyLogLevel = "debug_log_level";

    private const string RemoteKeyEnableLog = "crashlytics_log_enabled";
    
    public static bool EnableCrashlyticsLog { get; private set; } = true;

    public static void InitDebugRemoteConfig()
    {
        // RemoteConfig bool 값이 없으면 true를 기본값으로 사용
        EnableCrashlyticsLog = FirebaseRemoteConfig.DefaultInstance.GetValue(RemoteKeyEnableLog).BooleanValue;
                            
        string levelStr = FirebaseRemoteConfig.DefaultInstance
            .GetValue(RemoteKeyLogLevel).StringValue;

        if (Enum.TryParse(levelStr, out MyDebug.LogLevel remoteLevel))
        {
            MyDebug.CurrentLevel = remoteLevel;
            Debug.Log($"[RemoteConfig] Set MyDebug.LogLevel to {remoteLevel}");
        }
        else
        {
            Debug.LogWarning($"[RemoteConfig] Invalid log level '{levelStr}', fallback to default");
        }
    }
}
