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
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;

        string currentLocale = LocalizationSettings.SelectedLocale?.Identifier.Code ?? "en";
        await ApplyFontFallbacks(currentLocale);
    }

    private void OnDestroy()
    {
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

    public async Task ApplyFontFallbacks(string locale)
    {
        Debug.Log($"[FontManager] Applying fallback for locale: {locale}");

        List<TMP_FontAsset> fallbackList = await LoadFallbackFontsForLocale(locale);

        ApplyFallbackToFont(uiBaseFont, fallbackList);
        ApplyFallbackToFont(contentBaseFont, fallbackList);
        ApplyFallbackToTMPSettings(fallbackList);
    }

    private async Task<List<TMP_FontAsset>> LoadFallbackFontsForLocale(string locale)
    {
        if (_localeFontCache.TryGetValue(locale, out var cached))
            return cached;

        var mapping = localeFontMappings.FirstOrDefault(x => x.localeCode == locale);
        if (mapping == null)
        {
            Debug.LogWarning($"[FontManager] No fallback fonts found for locale: {locale}");
            return new List<TMP_FontAsset>();
        }

        var result = new List<TMP_FontAsset>();

        foreach (var reference in mapping.fallbackFonts)
        {
            var font = LoadFontInEditorOrRuntime(reference);
            if (font != null)
            {
                result.Add(font);
            }
        }

        _localeFontCache[locale] = result;
        return result;
    }

    private TMP_FontAsset LoadFontInEditorOrRuntime(AssetReferenceT<TMP_FontAsset> reference)
    {
#if UNITY_EDITOR
        string guid = reference.AssetGUID;
        string path = AssetDatabase.GUIDToAssetPath(guid);
        TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);

        if (font != null)
        {
            Debug.Log($"[FontManager] (Editor) Loaded font: {font.name}");
            FixFontMaterial(font);
            return font;
        }
        else
        {
            Debug.LogWarning($"[FontManager] (Editor) Failed to load font via AssetDatabase: {reference.RuntimeKey}");
            return null;
        }
#else
        var handle = reference.LoadAssetAsync();
        handle.WaitForCompletion();

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _runtimeHandles.Add(handle);
            FixFontMaterial(handle.Result);
            Debug.Log($"[FontManager] (Runtime) Loaded font: {handle.Result.name}");
            return handle.Result;
        }
        else
        {
            Debug.LogError($"[FontManager] (Runtime) Failed to load font: {reference.RuntimeKey}");
            return null;
        }
#endif
    }

    private void ApplyFallbackToFont(TMP_FontAsset baseFont, List<TMP_FontAsset> fallbacks)
    {
        if (baseFont == null)
        {
            Debug.LogWarning("[FontManager] Base font is null.");
            return;
        }

        baseFont.fallbackFontAssetTable = fallbacks;
        Debug.Log($"[FontManager] Applied fallback to {baseFont.name}: {string.Join(", ", fallbacks.Select(f => f.name))}");
    }

    private void ApplyFallbackToTMPSettings(List<TMP_FontAsset> fallbacks)
    {
        TMP_Settings.fallbackFontAssets.Clear();
        TMP_Settings.fallbackFontAssets.AddRange(fallbacks);
        Debug.Log($"[FontManager] TMP_Settings fallback applied: {string.Join(", ", fallbacks.Select(f => f.name))}");
    }

    private void FixFontMaterial(TMP_FontAsset fontAsset)
    {
        if (fontAsset == null) return;

        if (fontAsset.material == null)
        {
            fontAsset.material = new Material(Shader.Find("TextMeshPro/Distance Field"));
            Debug.LogWarning($"Material가 null이어서 새로 생성: {fontAsset.name}");
        }

        if (fontAsset.material.mainTexture == null)
        {
            fontAsset.material.mainTexture = fontAsset.atlasTexture;
            Debug.LogWarning($"mainTexture가 없어서 atlas로 연결함: {fontAsset.name}");
        }
    }
}
