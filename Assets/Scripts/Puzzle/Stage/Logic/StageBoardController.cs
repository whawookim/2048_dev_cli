using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Puzzle.Stage
{
    /// <summary>
    /// 스테이지 내에서 보드와 블록 생성/이동/삭제 등 전체 게임 흐름을 관리하는 컨트롤러
    /// </summary>
    public class StageBoardController
    {
        /// <summary>현재 스테이지 모드 (블록 사이즈 등 규격 정보 포함)</summary>
        private readonly StageMode _mode;
        
        /// <summary>보드 상태 데이터를 관리하는 모델</summary>
        private readonly StageBoardModel _model;
        
        /// <summary>블록 및 보드를 렌더링하는 UI 컴포넌트</summary>
        private readonly UI.BoardUI _view;
        
        /// <summary>Block 풀링 팩토리</summary>
        private readonly IPoolFactory<UI.Block> _blockFactory;
        
        /// <summary>Board 풀링 팩토리</summary>
        private readonly IPoolFactory<UI.Board> _boardFactory;
        
        /// <summary>보드 위치에 배치된 블록들을 관리하는 딕셔너리</summary>
        private Dictionary<Vector2Int, UI.Block> _blockDict;
        
        /// <summary>보드 위치에 배치된 보드 오브젝트</summary>
        private Dictionary<Vector2Int, UI.Board> _boardDict;
        
        /// <summary>블록 이동, 병합 등 커맨드 애니메이션 처리용 큐</summary>
        private CommandExecutor _executor;

        public StageBoardController(StageMode mode, StageBoardModel model, UI.BoardUI view,
            IPoolFactory<UI.Block> blockFactory, IPoolFactory<UI.Board> boardFactory)
        {
            _mode = mode;
            _model = model;
            _view = view;
            _blockFactory = blockFactory;
            _boardFactory = boardFactory;

            _blockDict = new();
            _boardDict = new();
            _executor = new();
        }

        /// <summary>
        /// 보드 타일 생성 및 초기 블록 배치
        /// </summary>
        public void Initialize()
        {
            int boardSize = _mode.GetBlockSize();
            int spacing = _mode.GetGridSize() - boardSize;

            var startPos = new Vector2(-(_model.MaxSize * _mode.GetGridSize() * 0.5f) + _mode.GetGridSize() * 0.5f,
                                       _model.MaxSize * _mode.GetGridSize() * 0.5f - _mode.GetGridSize() * 0.5f);

            for (int y = 0; y < _model.MaxSize; y++)
            {
                for (int x = 0; x < _model.MaxSize; x++)
                {
                    var pos = new Vector2Int(x, y);
                    var board = _boardFactory.Create();
                    board.SetName($"board[{x},{y}]");
                    board.SetSize(boardSize);
                    board.Show();
                    board.SetPosition(startPos + new Vector2((boardSize + spacing) * x, -(boardSize + spacing) * y));
                    _boardDict[pos] = board;
                }
            }

            SpawnInitialBlock();
        }

        /// <summary>
        /// 게임 시작 시 최초 블록 생성
        /// </summary>
        public void SpawnInitialBlock()
        {
            SpawnRandomBlock();
            _model.UpdateBoardState(_blockDict);
        }
        
        /// <summary>
        /// 게임 재시작 시 블록 전부 제거 후 초기화
        /// </summary>
        public void Reset()
        {
            foreach (var block in _blockDict.Values)
                _blockFactory.Release(block);
            _blockDict.Clear();

            SpawnInitialBlock();
        }

        /// <summary>
        /// 메시지 시스템에 블록 이동 / 보드 리셋 이벤트 구독
        /// </summary>
        public void SubscribeEvents()
        {
            MessageSystem.Instance.Subscribe<BlockMoveEvent>(OnMoveBlockEvent);
            MessageSystem.Instance.Subscribe<ResetBoardEvent>(OnResetBoard);
        }

        /// <summary>
        /// 메시지 시스템 구독 해제
        /// </summary>
        public void UnsubscribeEvents(bool deleteKey = false)
        {
            MessageSystem.Instance.Unsubscribe<BlockMoveEvent>(OnMoveBlockEvent, deleteKey);
            MessageSystem.Instance.Unsubscribe<ResetBoardEvent>(OnResetBoard);
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
        
        private bool OnMoveBlockEvent(Events e)
        {
            if (e is BlockMoveEvent bme)
            {
                var direction = bme.Direction;

                if (direction == MoveDirection.None) return false;
                
                _ = OnBlockMove(direction.GetMoveVec());

                return true;
            }

            return false;
        }

        /// <summary>
        /// 블록 이동 처리 및 병합, 승리/패배 판단 포함
        /// </summary>
        public async Task OnBlockMove(Vector2Int direction)
        {
            if (StageManager.Instance.StatusController.CurrentState != StageState.Playing)
                return;

            StageLogic.GenerateMoveCommands(_blockDict, direction, _model.Board, _executor);
            if (_executor.Count <= 0) return;

            UI.UIBlocker.Instance.SetEnabled();
            await _executor.ExecuteAllAsync();

            _model.UpdateBoardState(_blockDict);

            if (_model.IsGameClear())
            {
                UI.UIBlocker.Instance.SetDisabled();
                StageManager.Instance.StatusController.ClearGame();
                return;
            }

            SpawnRandomBlock(true);
            await _executor.ExecuteAllAsync();

            UI.UIBlocker.Instance.SetDisabled();
            _model.UpdateBoardState(_blockDict);

            if (_model.IsGameOver(_blockDict))
            {
                StageManager.Instance.StatusController.GameFail();
            }
        }

        /// <summary>
        /// 빈 공간 중 하나를 골라 블록 생성. queued 시 애니메이션 큐에 등록
        /// </summary>
        private void SpawnRandomBlock(bool queued = false)
        {
            var candidates = new List<Vector2Int>();
            for (int y = 0; y < _model.MaxSize; y++)
            for (int x = 0; x < _model.MaxSize; x++)
            {
                var pos = new Vector2Int(x, y);
                if (_blockDict.ContainsKey(pos)) continue;
                candidates.Add(pos);
            }

            if (candidates.Count == 0) return;
            var selected = candidates[Random.Range(0, candidates.Count)];
            var block = _blockFactory.Create();
            block.Init(Constants.GetRandomInitBlockValue());
            block.transform.localScale = Vector3.one;
            block.SetSize(_mode.GetBlockSize());

            if (queued) block.Hide();
            else block.Show();
            
            block.SetPosition(GetBoardPosition(selected));
            _blockDict[selected] = block;

            if (queued)
            {
                _executor.EnqueueGroup(new List<IBlockCommand>()
                {
                    new SpawnBlockCommand(block, GetBoardPosition(selected))
                });
            }
        }
        
        /// <summary>
        /// 보드 좌표계 기준 UI 위치 반환
        /// </summary>
        public Vector3 GetBoardPosition(Vector2Int pos)
        {
            return _boardDict[pos].GetPosition();
        }

        public Dictionary<Vector2Int, UI.Block> GetBlockDict() => _blockDict;

        public Dictionary<Vector2Int, UI.Board> GetBoardDict() => _boardDict;
        
        /// <summary>
        /// 씬 종료 시 풀 반환 및 오브젝트 제거
        /// </summary>
        public void Dispose()
        {
            _blockFactory.Dispose();
            _boardFactory.Dispose();
            
            foreach (var block in _blockDict.Values)
                Object.Destroy(block.gameObject);
            _blockDict.Clear();
            _blockDict = null;

            foreach (var board in _boardDict.Values)
                Object.Destroy(board.gameObject);
            _boardDict.Clear();
            _boardDict = null;

            _executor.Clear();
            _executor = null;
        }
    }
}
