using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

/// <summary>
/// TODO: 블록 이동시 순서대로 하기 때문에 어떤걸 
/// </summary>
public class Board : MonoBehaviour
{
	public static Board Instance { get; set; }
	
	// TODO: MaxSize 변경 적용 가능하게
	private const int MaxSize = 4;
	
	[SerializeField]
	private UIWidget[] boards;

	[SerializeField]
	private Block originBlock;

	private Coroutine moveCoroutine;

	/// <summary>
	/// 현재 위치에 있는 Block 캐시
	/// </summary>
	private Dictionary<int, Block> blockDict = new Dictionary<int, Block>();
	
	void Awake()
	{
		originBlock.gameObject.SetActive(false);
		
		Instance = this;
	}

	private void OnDestroy()
	{
		Instance = null;
	}

	private void OnEnable()
	{
		CreateBlock();
		
		StageUi.Instance.SetGameStatus(StageUi.GameState.Start);
		StageUi.Instance.SetGameScore(0);
		
		MessageSystem.Instance.Subscribe(OnMoveTest);
	}

	private void OnDisable()
	{
		MessageSystem.Instance.Unsubscribe(OnMoveTest);
	}

	/// <summary>
	/// x, y 인덱스(zero-based)로 찾은 board 위치
	/// </summary>
	public Vector3 GetBoardPosition(int xIndex, int yIndex)
	{
		return boards[yIndex * MaxSize + xIndex].cachedTransform.position;
	}

	public int GetRandomBlankIndex()
	{
		List<int> candidates = new List<int>();

		for (var i = 0; i < MaxSize * MaxSize; i++)
		{
			if (blockDict.ContainsKey(i) && blockDict[i] != null) continue;
			
			candidates.Add(i);
		}

		if (candidates.Count == 0) return -1;
		
		return candidates[Random.Range(0, candidates.Count)];
	}

	public void CreateBlock()
	{
		var blockIndex = GetRandomBlankIndex();

		// TODO: 게임 종료
		if (blockIndex == -1)
		{
			Debug.LogError("Game Over !!!!!!!!!!!!!!!!");
			return;
		}
		
		var blockObj = Instantiate(originBlock.gameObject);
		blockObj.transform.parent = originBlock.transform.parent;

		blockObj.SetActive(true);

		blockObj.transform.localScale = Vector3.one;
		
		var block = blockObj.GetComponent<Block>();

		// 2 or 4가 나오게 설정하기 위함
		var rangeLimit = 0.95f;
		var range = Random.Range(0.0f, 1.0f);

		var initVal = (range > rangeLimit) ? 4 : 2;
		
		block.Init(initVal, blockIndex % MaxSize, blockIndex / MaxSize);

		if (!blockDict.ContainsKey(blockIndex))
		{
			blockDict.Add(blockIndex, block);
		}
		else
		{
			blockDict[blockIndex] = block;
		}
	}

	public void DestroyBlock(Block targetBlock)
	{
		for (var i = 0; i < blockDict.Count; i++)
		{
			if (blockDict[i] != targetBlock) continue;
			
			blockDict[i] = null;
			Destroy(targetBlock.gameObject);
		}
	}

	private bool OnMoveTest(Events e)
	{
		if (e is BlockMoveEvent bme)
		{
			if (moveCoroutine != null) return true;
			
			var direction = bme.Direction;
			Debug.LogWarning(bme.Direction);

			if (direction == MoveDirection.None) return true;
	
			var moveResult = GetMoveBlockResult(direction);

			moveCoroutine = StartCoroutine(MoveBlocksAndMerge(moveResult));
		}

		return false;
	}

