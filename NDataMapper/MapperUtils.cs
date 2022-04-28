namespace NDataMapper;

internal class MapperUtils
{
    private static readonly Dictionary<Type, Dictionary<string, PropertyInfo>> psCaches = new();

    internal static bool GetCachePropertis(Type type, out Dictionary<string, PropertyInfo>? ps) => psCaches.TryGetValue(type, out ps);

    internal static bool GetPropertyInfo(Type type, string name, out PropertyInfo? property)
    {
        if (!psCaches.ContainsKey(type))
        {
            psCaches.Add(type, new());
            type.GetProperties().ToList().ForEach(p =>
            {
                string ncolumnName = p.Name.ToLower();
                if (p.GetCustomAttribute<NColumnAttribute>() is NColumnAttribute nColumn)
                {
                    if (!string.IsNullOrWhiteSpace(nColumn.ColumnName)) ncolumnName = nColumn.ColumnName;
                }
                else if (p.GetCustomAttribute<ColumnAttribute>() is ColumnAttribute dColumn)
                {
                    if (!string.IsNullOrWhiteSpace(dColumn.Name)) ncolumnName = dColumn.Name;
                }
                psCaches[type].TryAdd(ncolumnName, p);
            });
        }
        return psCaches[type].TryGetValue(name, out property);
    }

    internal static int GetTypePropertyCount(Type type)
        => psCaches.TryGetValue(type, out var ps) ? ps.Count : 0;

    internal static T? GetValue<T>(object? obj)
    {
        try
        {
            var targetType = typeof(T);
            if (obj is null || Convert.IsDBNull(obj)
                || (targetType != typeof(string) && !targetType.IsValueType)) return default;
            if (obj is T tval) return tval;
            if (obj is Array array && targetType.IsArray)
            {
                var elementType = targetType.GetElementType();
                elementType = Nullable.GetUnderlyingType(elementType!) ?? elementType;
                var result = Array.CreateInstance(elementType!, array.Length);
                for (int i = 0; i < array.Length; i++)
                    result.SetValue(Convert.ChangeType(array.GetValue(i), elementType!), i);
                return (T)(object)result;
            }
            if (obj is DateTime date && (targetType == typeof(DateOnly) || targetType == typeof(DateOnly?))) 
                return (T)(object)DateOnly.FromDateTime(date);
            if (obj is DateTime time && (targetType == typeof(TimeOnly) || targetType == typeof(TimeOnly?)))
                return (T)(object)TimeOnly.FromDateTime(time);
            targetType = Nullable.GetUnderlyingType(targetType) ?? targetType;
            return (T)Convert.ChangeType(obj, targetType);
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// 是否是匿名类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    internal static bool IsAnonymousType(Type type) => type.FullName?.StartsWith("<>f__AnonymousType") is true;
}

[AttributeUsage(AttributeTargets.Property)]
public class NColumnAttribute: Attribute
{
    public string? ColumnName { get; set; }
    public NColumnAttribute(string columnName) => ColumnName = columnName;
}