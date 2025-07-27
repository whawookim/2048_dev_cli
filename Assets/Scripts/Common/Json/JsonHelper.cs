using System;
using System.Collections.Generic;

/// <summary>
/// Dictionary 기반 JSON-like 구조에서 안전하게 값을 추출하는 헬퍼 클래스
/// </summary>
public static class JsonHelper
{
    /// <summary>
    /// 딕셔너리에서 키에 해당하는 값을 제네릭 타입으로 안전하게 추출
    /// - 타입이 맞을 경우 바로 캐스팅
    /// - 타입이 다를 경우 ChangeType으로 시도
    /// - 실패 시 기본값 반환
    /// </summary>
    /// <typeparam name="T">리턴할 타입</typeparam>
    /// <param name="dict">대상 딕셔너리</param>
    /// <param name="key">찾을 키</param>
    /// <param name="defaultValue">없거나 변환 실패 시 반환할 기본값</param>
    /// <returns>키에 해당하는 값 또는 기본값</returns>
    public static T Get<T>(this Dictionary<string, object> dict, string key, T defaultValue = default)
    {
        if (dict.TryGetValue(key, out var value) && value is T typed)
            return typed;
        if (dict.TryGetValue(key, out var raw))
            return (T)Convert.ChangeType(raw, typeof(T));
        return defaultValue;
    }
}
