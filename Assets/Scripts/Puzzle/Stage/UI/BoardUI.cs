using UnityEngine;

namespace Puzzle.UI
{
    /// <summary>
    /// 보드판 UI
    /// </summary>
    public class BoardUI : MonoBehaviour
    {
        [SerializeField]
        private Transform blockTransform;

        [SerializeField]
        private Transform boardTransform;

        public Transform GetBlockParent() => blockTransform;
        public Transform GetBoardParent() => boardTransform;
    }
}
