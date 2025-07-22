using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public static class PatchHelper
{
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
                prop.SetValue(target, value);
            }
            else if (typeof(IPatchable).IsAssignableFrom(prop.PropertyType) && value is Dictionary<string, object> subDict)
            {
                object nestedObj = prop.GetValue(target) ?? Activator.CreateInstance(prop.PropertyType);
                ((IPatchable)nestedObj).ApplyPatch(subDict);
                prop.SetValue(target, nestedObj);
            }
            else if (prop.PropertyType.IsEnum && value is string strVal)
            {
                if (Enum.TryParse(prop.PropertyType, strVal, out var enumParsed))
                    prop.SetValue(target, enumParsed);
            }
            else if (prop.PropertyType == typeof(DateTime?) && DateTime.TryParse(value.ToString(), out var dt))
            {
                prop.SetValue(target, dt);
            }
            else if (prop.PropertyType.IsGenericType &&
                     prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
            {
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
