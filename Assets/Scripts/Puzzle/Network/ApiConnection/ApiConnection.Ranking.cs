using System.Collections.Generic;

public static partial class ApiConnection
{
    /// <summary>
    /// 스테이지 End API
    /// </summary>
    public static NetworkRequest GetRanking(Puzzle.StageMode stageMode, Puzzle.RankingMode rankingMode)
    {
        var requestData = new Dictionary<string, object>
        {
            { "Mode", stageMode.ToCamelCase() },
            { "Type", rankingMode.ToCamelCase() },
        };

        return new NetworkRequest(ApiClient.SendAsync("/api/ranking", requestData, 
            UnityEngine.Networking.UnityWebRequest.kHttpVerbGET));
    }
}
