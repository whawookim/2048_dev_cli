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
            Debug.Log("BuildAndroid Start");
            
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
        
            var isEC2 = Environment.GetEnvironmentVariable("IS_EC2") == "true";
        
            // keystore 정보 설정
            PlayerSettings.Android.useCustomKeystore = true;

            Debug.Log($"BuildAndroid isEC2 : {isEC2}");
            
            if (isEC2)
            {
                PlayerSettings.Android.keystoreName = "/home/ubuntu/2048_dev_cli/KeyStores/2048-release.jk";
            }
            else
            {
                PlayerSettings.Android.keystoreName = "C:/Users/User/dev-cli/KeyStores/2048-release.jks";
            }

            PlayerSettings.Android.keystorePass = "Ksh6194@";
            PlayerSettings.Android.keyaliasName = "2048alias";
            PlayerSettings.Android.keyaliasPass = "Ksh6194@";
            
            // 빌드 옵션
            BuildPlayerOptions buildOptions = new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Scenes/Game.unity", "Assets/Scenes/Lobby.unity", "Assets/Scenes/Stage.unity" },
                locationPathName = $"{outputPath}/2048Dev.apk",
                target = BuildTarget.Android,
                options = BuildOptions.None // 필요에 따라 BuildOptions.Development 추가 가능
            };
            
            Debug.Log("BuildPipeline.BuildPlayer Before");

            // 빌드 실행
            BuildReport report = BuildPipeline.BuildPlayer(buildOptions);

            Debug.Log("BuildPipeline.BuildPlayer End");
            
            if (report.summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Build Succeeded : " + report.summary.outputPath);
            }
            else
            {
                Debug.LogError("Android Build Failed!");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Android Build Exception: " + ex);
        }
    }

    private static void BuildAddressables()
    {
        Debug.Log("Addressables Build Start...");
        AddressableAssetSettings.CleanPlayerContent(AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilder);
        AddressableAssetSettings.BuildPlayerContent();
        Debug.Log("Addressables Build Complete!");
    }
}
