using System;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle.Stage
{
    /// <summary>
    /// 현재 스테이지의 보드 상태 정보를 관리하는 데이터 전용 모델 클래스
    /// - 게임 클리어/오버 판정
    /// - 현재 블록 상태 동기화
    /// - 보드 숫자 배열 제공
    /// </summary>
    public class StageBoardModel
    {
        /// <summary>
        /// 블록 숫자 상태를 나타내는 2차원 배열 (board[x,y] = block value)
        /// </summary>
        public int[,] Board { get; private set; }
        
        /// <summary>
        /// 보드 크기 (가로/세로 N)
        /// </summary>
        public int MaxSize { get; }
        
        /// <summary>
        /// 게임 클리어 조건: 이 수치의 블록이 생성되면 클리어
        /// </summary>
        public int MaxNum { get; }

        public StageBoardModel(int maxSize, int maxNum)
        {
            MaxSize = maxSize;
            MaxNum = maxNum;
            Board = new int[maxSize, maxSize];
        }

        /// <summary>
        /// 현재 블록 배치를 바탕으로 내부 보드 상태(Board)를 갱신함
        /// </summary>
        public void UpdateBoardState(Dictionary<Vector2Int, UI.Block> blockDict)
        {
            for (int x = 0; x < MaxSize; x++)
            for (int y = 0; y < MaxSize; y++)
                Board[x, y] = 0;

            foreach (var kvp in blockDict)
                Board[kvp.Key.x, kvp.Key.y] = kvp.Value.Number;
        }

        /// <summary>
        /// 보드 내에 MaxNum과 같은 수치의 블록이 있으면 게임 클리어로 간주
        /// </summary>
        public bool IsGameClear()
        {
            foreach (var val in Board)
                if (val == MaxNum) return true;
            return false;
        }

        /// <summary>
        /// 게임 오버 조건 판정:
        /// - 모든 칸이 채워져 있고
        /// - 인접한 블록끼리 병합할 수 없을 때
        /// </summary>
        public bool IsGameOver(Dictionary<Vector2Int, UI.Block> blockDict)
        {
            if (!IsFull()) return false;

            foreach (var kvp in blockDict)
                if (HasSameValueAround(kvp.Key)) return false;

            return true;
        }

        /// <summary>
        /// 모든 칸이 채워졌는지 여부 (0이 하나라도 있으면 false)
        /// </summary>
        private bool IsFull()
        {
            foreach (var val in Board)
                if (val == 0) return false;
            return true;
        }

        /// <summary>
        /// 상하좌우 인접한 칸 중 같은 숫자가 존재하는지 확인
        /// → 병합 가능성 판단용
        /// </summary>
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
