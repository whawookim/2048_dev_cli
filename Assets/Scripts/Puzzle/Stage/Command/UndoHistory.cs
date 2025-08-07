using System.Collections.Generic;

namespace Puzzle.Stage
{
    public static class UndoHistory
    {
        private static readonly Stack<StageSnapshot> Stack = new();

        public static void Push(StageSnapshot snapshot) => Stack.Push(snapshot);
        public static StageSnapshot Pop() => Stack.Count > 0 ? Stack.Pop() : null;
        public static bool CanUndo => Stack.Count > 0;
        public static void Clear() => Stack.Clear();
    }
}
