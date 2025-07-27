using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Puzzle.Stage
{
    /// <summary>
    /// 블록을 지정된 위치로 이동시키는 커맨드.
    /// </summary>
    public class MoveBlockCommand : IBlockCommand
    {
        private readonly UI.Block _block;
        private readonly Vector3 _toPosition;
        private readonly Vector3 _fromPosition;
        public Action ExecuteAction { get; set; }

        public MoveBlockCommand(UI.Block block, Vector3 from, Vector3 to, Action executeCallback = null)
        {
            _block = block;
            _fromPosition = from;
            _toPosition = to;
            ExecuteAction = executeCallback;
        }

        public async Task ExecuteAsync()
        {
            _block.SetPosition(_fromPosition); // 시작 위치 세팅
            await Animation.UIAnimations.MoveAsync(_block.Rect, _toPosition);
            ExecuteAction?.Invoke();
        }

        public async Task UndoAsync()
        {
            await Animation.UIAnimations.MoveAsync(_block.Rect, _fromPosition);
        }
    }
}
