using System;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Random = UnityEngine.Random;

namespace Puzzle.UI
{
	/// <summary>
	/// 보드판 UI
	/// </summary>
	public class BoardUI : MonoBehaviour
	{
		[SerializeField]
		private Transform boardTransform;

		[SerializeField]
		private Transform blockTransform;

		private Dictionary<Vector2Int, Board> _boardDict;

		/// <summary>
		/// 현재 위치에 있는 Block 캐시
		/// </summary>
		private Dictionary<Vector2Int, Block> _blockDict;
        
        /// <summary>
        /// 블록의 숫자가 저장되서 갱신되어야 하는 board
        /// </summary>
        private int[,] _board;

		private Board _originBoard;
		
		private Block _originBlock;
		
		/// <summary>
		/// n*n 보드 사이즈
		/// </summary>
		private int _maxSize;

		/// <summary>
		/// 블록 최대 수치
		/// </summary>
		private int _maxNum;
        
        private IPoolFactory<Block> _blockFactory;
        private IPoolFactory<Board> _boardFactory;
        
        private Stage.CommandExecutor _executor;

		public void SubscribeEvent()
		{
			MessageSystem.Instance.Subscribe<BlockMoveEvent>(OnMoveBlockEvent);
			MessageSystem.Instance.Subscribe<ResetBoardEvent>(OnResetBoard);
		}

        public void UnsubscribeEvent(bool deleteKey = false)
		{
			MessageSystem.Instance.Unsubscribe<BlockMoveEvent>(OnMoveBlockEvent, deleteKey);
			MessageSystem.Instance.Unsubscribe<ResetBoardEvent>(OnResetBoard);
		}

		public void InitOriginResource(GameObject originBoardObj, GameObject originBlockObj)
		{
			originBoardObj.transform.SetParent(boardTransform);
			_originBoard = originBoardObj.GetComponent<Board>();
			_originBoard.transform.localScale = Vector3.one;
			
			originBlockObj.transform.SetParent(blockTransform);
			_originBlock = originBlockObj.GetComponent<Block>();
			_originBlock.transform.localScale = Vector3.one;
		}
        
        /// <summary>
        /// 랜덤한 블록을 생성할 위치를 반환
        /// </summary>
        private Vector2Int? GetRandomBlockPos()
        {
            var candidates = new List<Vector2Int>();
            var tempVal = new Vector2Int();

            for (int y = 0; y < _maxSize; y++)
            {
                for (int x = 0; x < _maxSize; x++)
                {
                    tempVal.Set(x, y);

                    if (_blockDict.ContainsKey(tempVal) && _blockDict[tempVal] != null) continue;
                    
                    candidates.Add(new Vector2Int(tempVal.x, tempVal.y));
                }
            }
            
            if (candidates.Count == 0) return null;

            return candidates[Random.Range(0, candidates.Count)];
        }

		public void Dispose()
		{
			DisposeBoard();
			DisposeBlock();

            _executor.Clear();
            _executor = null;
        }

		private void DisposeBoard()
        {
            _boardFactory.Dispose();

			if (_boardDict != null)
			{
				foreach (var tempBoard in _boardDict)
				{
					DestroyImmediate(tempBoard.Value.gameObject);
				}
				
                _boardDict.Clear();
			}
			
            _boardDict = null;
            _board = null;
        }

		private void DisposeBlock()
		{
            _blockFactory.Dispose();

			if (_blockDict != null)
			{
				foreach (var block in _blockDict)
				{
					if (block.Value == null) continue;

					DestroyImmediate(block.Value.gameObject);
				}
				_blockDict.Clear();
			}
			
			_blockDict = null;
		}
		
		/// <summary>
		/// 블록들 가리기 (지우진 않음)
		/// </summary>
		public void HideBlocks()
		{
			//objectPoolBlock.Dispose();
			
			foreach (var block in _blockDict)
			{
				if (block.Value == null) continue;

				block.Value.gameObject.SetActive(false);
			}

			_blockDict.Clear();
		}

