using System.Collections.Generic;
using UnityEngine;

namespace Puzzle.Stage
{
    /// <summary>
    /// 블록 이동 및 병합 결과를 계산하여 커맨드 리스트로 반환하는 스테이지 로직 처리 클래스.
    /// </summary>
    public static class StageLogic
    {
        /// <summary>
        /// 현재 블록 상태와 입력 방향을 기반으로, 실행할 커맨드 리스트를 생성한다.
        /// </summary>
        /// <param name="blockDict">현재 존재하는 블록들 (index → Block)</param>
        /// <param name="direction">입력된 이동 방향</param>
        /// <param name="board">보드 상태 (Num 배열)</param>
        /// <returns>실행할 커맨드 리스트</returns>
        public static void GenerateMoveCommands(
            Dictionary<Vector2Int, UI.Block> blockDict,
            Vector2Int direction,
            int[,] board,
            CommandExecutor executor)
        {
            executor.Clear();
            HashSet<Vector2Int> mergedPositions = new();

            int width = board.GetLength(0);
            int height = board.GetLength(1);

            // 방향에 따라 루프 순서를 다르게 처리
            IEnumerable<int> xRange = direction.x > 0 ? ReverseRange(width) : ForwardRange(width);
            IEnumerable<int> yRange = direction.y > 0 ? ReverseRange(height) : ForwardRange(height);

            // 수평 이동일 경우 행 기준, 수직 이동일 경우 열 기준으로 분리해서 커맨드 그룹화
            if (direction.x != 0) // 좌우 이동: 행마다 커맨드 그룹
            {
                foreach (int y in yRange)
                {
                    List<IBlockCommand> group = new();

                    foreach (int x in xRange)
                    {
                        Vector2Int from = new(x, y);
                        if (!blockDict.TryGetValue(from, out var block)) continue;

                        Vector2Int to = from;
                        int num = board[x, y];

                        while (true)
                        {
                            Vector2Int next = to + direction;
                            if (!IsInBounds(next, width, height)) break;

                            if (!blockDict.ContainsKey(next))// board[next.x, next.y] == 0)
                            {
                                to = next;
                            }
                            else if (blockDict[next].Number == num && !mergedPositions.Contains(next))// num board[next.x, next.y] == num && !mergedPositions.Contains(next))
                            {
                                to = next;
                                mergedPositions.Add(to);
                                break;
                            }
                            else break;
                        }

                        if (to != from)
                        {
                            blockDict.Remove(from);
                            var toWorldPos = StageManager.Instance.BoardController.GetBoardPosition(to);
                            group.Add(new MoveBlockCommand(block, block.Rect.anchoredPosition, toWorldPos, () =>
                            {
                                StageManager.Instance.StatusController.AddMoveCount(1);
                            }));

                            if (mergedPositions.Contains(to))
                            {
                                if (!blockDict.TryGetValue(to, out var mergedBlock)) continue;

                                group.Add(new MergeBlockCommand(block, mergedBlock, () =>
                                {
                                    StageManager.Instance.StatusController.AddScore(block.Number);
                                }));
                            }
                            blockDict[to] = block;
                        }
                    }

                    if (group.Count > 0)
                        executor.EnqueueGroup(group);
                }
            }
            else if (direction.y != 0) // 상하 이동: 열마다 커맨드 그룹
            {
                foreach (int x in xRange)
                {
                    List<IBlockCommand> group = new();

                    foreach (int y in yRange)
                    {
                        Vector2Int from = new(x, y);
                        if (!blockDict.TryGetValue(from, out var block)) continue;

                        Vector2Int to = from;
                        int num = board[x, y];

                        while (true)
                        {
                            Vector2Int next = to + direction;
                            if (!IsInBounds(next, width, height)) break;

                            if (!blockDict.ContainsKey(next))// board[next.x, next.y] == 0)
                            {
                                to = next;
                            }
                            else if (blockDict[next].Number == num && !mergedPositions.Contains(next))//  board[next.x, next.y] == num && !mergedPositions.Contains(next))
                            {
                                to = next;
                                mergedPositions.Add(to);
                                break;
                            }
                            else break;
                        }

                        if (to != from)
                        {
                            blockDict.Remove(from);
                            var toWorldPos = StageManager.Instance.BoardController.GetBoardPosition(to);
                            group.Add(new MoveBlockCommand(block, block.Rect.anchoredPosition, toWorldPos, () =>
                            {
                                StageManager.Instance.StatusController.AddMoveCount(1);
                            }));

                            if (mergedPositions.Contains(to))
                            {
                                if (!blockDict.TryGetValue(to, out var mergedBlock)) continue;

                                group.Add(new MergeBlockCommand(block, mergedBlock, () =>
                                {
                                    StageManager.Instance.StatusController.AddScore(block.Number);
                                }));
                            }
                            blockDict[to] = block;
                        }
                    }

                    if (group.Count > 0)
                        executor.EnqueueGroup(group);
                }
            }
        }

        private static IEnumerable<int> ForwardRange(int count)
        {
            for (int i = 0; i < count; i++) yield return i;
        }

        private static IEnumerable<int> ReverseRange(int count)
        {
            for (int i = count - 1; i >= 0; i--) yield return i;
        }

        public static bool IsInBounds(Vector2Int pos, int width, int height)
        {
            return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
        }
    }
}
