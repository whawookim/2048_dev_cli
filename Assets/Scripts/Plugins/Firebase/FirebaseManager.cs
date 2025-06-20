using Firebase;
using Firebase.Analytics;
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
                Debug.Log("Firebase Analytics initialized.");
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
            }
        });
    }
}