	/// <summary>
	/// 움직이게 된 후 결과 딕셔너리 저장
	/// </summary>
	private Dictionary<Block, int> GetMoveBlockResult(MoveDirection direction)
	{
		var moveDict = new Dictionary<Block, int>();
		
		List<KeyValuePair<Block, int>> movePairList = new List<KeyValuePair<Block, int>>();

		var disOrder = (direction == MoveDirection.Down || direction == MoveDirection.Right);

		if (!disOrder)
		{
			for (int i = 0; i < MaxSize * MaxSize; i++)
			{
				if (!blockDict.ContainsKey(i) || blockDict[i] == null) continue;

				var index = i;
				var block = blockDict[i];
				
				// 이동할 인덱스의 초기값은 현재 인덱스
				int moveTargetIndex = index;

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
						continue;
					}
					// 2. 합쳐질수 있는 경우
					else if (pairList.Count == 1 && pairList[0].Key.Value == block.Value)
					{
						moveTargetIndex = targetIndex;
						break;
					}
					else
					{
						break;
					}
				}
			
				// 같으면 이동할 필요 없음
				//if (moveTargetIndex == index) continue;
			
				movePairList.Add(new KeyValuePair<Block, int>(block, moveTargetIndex));
			}
		}
		else
		{
			for (int i = MaxSize * MaxSize - 1; i >= 0; i--)
			{
				if (!blockDict.ContainsKey(i) || blockDict[i] == null) continue;
				
				var index = i;
				var block = blockDict[i];
				
				// 이동할 인덱스의 초기값은 현재 인덱스
				int moveTargetIndex = index;

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
						continue;
					}
					// 2. 합쳐질수 있는 경우
					else if (pairList.Count == 1 && pairList[0].Key.Value == block.Value)
					{
						moveTargetIndex = targetIndex;
						break;
					}
					else
					{
						break;
					}
				}
			
				// 같으면 이동할 필요 없음
				//if (moveTargetIndex == index) continue;
			
				movePairList.Add(new KeyValuePair<Block, int>(block, moveTargetIndex));
			}
		}

		if (movePairList.Count > 0)
		{
			foreach (var movePair in movePairList)
			{
				moveDict.Add(movePair.Key, movePair.Value);
			}
		}
		
		return moveDict;
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
		
		UIBlocker.Instance.SetEnabled();
		
		// 1. 이동
		foreach (var moveData in moveDict)
		{
			var moveBlock = moveData.Key;
			var moveXIndex = moveData.Value % MaxSize;
			var moveYIndex = moveData.Value / MaxSize;

			if (moveBlock.XIndex == moveXIndex && moveBlock.YIndex == moveYIndex)
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

			var distance = Mathf.Sqrt(Mathf.Pow(moveBlock.XIndex - moveXIndex, 2) + Mathf.Pow(moveBlock.YIndex - moveYIndex, 2));
			
			var duration = moveDuration * (distance * 0.5f);

			if (maxDuration < duration)
			{
				maxDuration = duration;
			}
			
			StartCoroutine(moveBlock.MoveToBoard(moveXIndex, moveYIndex, duration));
		}

		// 아무것도 안움직인 경우
		if (dontMoveCount == moveDict.Count)
		{
			UIBlocker.Instance.SetDisabled();
			yield break;
		}
		
		Debug.LogError("MaxDuration " + maxDuration);

		yield return new WaitForSeconds(maxDuration);

		var isGameClear = false;
		// 2. 미리 없앨 리스트와 변화시킬 리스트로 합치기
		foreach (var block in changeList)
		{
			var toValue = block.Value * 2;

			if (toValue == 2048)
			{
				isGameClear = true;
			}
			
			StartCoroutine(block.ChangeValue(toValue, changeBlockDuration));
		}

		foreach (var block in destroyList)
		{
			Destroy(block.gameObject);
		}

		if (changeList.Count > 0)
		{
			Debug.LogError("ChangeDuration " + changeBlockDuration * 0.2f);
			yield return new WaitForSeconds(changeBlockDuration * 0.2f);
		}

		// 3. 2048 체크
		if (isGameClear)
		{
			StageUi.Instance.SetGameStatus(StageUi.GameState.Clear);
			UIBlocker.Instance.SetDisabled();
			Debug.LogError("Game Clear !!!!!!");
			yield break;
		}
		
		yield return new WaitForSeconds(changeBlockDuration * 0.8f);

		blockDict.Clear();

		foreach (var block in moveDict)
		{
			if (block.Key == null) continue;
			
			if (block.Key.gameObject == null) continue;
			
			blockDict.Add(block.Value, block.Key);
		}

		// 4. 새로 블록 생성
		CreateBlock();

		moveCoroutine = null;
		
		if (CheckGameOver())
		{
			StageUi.Instance.SetGameStatus(StageUi.GameState.Fail);
			UIBlocker.Instance.SetDisabled();
			Debug.LogError("Game Over !!!!!!");
			yield break;
		}

		UIBlocker.Instance.SetDisabled();
	}

	private bool CheckGameOver()
	{
		var isMax = blockDict.Count == MaxSize * MaxSize;

		if (!isMax) return false;

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
		if (!blockDict.ContainsKey(index)) return false;

		var value = blockDict[index].Value;

		var downIndex = index + GetMoveOffset(MoveDirection.Down, 1);
		var upIndex = index + GetMoveOffset(MoveDirection.Up, 1);
		var leftIndex = index + GetMoveOffset(MoveDirection.Left, 1);
		var rightIndex = index + GetMoveOffset(MoveDirection.Right, 1);

		if (blockDict.ContainsKey(downIndex) && blockDict[downIndex].Value == value)
		{
			Debug.LogError("Down " + index + " / " + downIndex);
			return true;
		}
		
		if (blockDict.ContainsKey(upIndex) && blockDict[upIndex].Value == value)
		{
			Debug.LogError("Up " + index + " / " + upIndex);
			return true;
		}
		
		if (blockDict.ContainsKey(leftIndex) && GetYIndex(leftIndex) == GetYIndex(index) &&
			blockDict[leftIndex].Value == value)
		{
			Debug.LogError("Left " + index + " / " + leftIndex);
			return true;
		}
		
		if (blockDict.ContainsKey(rightIndex) && GetYIndex(rightIndex) == GetYIndex(index) &&
			blockDict[rightIndex].Value == value)
		{
			Debug.LogError("Right " + index + " / " + rightIndex);
			return true;
		}

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
}
