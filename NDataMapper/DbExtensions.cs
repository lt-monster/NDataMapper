namespace NDataMapper;

public static class DbExtensions
{
    /// <summary>
    /// 查询单行数据
    /// </summary>
    /// <typeparam name="T">返回的类型</typeparam>
    /// <param name="conn">连接</param>
    /// <param name="sql">sql语句</param>
    /// <param name="transaction">事务</param>
    /// <param name="paras">参数</param>
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
        if (targetType.IsArray) targetType = targetType.GetElementType()??typeof(object);
        if (targetType.IsValueType || targetType == typeof(string) || targetType == typeof(object))
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
                    p?.SetValue(targetValue, MapperUtils.GetValue(reader[i], p.PropertyType));
                }
            }
        }
        return (T?)targetValue;
    }
}
