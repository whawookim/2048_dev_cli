using System.Collections.Generic;

public class User : Network.IPatchable
{
    /// <summary>
    /// 내 유저 정보
    /// </summary>
    public static User Me { get; private set; } = new User();

    public string UserId { get; set; }
    
    public string NickName { get; set; }

    public string UUID =>
        (UserId.Contains('_') ? UserId.Split('_')[1] : UserId).Substring(0, UserId.Length < 8 ? UserId.Length : 8);
    
    public List<Login.LoginType> UserIdpBindings { get; set; } = new List<Login.LoginType>();
    
    public void ApplyPatch(IDictionary<string, object> jsonObject)
    {
        Network.PatchHelper.ApplyPatch(this, jsonObject);
    }
}
