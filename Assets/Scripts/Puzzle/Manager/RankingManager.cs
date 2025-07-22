using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Puzzle
{
    /// <summary>
    /// 랭킹을 매기는 모드
    /// </summary>
    public enum RankingMode
    {
        Score,
        ClearTime,
        MoveCount
    }
    
    public class RankingManager
    {
        private static RankingManager _instance;
        public static RankingManager Instance => _instance ??= new RankingManager();

        private Dictionary<(StageMode, RankingMode), List<RankingData>> cachedRankingDict =
            new Dictionary<(StageMode, RankingMode), List<RankingData>>();

        public async Task<List<RankingData>> GetRankingData(StageMode stageMode, RankingMode rankingMode,
            bool forceRefresh = false)
        {
            if (!forceRefresh && cachedRankingDict.TryGetValue((stageMode, rankingMode), out var list))
            {
                return list;
            }
            
            try
            {
                // 서버에 Ranking 요청
                var request = ApiConnection.GetRanking(stageMode, rankingMode);
                while (!request.IsDone)
                    await Task.Yield();

                if (request.Ok && request.Result != null &&
                    request.Result.TryGetValue(NetworkRequest.RESULT_RESOURCE_KEY, out var resObj) &&
                    resObj is JArray resList)
                {
                    var rankingDataList = new List<RankingData>();

                    for (int i = 0; i < resList.Count; i++)
                    {
                        var dataDict = (resList[i] as JObject)?.ToObject<Dictionary<string, object>>();
                        if (dataDict == null)
                            continue;
                        
                        var data = RankingData.Populate(dataDict);
                        data.SetRank(i + 1);
                        rankingDataList.Add(data);
                    }
                    
                    cachedRankingDict[(stageMode, rankingMode)] = rankingDataList;

                    return rankingDataList;
                }
                else
                {
                    MyDebug.LogError(
                        $"Get Ranking Request Failed: {request.Response?.error?.code}, message {request.Response?.error?.message}");
                    return null;
                }
            }
            catch (System.Exception ex)
            {
                MyDebug.LogError($"Get Ranking Failed: {ex.Message}");
                return null;
            }
        }
    }
}