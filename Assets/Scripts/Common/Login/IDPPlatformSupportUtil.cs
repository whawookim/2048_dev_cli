using System.Collections.Generic;

namespace Login
{
    /// <summary>
    /// 플랫폼별로 지원 가능한 로그인 방식(LoginType)을 반환하는 유틸리티 클래스.
    /// UNITY_ANDROID, UNITY_IOS 등을 기준으로 조건부로 IDP를 반환하며,
    /// Guest 로그인은 항상 포함됩니다.
    /// </summary>
    public static class IDPPlatformSupportUtil
    {
        /// <summary>
        /// 현재 플랫폼에서 지원하는 IDP(LoginType) 목록을 반환합니다.
        /// - Android/iOS: Google
        /// - iOS: Apple
        /// - 공통: Guest
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
}
