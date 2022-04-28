namespace NDataMapper;

public static class DbExtensions
{
    /// <summary>
    /// 查询单行数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="conn"></param>
    /// <param name="sql"></param>
    /// <param name="transaction"></param>
    /// <param name="paras"></param>
    /// <returns></returns>
    public static T? QueryFirst<T>(this IDbConnection conn, string sql, IDbTransaction? transaction = null, params IDbDataParameter[] paras)
    {
        if (string.IsNullOrWhiteSpace(sql)) return default;
        if (conn.State == ConnectionState.Closed) conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.Transaction = transaction;
        foreach (var para in paras) cmd.Parameters.Add(para);
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
