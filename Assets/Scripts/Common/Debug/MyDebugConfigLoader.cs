using Firebase.RemoteConfig;
using Firebase.Extensions;
using UnityEngine;
using System;
using System.Threading.Tasks;

public static class MyDebugConfigLoader
{
    private const string RemoteKey_LogLevel = "debug_log_level";

    private const string RemoteKey_EnableLog = "crashlytics_log_enabled";
    
    public static bool EnableCrashlyticsLog { get; private set; } = true;

    public static async Task InitDebugRemoteConfigAsync()
    {
        Task fetchTask = null;

        await FirebaseRemoteConfig.DefaultInstance.FetchAsync(System.TimeSpan.Zero)
            .ContinueWithOnMainThread(fTask =>
            {
                fetchTask = fTask;
            });

        if (!(fetchTask.IsCompleted && !fetchTask.IsFaulted && !fetchTask.IsCanceled))
        {
            MyDebug.LogError("Failed to Fetch remote config async");
            return;
        }

        var activateTask = FirebaseRemoteConfig.DefaultInstance.ActivateAsync()
            .ContinueWithOnMainThread(_ => { });
        
        await activateTask;

        if (!activateTask.IsCompleted)
        {
            MyDebug.LogError("Failed to activate remote config");
            return;
        }
        
        // RemoteConfig bool 값이 없으면 true를 기본값으로 사용
        EnableCrashlyticsLog = FirebaseRemoteConfig.DefaultInstance.GetValue(RemoteKey_EnableLog).BooleanValue;
                            
        string levelStr = FirebaseRemoteConfig.DefaultInstance
            .GetValue(RemoteKey_LogLevel).StringValue;

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
