using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Puzzle.UI
{
	public class BoardManager : MonoBehaviour
	{
		[SerializeField]
		private GridLayoutGroup grid;

		[SerializeField]
		private Transform blockTransform;

		private Coroutine moveCoroutine;

		private List<Board> boards;

		/// <summary>
		/// 현재 위치에 있는 Block 캐시
		/// </summary>
		private Dictionary<int, Block> blockDict;

		private ObjectPool<Board> objectPoolBoard;

		private ObjectPool<Block> objectPoolBlock;

		private Board originBoard;
		
		private Block originBlock;
		
		/// <summary>
		/// n*n 보드 사이즈
		/// </summary>
		private int maxSize;

		/// <summary>
		/// 블록 최대 수치
		/// </summary>
		private int maxNum;

		/// <summary>
		/// 블럭이 최대치로 가득 찼는지 체크
		/// </summary>
		private bool IsMax => blockDict.Count == maxSize * maxSize;

		/// <summary>
		/// 게임 클리어 체크 
		/// </summary>
		private bool isGameClear = false;

		private void Awake()
		{
			SubscribeEvent();
		}

		private void OnDestroy()
		{
			UnsubscribeEvent(true);
		}

		private void SubscribeEvent()
		{
			MessageSystem.Instance.Subscribe<BlockMoveEvent>(OnMoveBlockEvent);
		}

		private void UnsubscribeEvent(bool deleteKey = false)
		{
			MessageSystem.Instance.Unsubscribe<BlockMoveEvent>(OnMoveBlockEvent, deleteKey);
		}

		public void InitOriginResource(GameObject originBoardObj, GameObject originBlockObj)
		{
			originBoardObj.transform.SetParent(grid.transform);
			originBoard = originBoardObj.GetComponent<Board>();
			originBoard.transform.localScale = Vector3.one;
			
			originBlockObj.transform.SetParent(blockTransform);
			originBlock = originBlockObj.GetComponent<Block>();
			originBlock.transform.localScale = Vector3.one;
		}

		/// <summary>
		/// 랜덤한 블록을 생성할 인덱스를 반환
		/// </summary>
		private int GetRandomBlockIndex()
		{
			var candidates = new List<int>();

			for (var i = 0; i < maxSize * maxSize; i++)
			{
				if (blockDict.ContainsKey(i) && blockDict[i] != null) continue;

				candidates.Add(i);
			}

			if (candidates.Count == 0) return -1;

			return candidates[Random.Range(0, candidates.Count)];
		}

		public void Dispose()
		{
			blockDict?.Clear();
			blockDict = null;

			DisposeBoard();
			DisposeBlock();
		}

		private void DisposeBoard()
		{
			objectPoolBoard?.Dispose();
			objectPoolBoard = null;

			if (boards != null)
			{
				foreach (var board in boards)
				{
					DestroyImmediate(board.gameObject);
				}
				
				boards.Clear();
			}
			
			boards = null;
		}

		private void DisposeBlock()
		{
			objectPoolBlock?.Dispose();
			objectPoolBlock = null;

			if (blockDict != null)
			{
				foreach (var block in blockDict)
				{
					if (block.Value == null) continue;

					DestroyImmediate(block.Value.gameObject);
				}
				blockDict.Clear();
			}
			
			blockDict = null;
		}
		
		/// <summary>
		/// 블록들 가리기 (지우진 않음)
		/// </summary>
		public void HideBlocks()
		{
			//objectPoolBlock.Dispose();
			
			foreach (var block in blockDict)
			{
				if (block.Value == null) continue;

				block.Value.gameObject.SetActive(false);
			}

			blockDict.Clear();
		}

		/// <summary>
		/// 게임 시작시 처음 세팅
		/// TODO: 로딩이 들어가면 로딩 과정에 넣기
		/// </summary>
		public void Init(StageMode mode)
		{
			isGameClear = false;
			
			blockDict = new Dictionary<int, Block>();
			boards = new List<Board>();
			objectPoolBoard = new ObjectPool<Board>();
			objectPoolBlock = new ObjectPool<Block>();

			// 전체 보드 가로(혹은 세로)의 크기 결정
			maxSize = mode.GetBoardSize();
			maxNum = mode.GetBlockMaxNum();

			// 정사각형 블록, 보드 1개의 너비 (혹은 높이)
			var blockSize = mode.GetBlockSize();
			// 초기 블록, 보드의 사이즈 결정
			originBlock.SetSize(blockSize);
			originBoard.SetSize(blockSize);

			// 오브젝트 풀 초기 개수 세팅 (최대 블럭수만큼 미리 생성)
			objectPoolBlock.Init(originBlock, blockTransform, maxSize * maxSize);
			objectPoolBoard.Init(originBoard, grid.transform, maxSize * maxSize);

			for (var i = 0; i < maxSize * maxSize; i++)
			{
				var obj = objectPoolBoard.GetOrCreate();
				obj.Set($"board{i}");
				obj.gameObject.SetActive(true);
				boards.Add(obj);
			}

			originBoard.gameObject.SetActive(false);
			originBlock.gameObject.SetActive(false);

			// grid가 자동으로 보드를 정렬해서 배치를 해줌
			var gridSize = mode.GetGridSize();
			grid.cellSize = new Vector2(blockSize, blockSize);
			grid.spacing = new Vector2((gridSize - blockSize) * 0.5f, (gridSize - blockSize) * 0.5f);
			grid.constraintCount = maxSize;

			// NOTE: LayoutGroup을 사용한 경우 한 프레임 뒤에 자식 위치를 얻어올 수 있다고 함.
			Canvas.ForceUpdateCanvases();

			// 처음 배치되는 블록 생성
			CreateBlock();
		}

		/// <summary>
		/// 재시작용
		/// </summary>
		public void Reset()
		{
			isGameClear = false;

			// 처음 배치되는 블록 생성
			CreateBlock();
		}

		/// <summary>
		/// 블록이 생성될때 초기 수치 반환
		/// <remarks>게임의 밸런스적인 상수</remarks>
		/// </summary>
		private int GetInitBlockNum()
		{
			var initValues = Constants.InitValues;
			var initValuesProb = Constants.InitValuesProb;
			var roll = Random.Range(0.0f, 1.0f);
			var index = 0;

			foreach (var prob in initValuesProb)
			{
				if (roll > prob)
				{
					index++;
					roll -= prob;
				}
				else
				{
					break;
				}
			}

			return initValues[index];
		}

		/// <summary>
		/// 블록을 생성
		/// </summary>
		private void CreateBlock()
		{
			var blockIndex = GetRandomBlockIndex();

			if (IsOutOfIndex(blockIndex)) return;

			var block = objectPoolBlock.GetOrCreate();
			block.gameObject.SetActive(true);
			block.transform.localScale = Vector3.one;
			block.Init(GetInitBlockNum(), blockIndex);

			if (!blockDict.ContainsKey(blockIndex))
			{
				blockDict.Add(blockIndex, block);
			}
			else
			{
				blockDict[blockIndex] = block;
			}
		}

		private bool OnMoveBlockEvent(Events e)
		{
			if (e is BlockMoveEvent bme)
			{
				// 클리어시 움직이지 않게
				if (isGameClear) return false;
				if (moveCoroutine != null) return false;

				var direction = bme.Direction;

				if (direction == MoveDirection.None) return false;

				SetMoveBlockResultNew(direction);
				moveCoroutine = CoroutineManager.Instance.Run(MoveAndMergeBlocks());

				return true;
			}

			return false;
		}

		/// <summary>
		/// 블록이 이동한 결과 딕셔너리에 저장
		/// </summary>
		private void SetMoveBlockResultNew(MoveDirection direction)
		{
			var disOrder = (direction == MoveDirection.Down || direction == MoveDirection.Right);

			// move case
			// 1. 그냥 이동 하는 경우
			// 2. 이동 후 다른 블록이 합쳐지는 경우 (합쳐짐을 당하는 블록은 없애기 vs 합치기를 시도한 블록을 없애기)
			// 3. 2의 결과로 나온 블록은 다른 블록에 의해서 합쳐지지 않는다

			var tempDict = new Dictionary<int, Block>();

			for (var i = (disOrder) ? maxSize * maxSize - 1 : 0;
				(disOrder) ? i >= 0 : i < maxSize * maxSize;)
			{
				if (blockDict.ContainsKey(i))
				{
					var moveBlock = blockDict[i];
					var xIndex = GetXIndex(i);
					var yIndex = GetYIndex(i);
					// 이동 가능 거리
					var moveDist = 0;

					if (direction == MoveDirection.Left)
					{
						moveDist = xIndex;
					}
					else if (direction == MoveDirection.Right)
					{
						moveDist = (maxSize - 1) - xIndex;
					}
					else if (direction == MoveDirection.Down)
					{
						moveDist = (maxSize - 1) - yIndex;
					}
					else if (direction == MoveDirection.Up)
					{
						moveDist = yIndex;
					}

					if (moveDist >= 1)
					{
						var moveToIndex = i;
						var moveAndMerged = false;

						for (var j = 1; j <= moveDist; j++)
						{
							var toIndex = i + GetMoveOffset(direction, j);

							if (!tempDict.ContainsKey(toIndex))
							{
								moveToIndex = toIndex;
							}
							else if (tempDict[toIndex].Data.Num == moveBlock.Data.Num && !tempDict[toIndex].Data.IsMerged)
							{
								moveToIndex = toIndex;
								moveAndMerged = true;
								break;
							}
							else
							{
								break;
							}
						}

						if (moveToIndex != i)
						{
							if (moveAndMerged)
							{
								// 이동하고 사라짐
								moveBlock.MoveData = new BlockData()
								{
									Index = moveToIndex,
									Num = -1
								};
								//Debug.LogError($"index {moveToIndex} will be delete");
								var originIndex = tempDict[moveToIndex].Data.Index;
								var originValue = tempDict[moveToIndex].Data.Num;
								var toValue = originValue * 2;
								tempDict[moveToIndex].MoveData = new BlockData()
								{
									Index = moveToIndex,
									Num = toValue,
									WaitMergeBlock = moveBlock
								};
								tempDict[moveToIndex].Data = new BlockData()
								{
									Index = originIndex,
									Num = originValue,
									IsMerged = true
								};
							}
							else
							{
								moveBlock.MoveData = new BlockData()
								{
									Index = moveToIndex,
									Num = moveBlock.Data.Num
								};
								tempDict.Add(moveToIndex, moveBlock);
							}
						}
						else
						{
							// 제자리인 경우
							moveBlock.MoveData = null;
							tempDict.Add(i, moveBlock);
						}
					}
					else
					{
						moveBlock.MoveData = null;
						tempDict.Add(i, moveBlock);
					}
				}

				if (disOrder) i--;
				else i++;
			}
		}

		/// <summary>
		/// 블록 이동 결과에 따른 블록 이동 연출
		/// </summary>
		private IEnumerator MoveAndMergeBlocks()
		{
			UIBlocker.Instance.SetEnabled();

			var dontMoveCount = 0;
			var moveResCount = 0;
			var blockDictCount = blockDict.Count;

			foreach (var block in blockDict.Values)
			{
				if (block.MoveData == null)
				{
					dontMoveCount++;
				}
				else
				{
					CoroutineManager.Instance.Run(block.MoveAndChange(() =>
					{
						moveResCount++;

						if (block.Data.Num >= maxNum)
						{
							isGameClear = true;
						}
					}));
				}
			}

			if (blockDictCount == dontMoveCount)
			{
				moveCoroutine = null;
				UIBlocker.Instance.SetDisabled();
				yield break;
			}

			yield return new WaitUntil(() => blockDictCount == dontMoveCount + moveResCount);

			UIBlocker.Instance.SetDisabled();

			if (isGameClear)
			{
				moveCoroutine = null;
				Stages.Instance.ClearGame();
				yield break;
			}

			var moveDict = new Dictionary<int, Block>();

			foreach (var blockData in blockDict.Values)
			{
				if (!blockData.gameObject.activeSelf)
				{
					continue;
				}

				// if (moveDict.ContainsKey(blockData.Value.Data.Index))
				// {
				// 	//Debug.LogError("Test");
				// }

				moveDict.Add(blockData.Data.Index, blockData);
			}

			blockDict = moveDict;

			CreateBlock();

			moveCoroutine = null;

			if (CheckGameOver())
			{
				Stages.Instance.EndGame();
			}
		}

		/// <summary>
		/// 게임 오버인지 체크
		/// </summary>
		private bool CheckGameOver()
		{
			// 가득 차지 않은 경우면 무조건 게임오버 아님
			if (!IsMax) return false;

			// 가득찬 경우에는 합칠수 있는 블록 배치인지 체크
			foreach (var blockData in blockDict)
			{
				if (CheckSameValue(blockData.Key)) return false;
			}

			return true;
		}

		/// <summary>
		/// 특정 인덱스의 4방향에 같은 값이 있는지 체크
		/// </summary>
		private bool CheckSameValue(int index)
		{
			var value = blockDict[index].Data.Num;

			var downIndex = index + GetMoveOffset(MoveDirection.Down, 1);
			if (blockDict.ContainsKey(downIndex) && blockDict[downIndex].Data.Num == value)
			{
				//Debug.Log($"Down {index} / {downIndex} / {blockDict[downIndex].Data.Num} / {value}");
				return true;
			}

			var upIndex = index + GetMoveOffset(MoveDirection.Up, 1);
			if (blockDict.ContainsKey(upIndex) && blockDict[upIndex].Data.Num == value)
			{
				//Debug.Log($"Up {index} / {upIndex} / {blockDict[upIndex].Data.Num} / {value}");
				return true;
			}

			var leftIndex = index + GetMoveOffset(MoveDirection.Left, 1);
			if (blockDict.ContainsKey(leftIndex) && GetYIndex(leftIndex) == GetYIndex(index) &&
				blockDict[leftIndex].Data.Num == value)
			{
				//Debug.Log($"Left {index} / {leftIndex} / {blockDict[leftIndex].Data.Num} / {value}");
				return true;
			}

			var rightIndex = index + GetMoveOffset(MoveDirection.Right, 1);
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
			return index < 0 || index >= maxSize * maxSize;
		}

		private int GetYIndex(int index)
		{
			return index / maxSize;
		}

		private int GetXIndex(int index)
		{
			return index % maxSize;
		}

		/// <summary>
		/// x, y 인덱스(zero-based)로 찾은 board 위치
		/// </summary>
		public Vector3 GetBoardPosition(int index)
		{
			return boards[index].GetPosition();
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
					return moveDist * maxSize;
				case MoveDirection.Up:
					return -moveDist * maxSize;
				default:
					return 0;
			}
		}
	}
}
