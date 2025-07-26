using System;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle.Stage
{
    public class StageBoardModel
    {
        public int[,] Board { get; private set; }
        public int MaxSize { get; }
        public int MaxNum { get; }

        public StageBoardModel(int maxSize, int maxNum)
        {
            MaxSize = maxSize;
            MaxNum = maxNum;
            Board = new int[maxSize, maxSize];
        }

        public void UpdateBoardState(Dictionary<Vector2Int, UI.Block> blockDict)
        {
            for (int x = 0; x < MaxSize; x++)
            for (int y = 0; y < MaxSize; y++)
                Board[x, y] = 0;

            foreach (var kvp in blockDict)
                Board[kvp.Key.x, kvp.Key.y] = kvp.Value.Number;
        }

        public bool IsGameClear()
        {
            foreach (var val in Board)
                if (val == MaxNum) return true;
            return false;
        }

        public bool IsGameOver(Dictionary<Vector2Int, UI.Block> blockDict)
        {
            if (!IsFull()) return false;

            foreach (var kvp in blockDict)
                if (HasSameValueAround(kvp.Key)) return false;

            return true;
        }

        private bool IsFull()
        {
            foreach (var val in Board)
                if (val == 0) return false;
            return true;
        }

        private bool HasSameValueAround(Vector2Int pos)
        {
            int value = Board[pos.x, pos.y];
            foreach (var dir in (MoveDirection[])Enum.GetValues(typeof(MoveDirection)))
            {
                if (dir == MoveDirection.None) continue;
                var checkPos = pos + dir.GetMoveVec();

                if (!StageLogic.IsInBounds(checkPos, MaxSize, MaxSize)) continue;
                if (Board[checkPos.x, checkPos.y] == value) return true;
            }
            return false;
        }
    }
}
