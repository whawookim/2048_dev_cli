using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.UI
{
    public class LoadingScreen : MonoBehaviour
    {
        public static LoadingScreen Instance { get; private set; }

        /// <summary>
        /// 로딩 이미지
        /// </summary>
        [SerializeField]
        private Image loadingImage;

        [SerializeField]
        private TextMeshProUGUI loadingText;
        
        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            Instance = null;
        }
        
        public void SetEnabled(bool isBlocked = false)
        {
            if (isBlocked)
            {
                UIBlocker.Instance.enabled = true;
            }
            
            // TODO: Fade 연출?
            loadingImage.gameObject.SetActive(true);
            loadingText.gameObject.SetActive(true);
        }

        public void SetDisabled(bool isBlocked = false)
        {
            if (isBlocked)
            {
                UIBlocker.Instance.enabled = false;
            }
            
            // TODO: Fade 연출?
            loadingImage.gameObject.SetActive(false);
            loadingText.gameObject.SetActive(false);
        }
    }
}
