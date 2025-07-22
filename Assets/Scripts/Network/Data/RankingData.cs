using System.Collections.Generic;

namespace Puzzle
{
    public struct RankingData
    {
        public int Rank;
        public string NickName;
        public int Score;
        public int ClearTime;
        public int MoveCount;

        public void SetRank(int rank)
        {
            Rank = rank;
        }

        public static RankingData Populate(Dictionary<string, object> data)
        {
            var resData = new RankingData();
            resData.NickName = data.Populate(nameof(NickName).ToCamelCase(), resData.NickName);
            resData.Score = data.Populate(nameof(Score).ToCamelCase(), resData.Score);
            resData.ClearTime = data.Populate(nameof(ClearTime).ToCamelCase(), resData.ClearTime);
            resData.MoveCount = data.Populate(nameof(MoveCount).ToCamelCase(), resData.MoveCount);
            
            return resData;
        }
    }
}