		/// <summary>
		/// 게임 시작시 처음 세팅
		/// TODO: 로딩이 들어가면 로딩 과정에 넣기
		/// </summary>
		public void Init(StageMode mode)
        {
            _executor = new Stage.CommandExecutor();
            
			_blockDict = new Dictionary<Vector2Int, Block>();
            _boardDict = new Dictionary<Vector2Int, Board>();

			// 전체 보드 가로(혹은 세로)의 크기 결정
			_maxSize = mode.GetBoardSize();
			_maxNum = mode.GetBlockMaxNum();

            _board = new int[_maxSize, _maxSize];

			// 정사각형 블록, 보드 1개의 너비 (혹은 높이)
			var blockSize = mode.GetBlockSize();
			// 초기 블록, 보드의 사이즈 결정
			_originBlock.SetSize(blockSize);
			_originBoard.SetSize(blockSize);
            
            // 오프젝트풀 팩톻리로 오브젝트 풀 초기 개수 세팅 (최대 블럭수만큼 미리 생성)
            int initBlockCount = _maxSize * _maxSize;
            int initBoardCount = _maxSize * _maxSize;
            
            _blockFactory = new ObjectPoolFactory<Block>(
                _originBlock.gameObject,
                blockTransform,
                initBlockCount,
                (block) => {
                    block.gameObject.SetActive(true);
                    block.transform.localScale = Vector3.one;
                    block.Init(GetInitBlockNum());
                });
            
            _boardFactory = new ObjectPoolFactory<Board>(
                _originBoard.gameObject,
                boardTransform,
                initBoardCount,
                (board) =>
                {
                    board.gameObject.SetActive(true);
                    board.transform.localScale = Vector3.one;
                });
            // board 초기화는 Create() 이후 호출부에서 name, size, pos 직접 전달
            
            _originBoard.gameObject.SetActive(false);
            _originBlock.gameObject.SetActive(false);

            InitBoard(mode, blockSize);

			// 처음 배치되는 블록 생성
			CreateBlock();
            UpdateBoardState();
		}

        private void InitBoard(StageMode mode, int blockSize)
        {
            // GridLayoutGroup 썼다가 좌상단으로 정렬이 되는 문제가 있어서 그냥 직접 구현함
            var gridSize = mode.GetGridSize();
            int spacing = gridSize - blockSize;
            var startPos = new Vector2(-(_maxSize * gridSize * 0.5f) + gridSize * 0.5f,
                _maxSize * gridSize * 0.5f - gridSize * 0.5f);
            
            for (int y = 0; y < _maxSize; y++)
            {
                for (int x = 0; x < _maxSize; x++)
                {
                    var obj = _boardFactory.Create();
                    obj.SetName($"board[{x},{y}]");
                    obj.gameObject.SetActive(true);
                    obj.SetPosition(startPos + new Vector2((blockSize + spacing) * x, -(blockSize + spacing) * y));
                    _boardDict.Add(new Vector2Int(x, y), obj);
                }
            }
        }

		/// <summary>
		/// 재시작용
		/// </summary>
		public void Reset()
		{
            HideBlocks();
			// 처음 배치되는 블록 생성
			CreateBlock();
            UpdateBoardState();
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
			var randomBlockPos = GetRandomBlockPos();

            if (randomBlockPos == null)
            {
                MyDebug.LogWarning("CreateBlock Fail: randomBlockPos is null");
                return;
            }

            var pos = randomBlockPos.Value;
			var block = _blockFactory.Create();
			_blockDict[pos] = block;
            var worldBoardPos = GetBoardPosition(pos);
            block.SetPosition(worldBoardPos);
        }

		private bool OnMoveBlockEvent(Events e)
		{
			if (e is BlockMoveEvent bme)
			{
				var direction = bme.Direction;

				if (direction == MoveDirection.None) return false;
                
                _ = OnBlockMoveEvent(direction.GetMoveVec());

				return true;
			}

			return false;
		}
        
