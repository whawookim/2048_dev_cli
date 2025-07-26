using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Puzzle.Stage
{
    /// <summary>
    /// 블록 병합 연출을 실행하는 커맨드.
    /// </summary>
    public class MergeBlockCommand : IBlockCommand
    {
        private readonly UI.Block _targetBlock;
        private readonly UI.Block _mergedBlock;
        public Action ExecuteAction { get; set; }

        public MergeBlockCommand(UI.Block targetBlock, UI.Block mergedBlock, Action executeCallback = null)
        {
            _targetBlock = targetBlock;
            _mergedBlock = mergedBlock;
            ExecuteAction = executeCallback;
        }

        public async Task ExecuteAsync()
        {
            var tasks = new Task[]
            {
                Animation.UIAnimations.ScaleAsync(_mergedBlock.Rect, Vector3.zero),
                Animation.UIAnimations.MergeAsync(_targetBlock.Rect)
            };

            await Task.WhenAll(tasks);

            _targetBlock.Init(_targetBlock.Number * 2);
            _mergedBlock.Hide();
            
            ExecuteAction?.Invoke();
        }

        public Task UndoAsync()
        {
            // 병합 Undo는 선택 사항: 시각적으로만 되돌릴 수 있음
            return Task.CompletedTask;
        }
    }
}
