using UnityEditor;
using UnityEngine;
using System.Diagnostics;
using System.IO;
using Debug = UnityEngine.Debug;

public class AddressableUploader
{
    private const string PythonScriptName = "upload_addressable_to_s3.py";

    [MenuItem("Tools/Addressables/Build & Upload to AWS")]
    public static void BuildAndUploadAddressables()
    {
        // Addressable 빌드
        Debug.Log("[AddressableUploader] Addressable Build 시작...");
        try
        {
            UnityEditor.AddressableAssets.Settings.AddressableAssetSettings.BuildPlayerContent();
            Debug.Log("[AddressableUploader] Addressable Build 완료 ✅");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[AddressableUploader] Addressable Build 실패 ❌\n{ex.Message}");
            return;
        }

        // Python 스크립트 경로 찾기
        string projectPath = Directory.GetParent(Application.dataPath).FullName;
        string scriptPath = Path.Combine(projectPath, PythonScriptName);

        if (!File.Exists(scriptPath))
        {
            Debug.LogError($"[AddressableUploader] Python 스크립트가 존재하지 않습니다: {scriptPath}");
            return;
        }

        // Python 실행
        ProcessStartInfo start = new ProcessStartInfo();
        start.FileName = "python";
        start.Arguments = $"\"{scriptPath}\"";
        start.UseShellExecute = false;
        start.RedirectStandardOutput = true;
        start.RedirectStandardError = true;
        start.CreateNoWindow = true;

        using (Process process = Process.Start(start))
        {
            process.OutputDataReceived += (sender, e) => { if (e.Data != null) UnityEngine.Debug.Log(e.Data); };
            process.BeginOutputReadLine();

            process.ErrorDataReceived += (sender, e) => { if (e.Data != null) UnityEngine.Debug.LogError(e.Data); };
            process.BeginErrorReadLine();

            process.WaitForExit();
        }

        Debug.Log("[AddressableUploader] 업로드 완료 ✅");
    }
}
