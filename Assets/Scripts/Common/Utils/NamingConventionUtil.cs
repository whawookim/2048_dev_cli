using System;

/// <summary>
/// Api 호출 등의 문제로 서버에서는 camelCase를 사용하기에 변환을 거치기용
/// </summary>
public static class NamingConventionUtil
{
    public static string ToCamelCase(this string input)
    {
        if (string.IsNullOrEmpty(input) || char.IsLower(input[0]))
            return input;

        return char.ToLowerInvariant(input[0]) + input.Substring(1);
    }

    public static string ToCamelCase<TEnum>(this TEnum value) where TEnum : Enum
    {
        return value.ToString().ToCamelCase();
    }
}
