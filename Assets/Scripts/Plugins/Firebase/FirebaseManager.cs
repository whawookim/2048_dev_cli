using System.Threading.Tasks;
using Firebase;
using Firebase.Analytics;
using Firebase.Crashlytics;
using Firebase.RemoteConfig;
using Firebase.Extensions;

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
        
        Task fetchTask = null;
        
        await FirebaseRemoteConfig.DefaultInstance.FetchAsync(System.TimeSpan.Zero).ContinueWithOnMainThread(fTask =>
        {
            fetchTask = fTask;
        });
        
        if ((fetchTask.IsCompleted && !fetchTask.IsFaulted && !fetchTask.IsCanceled))
        {
            var activateTask = FirebaseRemoteConfig.DefaultInstance.ActivateAsync()
                .ContinueWithOnMainThread(_ => { });
        
            await activateTask;

            if (!activateTask.IsCompleted)
            {
                MyDebug.LogError("Failed to activate remote config");
            }
        }
        else
        {
            MyDebug.LogError("Failed to Fetch remote config async");
        }
    }
}
