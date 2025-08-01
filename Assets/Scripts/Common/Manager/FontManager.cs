using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 언어(로케일)에 따라 UI 및 콘텐츠 폰트에 fallback 폰트를 자동으로 적용하는 매니저.
/// Addressables 기반으로 폰트를 로드하며, TMP Settings에도 글로벌 fallback을 설정합니다.
/// </summary>
public class FontManager : MonoBehaviour
{
    [Header("Base Fonts")]
    public TMP_FontAsset uiBaseFont;
    public TMP_FontAsset contentBaseFont;

    [System.Serializable]
    public class LocaleFontMapping
    {
        public string localeCode;
        public List<AssetReferenceT<TMP_FontAsset>> fallbackFonts;
    }

    [Header("Fallback Fonts Per Locale")]
    public List<LocaleFontMapping> localeFontMappings = new();

    private Dictionary<string, List<TMP_FontAsset>> _localeFontCache = new();
    private List<AsyncOperationHandle<TMP_FontAsset>> _runtimeHandles = new();

    private async void Awake()
    {
        // 언어 변경 시 fallback 재적용
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    private void OnDestroy()
    {
        // 런타임 핸들 해제
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;

        // 런타임 핸들 해제
        foreach (var handle in _runtimeHandles)
        {
            Addressables.Release(handle);
        }

        _runtimeHandles.Clear();
    }

    private async void OnLocaleChanged(UnityEngine.Localization.Locale locale)
    {
        await ApplyFontFallbacks(locale.Identifier.Code);
    }

    public async Task InitializeAsync()
    {
        MyDebug.LogError("[FontManager] InitializeAsync 0");
        
        await LocalizationSettings.InitializationOperation.Task;
        
        MyDebug.LogError("[FontManager] InitializeAsync 1");
        
        string currentLocale = LocalizationSettings.SelectedLocale?.Identifier.Code ?? "en";
        await ApplyFontFallbacks(currentLocale);
        
        MyDebug.LogError("[FontManager] InitializeAsync 2");
    }

    /// <summary>
    /// 주어진 locale 코드에 따라 fallback 폰트를 로드하고 적용
    /// </summary>
    public async Task ApplyFontFallbacks(string locale)
    {
        MyDebug.Log($"[FontManager] Applying fallback for locale: {locale}");

        List<TMP_FontAsset> fallbackList = await LoadFallbackFontsForLocale(locale);

        ApplyFallbackToFont(uiBaseFont, fallbackList);
        ApplyFallbackToFont(contentBaseFont, fallbackList);
        ApplyFallbackToTMPSettings(fallbackList);
    }

    /// <summary>
    /// 언어별 fallback 폰트 목록을 Addressable 기반으로 로드
    /// </summary>
    private async Task<List<TMP_FontAsset>> LoadFallbackFontsForLocale(string locale)
    {
        if (_localeFontCache.TryGetValue(locale, out var cached))
            return cached;

        var mapping = localeFontMappings.FirstOrDefault(x => x.localeCode == locale);
        if (mapping == null)
        {
            MyDebug.LogWarning($"[FontManager] No fallback fonts found for locale: {locale}");
            return new List<TMP_FontAsset>();
        }

        var result = new List<TMP_FontAsset>();

        foreach (var reference in mapping.fallbackFonts)
        {
            var font = await LoadFontInEditorOrRuntimeAsync(reference);
            if (font != null)
            {
                result.Add(font);
            }
        }

        _localeFontCache[locale] = result;
        return result;
    }

    /// <summary>
    /// 에디터와 런타임에 따라 폰트를 로드하는 함수
    /// </summary>
    private async Task<TMP_FontAsset> LoadFontInEditorOrRuntimeAsync(AssetReferenceT<TMP_FontAsset> reference)
    {
#if UNITY_EDITOR
        string guid = reference.AssetGUID;
        string path = AssetDatabase.GUIDToAssetPath(guid);
        TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);

        if (font != null)
        {
            MyDebug.Log($"[FontManager] (Editor) Loaded font: {font.name}");
            FixFontMaterial(font);
            return font;
        }
        else
        {
            MyDebug.LogWarning($"[FontManager] (Editor) Failed to load font via AssetDatabase: {reference.RuntimeKey}");
            return null;
        }
#else
        var handle = reference.LoadAssetAsync();
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _runtimeHandles.Add(handle);
            FixFontMaterial(handle.Result);
            MyDebug.Log($"[FontManager] (Runtime) Loaded font: {handle.Result.name}");
            return handle.Result;
        }
        else
        {
            MyDebug.LogError($"[FontManager] (Runtime) Failed to load font: {reference.RuntimeKey}");
            return null;
        }
#endif
    }

    /// <summary>
    /// 단일 baseFont에 fallback 목록 적용
    /// </summary>
    private void ApplyFallbackToFont(TMP_FontAsset baseFont, List<TMP_FontAsset> fallbacks)
    {
        if (baseFont == null)
        {
            MyDebug.LogWarning("[FontManager] Base font is null.");
            return;
        }

        baseFont.fallbackFontAssetTable = fallbacks;
        MyDebug.Log($"[FontManager] Applied fallback to {baseFont.name}: {string.Join(", ", fallbacks.Select(f => f.name))}");
    }

    /// <summary>
    /// TMP 글로벌 fallback 설정 적용
    /// </summary>
    private void ApplyFallbackToTMPSettings(List<TMP_FontAsset> fallbacks)
    {
        TMP_Settings.fallbackFontAssets.Clear();
        TMP_Settings.fallbackFontAssets.AddRange(fallbacks);
        MyDebug.Log($"[FontManager] TMP_Settings fallback applied: {string.Join(", ", fallbacks.Select(f => f.name))}");
    }

    /// <summary>
    /// 폰트 Asset의 Material 누락이나 atlas 미연결 문제를 보정
    /// </summary>
    private void FixFontMaterial(TMP_FontAsset fontAsset)
    {
        if (fontAsset == null) return;

        if (fontAsset.material == null)
        {
            fontAsset.material = new Material(Shader.Find("TextMeshPro/Distance Field"));
            MyDebug.LogWarning($"Material가 null이어서 새로 생성: {fontAsset.name}");
        }

        if (fontAsset.material.mainTexture == null)
        {
            fontAsset.material.mainTexture = fontAsset.atlasTexture;
            MyDebug.LogWarning($"mainTexture가 없어서 atlas로 연결함: {fontAsset.name}");
        }
    }
}
