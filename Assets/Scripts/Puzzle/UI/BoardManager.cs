using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Puzzle.UI
{
	/// <summary>
	/// TODO: 코드 정리가 필요할것
	/// </summary>
	public class BoardManager : MonoBehaviour
	{
		/// <summary>
		/// TODO: MaxSize 변경 적용 가능하게
		/// </summary>
		private int MaxSize => Game.Instance.CurrentStage.GetBoardSize();

		[SerializeField]
		private Board originBoard;

		[SerializeField]
		private Block originBlock;

		[SerializeField]
		private UIGrid grid;

		[SerializeField]
		private Transform blockTransform;

		private Coroutine moveCoroutine;

		private readonly List<Board> boards = new List<Board>();

		/// <summary>
		/// 현재 위치에 있는 Block 캐시
		/// </summary>
		private readonly Dictionary<int, Block> blockDict = new Dictionary<int, Block>();

		private readonly ObjectPool<Board> objectPoolBoard = new ObjectPool<Board>();

		private readonly ObjectPool<Block> objectPoolBlock = new ObjectPool<Block>();

		private void Awake()
		{
			originBlock.gameObject.SetActive(false);
		}

		private void OnEnable()
		{
			MessageSystem.Instance.Subscribe<BlockMoveEvent>(OnMoveBlockEvent);
		}

		private void OnDisable()
		{
			MessageSystem.Instance.Unsubscribe<BlockMoveEvent>(OnMoveBlockEvent);
		}

		public int GetRandomBlockIndex()
		{
			var candidates = new List<int>();

			for (var i = 0; i < MaxSize * MaxSize; i++)
			{
				if (blockDict.ContainsKey(i) && blockDict[i] != null) continue;

				candidates.Add(i);
			}

			if (candidates.Count == 0) return -1;

			return candidates[Random.Range(0, candidates.Count)];
		}

		/// <summary>
		/// 보드 비우기
		/// </summary>
		private void ClearBoard()
		{
			objectPoolBoard.Dispose();
			objectPoolBlock.Dispose();

			foreach (var board in boards)
			{
				DestroyImmediate(board.gameObject);
			}

			boards.Clear();

			foreach (var block in blockDict)
			{
				if (block.Value == null) continue;

				DestroyImmediate(block.Value.gameObject);
			}

			blockDict.Clear();
		}

		public void SetBoard()
		{
			ClearBoard();

			var mode = Game.Instance.CurrentStage;
			var blockSize = mode.GetBlockSize();
			var gridSize = mode.GetGridSize();

			originBlock.SetSize(blockSize);

			objectPoolBlock.Init(originBlock, blockTransform, MaxSize * MaxSize);
			objectPoolBoard.Init(originBoard, originBoard.transform.parent, MaxSize * MaxSize);

			for (var i = 0; i < MaxSize * MaxSize; i++)
			{
				var obj = objectPoolBoard.GetOrCreate();
				obj.Set(blockSize, $"board{i}");
				boards.Add(obj);
			}

			originBoard.gameObject.SetActive(false);
			originBlock.gameObject.SetActive(false);

			grid.cellHeight = gridSize;
			grid.cellWidth = gridSize;
			grid.maxPerLine = MaxSize;
			grid.Reposition();
		}

		public bool CreateBlock()
		{
			var blockIndex = GetRandomBlockIndex();

			// TODO: 게임 종료
			if (blockIndex == -1)
			{
				return false;
			}

			var blockObj = objectPoolBlock.GetOrCreate();

			blockObj.gameObject.SetActive(true);

			blockObj.transform.localScale = Vector3.one;

			var block = blockObj.GetComponent<Block>();

			// 2 or 4가 나오게 설정하기 위함
			var rangeLimit = 0.95f;
			var range = Random.Range(0.0f, 1.0f);

			var initVal = (range > rangeLimit) ? 4 : 2;

			block.Init(initVal, blockIndex);

			if (!blockDict.ContainsKey(blockIndex))
			{
				blockDict.Add(blockIndex, block);
			}
			else
			{
				blockDict[blockIndex] = block;
			}

			return true;
		}

		private bool OnMoveBlockEvent(Events e)
		{
			if (e is BlockMoveEvent bme)
			{
				if (moveCoroutine != null) return false;

				var direction = bme.Direction;
				// Debug.LogWarning(bme.Direction);

				if (direction == MoveDirection.None) return false;

				var moveResult = GetMoveBlockResult(direction);

				moveCoroutine = StartCoroutine(MoveBlocksAndMerge(moveResult));

				return true;
			}

			return false;
		}

		/// <summary>
		/// 움직이게 된 후 결과 딕셔너리 저장
		/// TODO: GC Alloc 많이 되는 부분
		/// </summary>
		private Dictionary<Block, int> GetMoveBlockResult(MoveDirection direction)
		{
			var moveDict = new Dictionary<Block, int>();
			var movePairList = new List<KeyValuePair<Block, int>>();

			var disOrder = (direction == MoveDirection.Down || direction == MoveDirection.Right);

			var j = (disOrder) ? MaxSize * MaxSize - 1 : 0;

			while (true)
			{
				if (!((disOrder) ? j >= 0 : j < MaxSize * MaxSize)) break;

				if (!blockDict.ContainsKey(j) || blockDict[j] == null)
				{
					j = (disOrder) ? j - 1 : j + 1;

					continue;
				}

				var index = j;
				var block = blockDict[j];

				// 이동할 인덱스의 초기값은 현재 인덱스
				var moveTargetIndex = index;

				for (var move = 0; move < MaxSize; move++)
				{
					var targetIndex = index + GetMoveOffset(direction, move);

					if (direction == MoveDirection.Right && targetIndex % MaxSize < index % MaxSize) break;

					if (direction == MoveDirection.Left && targetIndex % MaxSize > index % MaxSize) break;

					// 이동할 곳이 바깥으로 빠져나감
					if (targetIndex < 0 || targetIndex >= MaxSize * MaxSize) break;

					var pairList = movePairList.FindAll((pair) => block != pair.Key && pair.Value == targetIndex);

					// 1. 그냥 이동 하면 되는 경우
					if (pairList.Count == 0)
					{
						moveTargetIndex = targetIndex;
					}
					// 2. 합쳐질수 있는 경우
					else if (pairList.Count == 1 && pairList[0].Key.Data.Num == block.Data.Num)
					{
						moveTargetIndex = targetIndex;
						break;
					}
					else
					{
						break;
					}
				}

				movePairList.Add(new KeyValuePair<Block, int>(block, moveTargetIndex));

				j = (disOrder) ? j - 1 : j + 1;
			}

			foreach (var movePair in movePairList)
			{
				moveDict.Add(movePair.Key, movePair.Value);
			}

			return moveDict;
		}

		/// <summary>
		/// 블락들을 이동시키고 합치기
		/// </summary>
		private IEnumerator MoveBlocksAndMerge(Dictionary<Block, int> moveDict)
		{
			var changeBlockDuration = 0.05f;
			var moveDuration = 0.25f;
			float maxDuration = 0;

			var mergeDict = new Dictionary<int, Block>();
			var destroyList = new List<Block>();
			var changeList = new List<Block>();
			var dontMoveCount = 0;
			var moveResCount = 0;

			UIBlocker.Instance.SetEnabled();

			// 1. 이동
			foreach (var moveData in moveDict)
			{
				var moveBlock = moveData.Key;
				var moveXIndex = moveData.Value % MaxSize;
				var moveYIndex = moveData.Value / MaxSize;

				if (moveBlock.Data.Index == moveData.Value)
				{
					if (!mergeDict.ContainsKey(moveData.Value))
					{
						mergeDict.Add(moveData.Value, moveData.Key);
					}

					dontMoveCount++;
					continue;
				}

				if (mergeDict.ContainsKey(moveData.Value))
				{
					destroyList.Add(mergeDict[moveData.Value]);
					changeList.Add(moveBlock);
				}
				else
				{
					mergeDict.Add(moveData.Value, moveData.Key);
				}

				var distance = Mathf.Sqrt(Mathf.Pow(GetXIndex(moveBlock.Data.Index) - moveXIndex, 2) + Mathf.Pow(GetYIndex(moveBlock.Data.Index) - moveYIndex, 2));
				var duration = moveDuration * (distance * 0.5f);

				if (maxDuration < duration)
				{
					maxDuration = duration;
				}

				StartCoroutine(moveBlock.MoveToBoard(moveData.Value, duration, () =>
				{
					moveResCount++;
				}));
			}

			// 아무것도 안움직인 경우
			if (dontMoveCount == moveDict.Count)
			{
				UIBlocker.Instance.SetDisabled();
				yield break;
			}

			yield return new WaitUntil(() => moveResCount == moveDict.Count - dontMoveCount);

			var resSum = 0;
			var isGameClear = false;
			// 2. 미리 없앨 리스트와 변화시킬 리스트로 합치기
			foreach (var block in changeList)
			{
				var toValue = block.Data.Num * 2;

				if (toValue == Constants.MaxValue[0])
				{
					isGameClear = true;
				}

				StartCoroutine(block.ChangeValue(toValue, changeBlockDuration, () =>
				{
					resSum++;
				}));
				Stages.Instance.AddScore(toValue);
			}

			foreach (var block in destroyList)
			{
				if (moveDict.ContainsKey(block))
				{
					moveDict.Remove(block);
				}

				block.gameObject.SetActive(false);
			}

			if (changeList.Count > 0)
			{
				yield return new WaitUntil(() => resSum == changeList.Count);
			}

			// 3. 2048 체크
			if (isGameClear)
			{
				UIBlocker.Instance.SetDisabled();
				Stages.Instance.ClearGame();
				yield break;
			}

			blockDict.Clear();

			foreach (var block in moveDict)
			{
				blockDict.Add(block.Value, block.Key);
			}

			// 4. 새로 블록 생성
			var isCreateBlock = CreateBlock();

			if (!isCreateBlock || CheckGameOver())
			{
				moveCoroutine = null;
				UIBlocker.Instance.SetDisabled();
				Stages.Instance.EndGame();
				yield break;
			}

			UIBlocker.Instance.SetDisabled();

			moveCoroutine = null;
		}

		private bool CheckGameOver()
		{
			if (!IsMax)
			{
				//Debug.Log($"max size is not {blockDict.Count}");
				return false;
			}

			foreach (var blockData in blockDict)
			{
				if (CheckSameValue(blockData.Key)) return false;
			}

			return true;
		}

		public bool IsMax => blockDict.Count == MaxSize * MaxSize;

		/// <summary>
		/// 특정 인덱스의 4방향에 같은 값이 있는지 체크
		/// </summary>
		private bool CheckSameValue(int index)
		{
			if (!blockDict.ContainsKey(index))
			{
				Debug.Log($"index {index} is not in block dict");
				return false;
			}

			var value = blockDict[index].Data.Num;

			var downIndex = index + GetMoveOffset(MoveDirection.Down, 1);
			var upIndex = index + GetMoveOffset(MoveDirection.Up, 1);
			var leftIndex = index + GetMoveOffset(MoveDirection.Left, 1);
			var rightIndex = index + GetMoveOffset(MoveDirection.Right, 1);

			if (blockDict.ContainsKey(downIndex) && blockDict[downIndex].Data.Num == value)
			{
				//Debug.Log($"Down {index} / {downIndex} / {blockDict[downIndex].Data.Num} / {value}");
				return true;
			}

			if (blockDict.ContainsKey(upIndex) && blockDict[upIndex].Data.Num == value)
			{
				//Debug.Log($"Up {index} / {upIndex} / {blockDict[upIndex].Data.Num} / {value}");
				return true;
			}

			if (blockDict.ContainsKey(leftIndex) && GetYIndex(leftIndex) == GetYIndex(index) &&
				blockDict[leftIndex].Data.Num == value)
			{
				//Debug.Log($"Left {index} / {leftIndex} / {blockDict[leftIndex].Data.Num} / {value}");
				return true;
			}

			if (blockDict.ContainsKey(rightIndex) && GetYIndex(rightIndex) == GetYIndex(index) &&
				blockDict[rightIndex].Data.Num == value)
			{
				//Debug.Log($"Right {index} / {rightIndex} / {blockDict[rightIndex].Data.Num} / {value}");
				return true;
			}

			//Debug.Log($"value {value} and index {index} is not checked");
			return false;
		}

		/// <summary>
		/// 해당 인덱스가 수치 범위 밖으로 벗어났는지 체크용
		/// </summary>
		private bool IsOutOfIndex(int index)
		{
			return index < 0 || index >= MaxSize * MaxSize;
		}

		private int GetYIndex(int index)
		{
			return index / MaxSize;
		}

		private int GetXIndex(int index)
		{
			return index % MaxSize;
		}

		/// <summary>
		/// x, y 인덱스(zero-based)로 찾은 board 위치
		/// </summary>
		public Vector3 GetBoardPosition(int index)
		{
			return boards[index].transform.position;
		}

		private int GetMoveOffset(MoveDirection direction, int moveDist)
		{
			switch (direction)
			{
				case MoveDirection.Right:
					return moveDist;
				case MoveDirection.Left:
					return -moveDist;
				case MoveDirection.Down:
					return moveDist * MaxSize;
				case MoveDirection.Up:
					return -moveDist * MaxSize;
				default:
					return 0;
			}
		}
	}
}
