using System.Collections.Generic;

public static class IDPPlatformSupportUtil
{
    /// <summary>
    /// 플랫폼에 따라 지원하는 로그인 IDP 타입 리스트
    /// </summary>
    public static List<LoginType> GetSupportedIDPs()
    {
        var list = new List<LoginType>();

#if !UNITY_EDITOR
        #if UNITY_ANDROID || UNITY_IOS
                list.Add(LoginType.Google);
        #endif
        #if UNITY_IOS
                list.Add(LoginType.Apple);
        #endif
#endif
        list.Add(LoginType.Guest); // Always available

        return list;
    }
}
