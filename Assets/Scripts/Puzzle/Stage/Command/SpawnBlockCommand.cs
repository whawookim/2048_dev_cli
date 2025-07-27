using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Puzzle.Stage
{
    /// <summary>
    /// 새로운 블록을 보드 상에 생성하는 커맨드.
    /// 생성 시 스케일 0에서 1로 커지는 연출 포함.
    /// </summary>
    public class SpawnBlockCommand : IBlockCommand
    {
        private readonly UI.Block _block;
        private readonly Vector3 _spawnPosition;
        public Action ExecuteAction { get; set; }

        public SpawnBlockCommand(UI.Block block, Vector3 spawnPosition, Action onExecuteCallback = null)
        {
            _block = block;
            _spawnPosition = spawnPosition;
        }

        public async Task ExecuteAsync()
        {
            _block.SetPosition(_spawnPosition);
            _block.SetScale(Vector3.zero);
            _block.Show();

            await Animation.UIAnimations.ScaleAsync(_block.Rect, Vector3.one);
            ExecuteAction?.Invoke();
        }

        public Task UndoAsync()
        {
            _block.Hide();
            return Task.CompletedTask;
        }
    }
}
