namespace NDataMapper;

public static class DbExtensions
{
    public static T? QueryFirst<T>(this IDbConnection conn, string sql, object? para = null)
    {
        if (string.IsNullOrWhiteSpace(sql)) return default;
        if(conn.State == ConnectionState.Closed) conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        var targetType = typeof(T);
        if (targetType.IsValueType || targetType == typeof(string) || targetType.IsArray)
        {
            return MapperUtils.GetValue<T>(cmd.ExecuteScalar());
        }
        object? targetValue = null;
        using var reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
        if (reader.Read())
        {
            targetValue = Activator.CreateInstance(targetType);
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (MapperUtils.GetPropertyInfo(targetType, reader.GetName(i), out var p))
                {
                    p?.SetValue(targetValue, reader.GetValue(i));
                }
            }
        }
        return (T?)targetValue;
    }
}
