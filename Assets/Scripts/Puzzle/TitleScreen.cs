using UnityEngine;
using System.Threading.Tasks;

namespace Puzzle
{
    /// <summary>
    /// 타이틀 스크린
    /// </summary>
    public class TitleScreen : MonoBehaviour
    {
        private static TitleScreen instance;

        public static TitleScreen Instance => instance;

        public void Awake()
        {
            instance = this;
            
            Debug.Assert(this != null);
        }

        public void Destroy()
        {
            instance = null;
            
            Debug.Assert(this == null);
        }

        /// <summary>
        /// 로그인 프로세스
        /// </summary>
        public async Task LoginProcess()
        {
            if (!PlayerPrefs.HasKey("guest_uuid"))
            {
                // UUID 없음 → 팝업: 어떤 방식으로 로그인할지 선택
                ShowLoginChoicePopup(); // Google 로그인 or 새로 시작
            }
            else
            {
                // 게스트 로그인 시도
                var result = await LoginManager.Instance.LoginAsync(LoginType.Guest);
            }
        }

        /// <summary>
        /// IDP 로그인 팝업
        /// </summary>
        public void ShowLoginChoicePopup()
        {
            
        }
    }
}
