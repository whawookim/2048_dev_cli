using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

namespace Puzzle.UI.Overlay
{
    /// <summary>
    /// 게임 설정 팝업
    /// </summary>
    public class SettingPopup : MonoBehaviour, IUIOverlay
    {
        public static SettingPopup Instance { get; private set; }
        
        public static string AddressableName => nameof(SettingPopup);
        
        /// <summary>
        /// 유저 닉네임 표시
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI nickNameText;

        /// <summary>
        /// 유저 아이디 표시
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI userIdText;
        
        /// <summary>
        /// 현재 언어 표시
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI languageText;
        
        /// <summary>
        /// 현재 표시 중인 언어 텍스트 스트링
        /// </summary>
        private LocalizedString _currentLanguageString;
        
        private Dictionary<Locales, LocalizedString> _cachedLocalizedDict = new ();

        #region Monobehaviour
        public void Awake()
        {
            Debug.Assert(Instance == null);
                    
            Instance = this;
        }

        public void OnDestroy()
        {
            Debug.Assert(Instance == this);
                    
            Instance = null;
            
            _cachedLocalizedDict.Clear();
            _cachedLocalizedDict = null;
            
            _currentLanguageString = null;
        }
        #endregion
        
        #region IUIOverlay
        public string Name => nameof(SettingPopup);
        public Flow.UIOverlayManager UIOverlayManager { get; set; }
        public void Begin(object state)
        {
            nickNameText.text = User.Me.NickName;
            userIdText.text = User.Me.UUID;
            UpdateLanguageText(LanguageManager.Instance.CurrentLocales);
        }

        public Task OpenAsync()
        {
            return Task.CompletedTask;
        }

        public Task CloseAsync()
        {
            return Task.CompletedTask;
        }

        public void Finish()
        {
        }

        public void OnClickBackButton()
        {
            OnClockExitButtonClick();
        }
        #endregion

        /// <summary>
        /// 닫기 버튼 or dimd 터치
        /// </summary>
        public void OnClockExitButtonClick()
        {
            Flow.UIFlowManager.Instance.PopOverlay();
        }

        /// <summary>
        /// 언어 버튼 터치하면 다음 언어로 변경 후 playerPref에 저장
        /// </summary>
        public void OnLanguageButtonClick()
        {
            // 현재 언어의 다음 순서의 언어로 변경 및 저장
            var availableLocalesList = LanguageManager.Instance.AvailableLocalesList;
            var currentLocale=  LanguageManager.Instance.CurrentLocale.ToLower();
            int cIndex = availableLocalesList.FindIndex( (s) => s.ToString().ToLower().Equals(currentLocale));
            int len = availableLocalesList.Count;
            int fIndex = cIndex + 1 >= len ? 0 : cIndex + 1;
            var targetLocales = availableLocalesList[fIndex];
            
            LanguageManager.Instance.ChangeLanguage(targetLocales);
            
            UpdateLanguageText(targetLocales);
        }

        private void UpdateLanguageText(Locales locales)
        {
            if (!_cachedLocalizedDict.ContainsKey(locales))
            {
                _cachedLocalizedDict.Add(locales,
                    new LocalizedString(GameStringsManager.DefaultTable, locales.GetStringKey()));
            }
            
            _currentLanguageString = _cachedLocalizedDict[locales];

            _currentLanguageString.StringChanged -= UpdateLanguageText;
            _currentLanguageString.StringChanged += UpdateLanguageText;
            _currentLanguageString.RefreshString();
        }

        private void UpdateLanguageText(string localizedString)
        {
            languageText.text = localizedString;
        }
    }
}
