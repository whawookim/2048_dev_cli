using UnityEngine;
using Firebase.Crashlytics;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;

public static class MyDebug
{
    public enum LogLevel
    {
        Log,
        Warning,
        Error,
        Exception
    }

    // 현재 로그 출력 레벨 (RemoteConfig 등으로 설정)
    public static LogLevel CurrentLevel = LogLevel.Log;

    public static async Task InitializeAsync()
    {
        await MyDebugConfigLoader.InitDebugRemoteConfigAsync();
        
#if !UNITY_EDITOR
        Application.logMessageReceived += HandleApplicationLog;
#endif
    }

    [Conditional("DEBUG")]
    public static void Log(string message, UnityEngine.Object context = null)
    {
        if (CurrentLevel > LogLevel.Log) return;
        Debug.Log(message, context);
    }

    [Conditional("DEBUG")]
    public static void LogWarning(string message, UnityEngine.Object context = null)
    {
        if (CurrentLevel > LogLevel.Warning) return;
        Debug.LogWarning(message, context);
    }

    public static void LogError(string message, UnityEngine.Object context = null)
    {
        if (CurrentLevel > LogLevel.Error) return;

        Debug.LogError(message, context);

#if !UNITY_EDITOR
        if (MyDebugConfigLoader.EnableCrashlyticsLog)
        {
            Crashlytics.Log($"[ERROR] {message}");
        }
#endif
    }

    public static void LogException(Exception ex, UnityEngine.Object context = null)
    {
        if (CurrentLevel > LogLevel.Exception) return;

        Debug.LogException(ex, context);

#if !UNITY_EDITOR
        if (MyDebugConfigLoader.EnableCrashlyticsLog)
        {
            Crashlytics.LogException(ex);
        }
#endif
    }
    
    private static void HandleApplicationLog(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Exception || type == LogType.Assert)
        {
            Log($"[Unhandled {type}] {condition}");
            LogException(new Exception(stackTrace));
        }
    }
}
