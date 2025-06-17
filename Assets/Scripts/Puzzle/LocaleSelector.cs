using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Puzzle
{
    public enum Locales
    {
        en,
        ko
    }
    
    public class LocaleSelector : MonoBehaviour
    {
        [SerializeField]
        private Locales currentLocale;

        private Locales lastLocale; // 변경 감지용

        private void Awake()
        {
            UpdateLocale();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Application.isPlaying) return;

            if (lastLocale != currentLocale)
            {
                lastLocale = currentLocale;
                UpdateLocale();
            }
        }
#endif

        [ContextMenu("Apply Locale Now")]
        public void UpdateLocale()
        {
            var locale = LocalizationSettings.AvailableLocales.Locales
                .Find(l => l.Identifier.Code == currentLocale.ToString());

            LocalizationSettings.SelectedLocale = locale;
            Debug.Log($"[LocaleSelector] Locale changed to: {locale?.Identifier.Code}");
        }
    }
}
