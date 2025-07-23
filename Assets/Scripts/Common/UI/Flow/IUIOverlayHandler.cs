using System;
using System.Threading.Tasks;
using Puzzle.UI.Overlay;

namespace Puzzle.UI.Flow
{
    public interface IUIOverlayHandler
    {
        Task PushOverlayAsync(Type overlayType, object state = null);
        Task PopOverlayAsync();
        void ClearAllOverlays();
        IUIOverlay CurrentOverlay { get; }
    }
}
