using System.Collections.Generic;

public static partial class ApiConnection
{
    public static NetworkRequest Login(string loginType, string userId, string token)
    {
        var requestData = new Dictionary<string, object>
        {
            { "LoginType", loginType },
            { "UserId", userId },
            { "Token", token }
        };

        return new NetworkRequest(ApiClient.SendAsync("/api/login", requestData));
    }
    
    public static NetworkRequest Bind(string userId, LoginType loginType)
    {
        var requestData = new Dictionary<string, object>
        {
            { "UserId", userId },
            { "LoginType", loginType },
        };

        return new NetworkRequest(ApiClient.SendAsync("/api/bind", requestData));
    }
    
    public static NetworkRequest Unbind(string userId, LoginType loginType)
    {
        var requestData = new Dictionary<string, object>
        {
            { "UserId", userId },
            { "LoginType", loginType },
        };

        return new NetworkRequest(ApiClient.SendAsync("/api/unbind", requestData));
    }
}