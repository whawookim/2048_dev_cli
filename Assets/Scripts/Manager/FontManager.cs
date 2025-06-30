using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class FontManager : MonoBehaviour
{
    public static FontManager Instance { get; private set; }

    [Header("UI Base Font (Empty)")]
    [SerializeField]
    private TMP_FontAsset uiBaseFont;

    [Header("Content Base Font (Empty)")]
    [SerializeField]
    private TMP_FontAsset contentBaseFont;

    [Header("UI Font Label Mappings")]
    [SerializeField]
    private List<FontLabelMapping> uiFontLabelMappings;

    [Header("Content Font Labels")]
    [SerializeField]
    private List<string> contentFontLabels;

    private List<TMP_FontAsset> currentUIFallbacks = new();
    private List<TMP_FontAsset> currentContentFallbacks = new();

    private List<AsyncOperationHandle<TMP_FontAsset>> handles = new();

    public TMP_FontAsset UIFont => uiBaseFont;
    public TMP_FontAsset ContentFont => contentBaseFont;

    [Serializable]
    public struct FontLabelMapping
    {
        public string locale;
        public List<string> addressableLabels;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
            InitializeFonts();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        ReleaseAllFonts();
    }

    private void OnLocaleChanged(Locale locale)
    {
        LoadUIFont(locale);
    }

    private void InitializeFonts()
    {
        LoadUIFont(LocalizationSettings.SelectedLocale);
        LoadContentFont();
    }

    /// <summary>
    /// UI Font
    /// </summary>
    private void LoadUIFont(Locale locale)
    {
        var mapping = uiFontLabelMappings.Find(m => m.locale == locale.Identifier.ToString());
        if (mapping.addressableLabels == null || mapping.addressableLabels.Count == 0)
        {
            Debug.LogWarning($"No UI font mapping for {locale.Identifier}, fallback to English.");
            mapping = uiFontLabelMappings.Find(m => m.locale == "en");
        }

        ReleaseCurrentFonts(currentUIFallbacks);
        currentUIFallbacks.Clear();

        foreach (var label in mapping.addressableLabels)
        {
            var handle = Addressables.LoadAssetAsync<TMP_FontAsset>(label);
            handles.Add(handle);

            handle.Completed += h =>
            {
                if (h.Status == AsyncOperationStatus.Succeeded)
                {
                    currentUIFallbacks.Add(h.Result);
                    ApplyUIFallback();
                }
                else
                {
                    Debug.LogError($"Failed to load font for label {label}");
                }
            };
        }
    }

    private void ApplyUIFallback()
    {
        if (uiBaseFont != null)
        {
            uiBaseFont.fallbackFontAssetTable = currentUIFallbacks;
        }
    }

    /// <summary>
    /// Content Font
    /// </summary>
    private void LoadContentFont()
    {
        ReleaseCurrentFonts(currentContentFallbacks);
        currentContentFallbacks.Clear();

        foreach (var label in contentFontLabels)
        {
            var handle = Addressables.LoadAssetAsync<TMP_FontAsset>(label);
            handles.Add(handle);

            handle.Completed += h =>
            {
                if (h.Status == AsyncOperationStatus.Succeeded)
                {
                    currentContentFallbacks.Add(h.Result);
                    ApplyContentFallback();
                }
                else
                {
                    Debug.LogError($"Failed to load content font for label {label}");
                }
            };
        }
    }

    private void ApplyContentFallback()
    {
        if (contentBaseFont != null)
        {
            contentBaseFont.fallbackFontAssetTable = currentContentFallbacks;
        }
    }

    private void ReleaseAllFonts()
    {
        ReleaseCurrentFonts(currentUIFallbacks);
        ReleaseCurrentFonts(currentContentFallbacks);
    }

    private void ReleaseCurrentFonts(List<TMP_FontAsset> fallbackList)
    {
        fallbackList.Clear();
    }
}
