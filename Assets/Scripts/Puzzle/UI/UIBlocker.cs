using UnityEngine;

namespace Puzzle.UI
{
	public class UIBlocker : MonoBehaviour
	{
		public static UIBlocker Instance { get; private set; }

		[SerializeField]
		private BoxCollider collider;

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
			collider.enabled = true;
		}

		public void SetDisabled()
		{
			collider.enabled = false;
		}
	}
}
