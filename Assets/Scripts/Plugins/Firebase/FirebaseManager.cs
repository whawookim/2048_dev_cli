using System.Threading.Tasks;
using Firebase;
using Firebase.Analytics;
using Firebase.Crashlytics;

public static class FirebaseManager
{
    /// <summary>
    /// Analytics 초기화
    /// </summary>
    public static async Task InitializeAsync()
    {
        var task = FirebaseApp.CheckAndFixDependenciesAsync();

        await task;
        
        if (task.Result == DependencyStatus.Available)
        {
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
            MyDebug.Log("Firebase Analytics initialized.");

            Crashlytics.IsCrashlyticsCollectionEnabled = true;
            MyDebug.Log("Firebase Crashlytics initialized!");
        }
        else
        {
            MyDebug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
        }
    }
}
