using UnityEngine;
using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using Firebase.Crashlytics;

/// <summary>
/// 디버깅 유틸리티 클래스
/// - 로그 레벨 설정 가능 (RemoteConfig 연동)
/// - Crashlytics 연동 지원
/// - Release 빌드에서는 조건부 로그 무시 가능
/// </summary>
public static class MyDebug
{
    /// <summary>
    /// 로그 출력 레벨 (Log / Warning / Error / Exception)
    /// </summary>
    public enum LogLevel
    {
        Log,
        Warning,
        Error,
        Exception
    }
    
    /// <summary>
    /// 현재 설정된 로그 레벨 (기본: Log)
    /// RemoteConfig 등을 통해 변경 가능
    /// </summary>
    public static LogLevel CurrentLevel = LogLevel.Log;

    /// <summary>
    /// 초기화 (Crashlytics 로그 연동 포함)
    /// </summary>
    public static void Initialize()
    {
        MyDebugConfig.InitDebugRemoteConfig();
        
#if !UNITY_EDITOR
        Application.logMessageReceived += HandleApplicationLog;
#endif
    }

    /// <summary>
    /// 일반 로그 출력 (DEBUG 빌드에서만 활성)
    /// </summary>
    [Conditional("DEBUG")]
    public static void Log(string message, UnityEngine.Object context = null)
    {
        if (CurrentLevel > LogLevel.Log) return;
        Debug.Log(message, context);
    }

    /// <summary>
    /// 경고 로그 출력
    /// </summary>
    [Conditional("DEBUG")]
    public static void LogWarning(string message, UnityEngine.Object context = null)
    {
        if (CurrentLevel > LogLevel.Warning) return;
        Debug.LogWarning(message, context);
    }

    /// <summary>
    /// 에러 로그 출력 및 Crashlytics 연동
    /// </summary>
    public static void LogError(string message, UnityEngine.Object context = null)
    {
        if (CurrentLevel > LogLevel.Error) return;

        Debug.LogError(message, context);

#if !UNITY_EDITOR
        if (MyDebugConfig.EnableCrashlyticsLog)
        {
            Crashlytics.Log($"[ERROR] {message}");
        }
#endif
    }

    /// <summary>
    /// 예외 출력 및 Crashlytics 예외 기록
    /// </summary>
    public static void LogException(Exception ex, UnityEngine.Object context = null)
    {
        if (CurrentLevel > LogLevel.Exception) return;

        Debug.LogException(ex, context);

#if !UNITY_EDITOR
        if (MyDebugConfig.EnableCrashlyticsLog)
        {
            Crashlytics.LogException(ex);
        }
#endif
    }

    /// <summary>
    /// 앱 전역 예외 처리용 (비 Unity 에러 포함)
    /// </summary>
    private static void HandleApplicationLog(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Exception || type == LogType.Assert)
        {
            Log($"[Unhandled {type}] {condition}");
            LogException(new Exception(stackTrace));
        }
    }
}
