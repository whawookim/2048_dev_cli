using System.Collections.Generic;

public partial class ApiConnection
{
    public static NetworkRequest ChangeNickname(string userId, string newNickName)
    {
        var requestData = new Dictionary<string, object>
        {
            { "UserId", userId },
            { "NewNickname", newNickName },
        };

        return new NetworkRequest(ApiClient.SendAsync("/api/change-nickname", requestData));
    }
}
