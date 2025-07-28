using System;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build.Reporting;
using System.IO;
using UnityEditor.AddressableAssets;
using UnityEngine;

public static class BuildScript
{
    [MenuItem("Build/Build APK with Keystore")]
    public static void BuildAndroid()
    {
        try
        {
            Debug.unityLogger.logEnabled = true;
            Debug.Log("✅ Debug logger 강제 활성화");
            
            Console.WriteLine("BuildAndroid Start");
            Debug.Log("BuildAndroid Start");
            
            string androidPlayerPath = Path.Combine(EditorApplication.applicationContentsPath, "PlaybackEngines/AndroidPlayer");
            if (!Directory.Exists(androidPlayerPath))
            {
                Debug.LogError("[Error] Android Build Support is not installed.");
                Console.WriteLine("[Console] Android Build Support missing: " + androidPlayerPath);
                return;
            }
            
            // Addressables 빌드 먼저!
            BuildAddressables();
        
            // 빌드 경로
#if UNITY_EDITOR
            // 에디터에서는 직접 경로 지정 (프로젝트 루트 기준으로 fallback)
            var workspacePath = Application.dataPath + "/..";
#else
            // Jenkins나 CI 환경에서 환경변수 사용
            var workspacePath = Environment.GetEnvironmentVariable("WORKSPACE");
#endif
            var outputPath = Path.Combine(workspacePath, "BuildOutput/Android");
        
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
            
            Debug.Log($"BuildAndroid OutputPath : {outputPath}");
            Console.WriteLine($"BuildAndroid OutputPath : {outputPath}");
        
            var isEC2 = Environment.GetEnvironmentVariable("IS_EC2") == "true";
        
            // keystore 정보 설정
            PlayerSettings.Android.useCustomKeystore = true;

            Debug.Log($"BuildAndroid isEC2 : {isEC2}");
            Console.WriteLine($"BuildAndroid isEC2 : {isEC2}");
            
            if (isEC2)
            {
                PlayerSettings.Android.keystoreName = "/home/ubuntu/2048_dev_cli/KeyStores/2048-release.jks";
            }
            else
            {
                PlayerSettings.Android.keystoreName = "C:/Users/User/dev-cli/KeyStores/2048-release.jks";
            }

            PlayerSettings.Android.keystorePass = "Ksh6194@";
            PlayerSettings.Android.keyaliasName = "2048alias";
            PlayerSettings.Android.keyaliasPass = "Ksh6194@";
            
            var apkPath = Path.Combine(outputPath, "2048Dev.apk");
            Debug.Log($"APK Exists: {File.Exists(apkPath)} - Path: {apkPath}");
            Console.WriteLine($"APK Exists: {File.Exists(apkPath)} - Path: {apkPath}");

            // 빌드 옵션
            BuildPlayerOptions buildOptions = new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Scenes/Game.unity", "Assets/Scenes/Lobby.unity", "Assets/Scenes/Stage.unity" },
                locationPathName = apkPath,
                target = BuildTarget.Android,
                options = BuildOptions.None // 필요에 따라 BuildOptions.Development 추가 가능
            };
            
            foreach (var scenePath in buildOptions.scenes)
            {
                Debug.Log($"Include Scene: {scenePath} - Exists: {File.Exists(scenePath)}");
                Console.WriteLine($"Include Scene: {scenePath} - Exists: {File.Exists(scenePath)}");
            }
            
            Debug.Log("BuildPipeline.BuildPlayer Before");
            Console.WriteLine("BuildPipeline.BuildPlayer Before");

            // 빌드 실행
            BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
            
            // 로그 저장
            File.WriteAllText(Path.Combine(workspacePath, "build.log"), report.summary.ToString());

            Debug.Log("BuildPipeline.BuildPlayer End");
            Console.WriteLine("BuildPipeline.BuildPlayer End");
            
            if (report.summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Build Succeeded : " + report.summary.outputPath);
                Console.WriteLine("Build Succeeded : " + report.summary.outputPath);
                EditorApplication.Exit(0);
            }
            else
            {
                Debug.LogError("Android Build Failed!");
                
                foreach (var step in report.steps)
                {
                    foreach (var msg in step.messages)
                    {
                        Debug.Log($"[{msg.type}] {msg.content}");
                        Console.WriteLine($"[{msg.type}] {msg.content}");
                    }
                }

                EditorApplication.Exit(1); // 종료 코드 1: 실패로 간주되게
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Android Build Exception: " + ex);
            Console.WriteLine("Android Build Exception: " + ex);
        }
    }

    private static void BuildAddressables()
    {
        Console.WriteLine("Addressables Build Start...");
        Debug.Log("Addressables Build Start...");
        AddressableAssetSettings.CleanPlayerContent(AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilder);
        AddressableAssetSettings.BuildPlayerContent();
        Debug.Log("Addressables Build Complete!");
        Console.WriteLine("Addressables Build Complete!");
    }
}
