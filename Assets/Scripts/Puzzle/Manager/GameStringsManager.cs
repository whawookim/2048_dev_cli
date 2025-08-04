namespace Puzzle
{
    public enum GameStringTable
    {
        GameStrings
    }
    
    public static class GameStringsManager
    {
        public static GameStringTable DefaultTableEnum => GameStringTable.GameStrings;
        
        public static string DefaultTable => DefaultTableEnum.ToString();
    }
}
