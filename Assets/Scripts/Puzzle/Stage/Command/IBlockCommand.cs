using System.Threading.Tasks;
using System;

namespace Puzzle.Stage
{
    /// <summary>
    /// 퍼즐 블록에 대한 커맨드 패턴 인터페이스.
    /// 이동, 병합, 생성 등 다양한 행동을 명령 객체로 캡슐화한다.
    /// </summary>
    public interface IBlockCommand
    {
        Task ExecuteAsync();
        Task UndoAsync(); // 선택 사항
        Action ExecuteAction { get; set; }
    }
}
