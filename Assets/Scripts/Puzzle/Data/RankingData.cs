namespace Puzzle.Data
{
    public struct RankingData
    {
        public int Rank;
        public string Nickname;
        public int Score;

        public RankingData(int rank, string nickname, int score)
        {
            Rank = rank;
            Nickname = nickname;
            Score = score;
        }
    }
}
