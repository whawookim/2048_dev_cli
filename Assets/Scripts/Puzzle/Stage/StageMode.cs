namespace Puzzle
{
    public enum StageMode
    {
    	Stage3x3 = 0,
    	Stage4x4,
    	Staeg5x5
    }

    public static class StageModeExtension
    {
    	/// <summary>
    	/// 보드의 가로 세로 사이즈
    	/// </summary>
    	public static int GetBoardSize(this StageMode mode)
    	{
    		switch (mode)
    		{
    			case StageMode.Stage3x3:
    				return 3;
    			case StageMode.Stage4x4:
    				return 4;
    			case StageMode.Staeg5x5:
    				return 5;
    		}

    		return 4;
    	}

    	/// <summary>
    	/// 해당 스테이지의 한 블록의 최대 수치가 되는 값을 가져오기
    	/// <remarks>현재는 모든 스테이지 최대 사이즈가 고정</remarks>
    	/// </summary>
    	public static int GetBlockMaxNum(this StageMode mode)
    	{
    		var maxSizes = Constants.MaxValue;

    		switch (mode)
    		{
    			case StageMode.Stage3x3:
    				return maxSizes[0];
    			case StageMode.Stage4x4:
    				return maxSizes[1];
    			case StageMode.Staeg5x5:
    				return maxSizes[2];
    			default:
    				return maxSizes[1];
    		}
    	}

    	public static int GetBlockSize(this StageMode mode)
    	{
    		var modeIndex = (int) mode;
    		var blockSizes = Constants.BoardSizes;

    		if (blockSizes.Length <= modeIndex || modeIndex < 0) return 0;

    		return blockSizes[modeIndex];
    	}

    	public static int GetGridSize(this StageMode mode)
    	{
    		var modeIndex = (int) mode;
    		var gridSizes = Constants.GridSizes;

    		if (gridSizes.Length <= modeIndex || modeIndex < 0) return 0;

    		return gridSizes[modeIndex];
    	}
    }
}