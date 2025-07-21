using System.Collections.Generic;

public static partial class ApiConnection
{
    /// <summary>
    /// 스테이지 End API
    /// </summary>
    public static NetworkRequest EndStage(User user, Puzzle.StageMode stageMode, int score = -1, int clearTime = -1, int moveCount = -1)
    {
        var requestData = new Dictionary<string, object>
        {
            { "UserId", user.UserId },
            { "NickName", user.NickName },
            { "Mode", stageMode.ToCamelCase() },
            { "Score", score },
            { "ClearTime", clearTime },
            { "MoveCount", moveCount },
        };

        return new NetworkRequest(ApiClient.SendAsync("/api/stage/end", requestData));
    }
}
