using System.Collections.Generic;
using System.Threading.Tasks;

namespace Puzzle.Stage
{
    /// <summary>
    /// 큐에 저장된 블록 커맨드를 **그룹 단위로 동시에 실행**.
    /// </summary>
    public class CommandExecutor
    {
        private readonly Queue<List<IBlockCommand>> _commandGroups = new();

        public void EnqueueGroup(List<IBlockCommand> group)
        {
            _commandGroups.Enqueue(group);
        }

        public async Task ExecuteAllAsync()
        {
            var allTasks = new List<Task>();

            while (_commandGroups.Count > 0)
            {
                var group = _commandGroups.Dequeue();
                foreach (var command in group)
                {
                    allTasks.Add(command.ExecuteAsync());
                    CommandHistory.Push(command);
                }
            }

            await Task.WhenAll(allTasks); 
            
            /*while (_commandGroups.Count > 0)
            {
                var group = _commandGroups.Dequeue();

                var tasks = new List<Task>();
                foreach (var command in group)
                {
                    tasks.Add(command.ExecuteAsync());
                    CommandHistory.Push(command);
                }

                await Task.WhenAll(tasks); // 한 그룹은 동시에 실행
            }*/
        }

        public void Clear()
        {
            _commandGroups.Clear();
        }

        public int Count => _commandGroups.Count;
    }
}
