using Firebase;
using Firebase.Analytics;
using Firebase.Crashlytics;
using UnityEngine;

public class FirebaseManager
{
    private static FirebaseManager instnace;
    
    public static FirebaseManager Instance => instnace ??= new FirebaseManager();
    
    /// <summary>
    /// Analytics 초기화
    /// </summary>
    public void Init()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                MyDebug.Log("Firebase Analytics initialized.");
                
                // Crashlytics 초기화 및 테스트 로그
                Crashlytics.Log("Firebase Crashlytics initialized!");

                // 강제 오류 테스트 (비치명적)
                Crashlytics.Log("Triggering test non-fatal error");
                Crashlytics.LogException(new System.Exception("This is a test non-fatal exception."));
            }
            else
            {
                MyDebug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
            }
        });
    }
}
