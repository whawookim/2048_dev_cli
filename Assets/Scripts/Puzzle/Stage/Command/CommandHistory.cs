using System.Collections.Generic;
using System.Threading.Tasks;

namespace Puzzle.Stage
{
    /// <summary>
    /// 실행된 블록 커맨드의 히스토리를 저장하여 Undo 및 Replay에 사용된다.
    /// </summary>
    public static class CommandHistory
    {
        private static readonly Stack<IBlockCommand> _history = new();

        public static void Push(IBlockCommand command)
        {
            _history.Push(command);
        }

        public static async Task UndoLastAsync()
        {
            if (_history.Count == 0) return;

            var command = _history.Pop();
            await command.UndoAsync();
        }

        public static void Clear()
        {
            _history.Clear();
        }

        public static int Count => _history.Count;
    }
}
