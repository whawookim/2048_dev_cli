using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Network
{
    /// <summary>
    /// 서버에서 내려온 JSON 데이터를 객체에 덮어씌우는 유틸리티 클래스.
    /// Reflection을 사용하여 property 기반으로 값을 반영하며,
    /// Enum, List, Nullable, IPatchable 등을 지원.
    /// </summary>
    public static class PatchHelper
    {
        /// <summary>
        /// JSON 객체를 target 인스턴스에 반영합니다.
        /// 타입 변환, 재귀 patch, enum/nullable/list 처리까지 지원합니다.
        /// </summary>
        /// <param name="target">패치할 대상 객체</param>
        /// <param name="jsonObject">서버에서 받은 Dictionary 데이터</param>
        public static void ApplyPatch(object target, IDictionary<string, object> jsonObject)
        {
            if (target == null || jsonObject == null)
                return;

            Type type = target.GetType();
            foreach (var kv in jsonObject)
            {
                PropertyInfo prop = type.GetProperty(kv.Key, BindingFlags.Public | BindingFlags.Instance);
                if (prop == null || !prop.CanWrite)
                    continue;

                object value = kv.Value;

                if (value == null)
                {
                    prop.SetValue(target, null);
                }
                else if (prop.PropertyType.IsAssignableFrom(value.GetType()))
                {
                    // 타입이 일치할 경우 바로 할당
                    prop.SetValue(target, value);
                }
                else if (typeof(IPatchable).IsAssignableFrom(prop.PropertyType) && value is Dictionary<string, object> subDict)
                {
                    // IPatchable 구현체인 경우 재귀 패치
                    object nestedObj = prop.GetValue(target) ?? Activator.CreateInstance(prop.PropertyType);
                    ((IPatchable)nestedObj).ApplyPatch(subDict);
                    prop.SetValue(target, nestedObj);
                }
                else if (prop.PropertyType.IsEnum && value is string strVal)
                {
                    // Enum 문자열 → 값 변환
                    if (Enum.TryParse(prop.PropertyType, strVal, out var enumParsed))
                        prop.SetValue(target, enumParsed);
                }
                else if (prop.PropertyType == typeof(DateTime?) && DateTime.TryParse(value.ToString(), out var dt))
                {
                    // Nullable DateTime 처리
                    prop.SetValue(target, dt);
                }
                else if (prop.PropertyType.IsGenericType &&
                         prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    // List<T> 처리
                    var elementType = prop.PropertyType.GetGenericArguments()[0];
                    var list = (IList)Activator.CreateInstance(prop.PropertyType);
                    if (value is IEnumerable enumerable)
                    {
                        foreach (var item in enumerable)
                        {
                            list.Add(Convert.ChangeType(item, elementType));
                        }
                    }
                    prop.SetValue(target, list);
                }
                else
                {
                    // 일반 타입 변환
                    try
                    {
                        prop.SetValue(target, Convert.ChangeType(value, prop.PropertyType));
                    }
                    catch
                    {
                        // Conversion 실패 무시
                    }
                }
            }
        }
    }   
}
