using System;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.UI
{
    /// <summary>
    /// IDP 선택 버튼
    /// </summary>
    /// <remarks>선택 버튼은 그저 선택 버튼으로 콜백을 호출해준다.</remarks>
    public class IDPChoiceButton : MonoBehaviour
    {
        [field:SerializeField]
        public LoginType LoginType { get; set; }

        [SerializeField]
        private Button button;

        private Action<LoginType> buttonCallback;

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public void Init(Action<LoginType> buttonAction)
        {
            buttonCallback = buttonAction;
        }

        public void SetVisualState(bool isBound)
        {
            button.interactable = !isBound;
        }

        public void OnButtonClick()
        {
            buttonCallback.Invoke(LoginType);
        }
    }
}