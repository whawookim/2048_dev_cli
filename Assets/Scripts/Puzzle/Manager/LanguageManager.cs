using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public enum Locales
{
    en,
    ko
}

public static class LocalesExtensions
{
    public static string GetStringKey(this Locales locale)
    {
        return $"language_{locale.ToString()}";
    }
}

namespace Puzzle
{
    public class LanguageManager
    {
        private static LanguageManager _instance;
        public static LanguageManager Instance => _instance ??= new LanguageManager();

        public Locale CurrentLocaleData => LocalizationSettings.SelectedLocale;

        public string CurrentLocale => CurrentLocaleData?.Identifier.Code ?? DefaultCode;

        public Locales CurrentLocales
        {
            get
            {
                // 현재 언어의 다음 순서의 언어로 변경 및 저장
                var localeList = AvailableLocalesList;
            
                // TODO: 플랫폼이나 퍼블리셔마다 다른 언어 지원을 할 경우 제어 필요
            
                var currentLocale=  CurrentLocale.ToLower();
                int cIndex = localeList.FindIndex( (s) => s.ToString().ToLower().Equals(currentLocale));
                return cIndex < 0 ? DefaultCodeEnum : localeList[cIndex];
            }
        }

        public static string DefaultCode => DefaultCodeEnum.ToString();

        public static Locales DefaultCodeEnum => Locales.en;

        public const string LanguagePrefKey = "selected_language_code";

        private List<Locales> _localesList;
        
        public List<Locales> AvailableLocalesList
        {
            get
            {
                if (_localesList == null)
                {
                    _localesList = GetLocalesList();
                }

                return _localesList;
            }
        }

        /// <summary>
        /// 게임 실행시 한 번 초기화
        /// </summary>
        /// <remarks>생성자에 안 하고 따로 한 이유는 호출 시점 제어용</remarks>
        public async Task InitializeAsync()
        {
            // 초기화
            await LocalizationSettings.InitializationOperation.Task;
            
            var code = PlayerPrefs.GetString(LanguagePrefKey);

            if (!string.IsNullOrEmpty(code))
            {
                ChangeLanguage(code);
                return;
            }
            
            // 한 번도 세팅 안된 경우 기기의 언어 Code를 따르자
            ChangeLanguage(CurrentLocale);
        }
        
        /// <summary>
        /// 언어 변경
        /// </summary>
        public void ChangeLanguage(string code)
        {
            ChangeAndSaveLanguage(code);
        }

        public void ChangeLanguage(Locales codeEnum)
        {
            ChangeAndSaveLanguage(codeEnum.ToString());
        }

        private void ChangeAndSaveLanguage(string code)
        {
            if (CurrentLocale.Equals(code)) return;
            
            var locale = LocalizationSettings.AvailableLocales.Locales
                .Find(l => l.Identifier.Code == code);

            LocalizationSettings.SelectedLocale = locale;

            SaveLocalCode(code);
        }

        /// <summary>
        /// 로컬 기기에 현재 언어 저장
        /// </summary>
        /// <remarks>서버에 저장할 필요까진 없어 보여서 일단 로컬로</remarks>
        private void SaveLocalCode(string code)
        {
            PlayerPrefs.SetString("selected_language_code", code);
            PlayerPrefs.Save();
        }

        public List<Locales> GetLocalesList()
        {
            // TODO: 플랫폼이나 퍼블리셔마다 다른 언어 지원을 할 경우 제어 필요
            return new List<Locales>()
            {
                Locales.en,
                Locales.ko
            };
        }
    }
}
