using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.UI
{
	public class UIBlocker : MonoBehaviour
	{
		public static UIBlocker Instance { get; private set; }

		/// <summary>
		/// Image가 붙어있고 Raycast Target이 되는 오브젝트
		/// </summary>
		[SerializeField]
		private Image blockerImage;

		private void Awake()
		{
			Instance = this;
		}

		private void OnDestroy()
		{
			Instance = null;
		}

		public void SetEnabled()
		{
			blockerImage.enabled = true;
		}

		public void SetDisabled()
		{
			blockerImage.enabled = false;
		}
	}
}
