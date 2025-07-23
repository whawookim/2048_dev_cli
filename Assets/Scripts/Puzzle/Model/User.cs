using System.Collections.Generic;

public class User : IPatchable
{
    /// <summary>
    /// 내 유저 정보
    /// </summary>
    public static User Me { get; private set; } = new User();

    public string UserId { get; set; }
    
    public string NickName { get; set; }
    
    public List<LoginType> UserIdpBindings { get; set; } = new List<LoginType>();
    
    public void ApplyPatch(IDictionary<string, object> jsonObject)
    {
        Network.PatchHelper.ApplyPatch(this, jsonObject);
    }
}