        public async Task OnBlockMoveEvent(Vector2Int direction)
        {
            // 일시정지, 클리어, 실패 등등의 이동 불가 상황
            if (StageManager.Instance.StatusController.CurrentState != Stage.StageState.Playing)
                return;
            
            Stage.StageLogic.GenerateMoveCommands(_blockDict, direction, _board, _executor);

            if (_executor.Count <= 0) return;
            
            UIBlocker.Instance.SetEnabled();
                
            await _executor.ExecuteAllAsync();

            UpdateBoardState();

            if (CheckGameClear())
            {
                UIBlocker.Instance.SetDisabled();

                StageManager.Instance.StatusController.ClearGame();
            }
            else
            {
                var randomBlockPos = GetRandomBlockPos();

                if (randomBlockPos == null)
                {
                    MyDebug.LogWarning("CreateBlock Fail: randomBlockPos is null");
                    return;
                }

                var pos = randomBlockPos.Value;
                var block = _blockFactory.Create();
                block.gameObject.SetActive(false);
                block.transform.localScale = Vector3.one;
                block.Init(GetInitBlockNum());
                _blockDict[pos] = block;
                var worldBoardPos = GetBoardPosition(pos);
                _executor.EnqueueGroup(new List<Stage.IBlockCommand>()
                {
                    new Stage.SpawnBlockCommand(block, worldBoardPos)
                });
                
                await _executor.ExecuteAllAsync();

                UIBlocker.Instance.SetDisabled();
                
                UpdateBoardState();

                if (CheckGameOver())
                {
                    StageManager.Instance.StatusController.GameFail();
                }
            }
        }

        /// <summary>
        /// 최신화된 Block 데이터로 Board 갱신
        /// </summary>
        private void UpdateBoardState()
        {
            var tempVal = new Vector2Int();
            
            for (int x = 0; x < _maxSize; x++)
            {
                for (int y = 0; y < _maxSize; y++)
                {
                    tempVal.Set(x, y);
                    if (_blockDict.TryGetValue(tempVal, out var block))
                    {
                        _board[x, y] = block.Number;
                    }
                    else
                    {
                        _board[x, y] = 0;
                    }
                }
            }
        }

        /// <summary>
        /// 게임 클리어 상태인지 체크
        /// </summary>
        private bool CheckGameClear()
        {
            foreach (var val in _board)
            {
                if (val == _maxNum)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 보드의 블록 숫자가 하나라도 0인 경우 false
        /// </summary>
        /// <remarks>가득 찼는지 체크용</remarks>
        private bool CheckFullOccupied()
        {
            foreach (var val in _board)
            {
                if (val == 0)
                {
                    return false;
                }
            }

            return true;
        }

		/// <summary>
		/// 게임 오버인지 체크
		/// </summary>
		private bool CheckGameOver()
		{
			// 가득 차지 않은 경우면 무조건 게임오버 아님
			if (!CheckFullOccupied()) return false;
            
			// 가득찬 경우에는 합칠수 있는 블록 배치인지 체크
			foreach (var blockData in _blockDict)
			{
				if (CheckSameValue(blockData.Key)) return false;
			}

			return true;
		}

		/// <summary>
		/// 특정 인덱스의 4방향에 같은 값이 있는지 체크
		/// </summary>
		private bool CheckSameValue(Vector2Int pos)
		{
			var value = _board[pos.x, pos.y];
            var directions = (MoveDirection[])Enum.GetValues(typeof(MoveDirection));

            foreach (var direction in directions)
            {
                if (direction == MoveDirection.None) continue;
                
                var calcPos = pos + direction.GetMoveVec();

                if (!Stage.StageLogic.IsInBounds(calcPos, _maxSize, _maxSize)) continue;
                
                if (_board[calcPos.x, calcPos.y] == value)
                {
                    return true;
                }
            }

			//MyDebug.Log($"value {value} and index {index} is not checked");
			return false;
		}

		/// <summary>
		/// x, y 인덱스(zero-based)로 찾은 board 위치
		/// </summary>
		public Vector3 GetBoardPosition(Vector2Int pos)
		{
			return _boardDict[pos].GetPosition();
		}
		
		private bool OnResetBoard(Events e)
		{
			if (e is ResetBoardEvent rbe)
			{
				Reset();
				return true;
			}

			return false;
		}
	}
}
