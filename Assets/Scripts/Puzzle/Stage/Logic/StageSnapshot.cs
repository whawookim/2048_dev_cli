using System;
using UnityEngine;
using System.Collections.Generic;

namespace Puzzle.Stage
{
    [Serializable]
    public class StageSnapshot
    {
        public List<BlockData> Blocks = new();
        public int Score;

        [Serializable]
        public class BlockData
        {
            public Vector2Int Position;
            public int Number;
        }
    }
}
