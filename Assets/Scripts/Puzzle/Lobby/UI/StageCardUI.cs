using UnityEngine;

namespace Puzzle.UI
{
	public class StageCardUI : MonoBehaviour
	{
		[field:SerializeField]
		public StageMode Mode { get; private set; }
	}
}
