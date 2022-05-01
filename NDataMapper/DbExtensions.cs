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

    /// <summary>
    /// 查询单行数据
    /// </summary>
    /// <param name="conn">连接</param>
    /// <param name="sql">sql语句</param>
    /// <param name="transaction">事务</param>
    /// <param name="paras">参数</param>
    /// <returns></returns>
    public static dynamic? QueryFirst(this IDbConnection conn, string sql, IDbTransaction? transaction = null, params IDbDataParameter[] paras)
    {
        if (string.IsNullOrWhiteSpace(sql)) return default;
        if (conn.State == ConnectionState.Closed) conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.Transaction = transaction;
        foreach (var para in paras) cmd.Parameters.Add(para);
        cmd.CommandText = sql;
        dynamic? targetValue = null;
        using var reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
        if (reader.Read())
        {
            targetValue = new NDynamicRow();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                targetValue[reader.GetName(i)] = reader[i];
            }
        }
        return targetValue;
    }

    /// <summary>
    /// 查询单行数据
    /// </summary>
    /// <typeparam name="T">返回的类型</typeparam>
    /// <param name="conn">连接</param>
    /// <param name="sql">sql语句</param>
    /// <param name="transaction">事务</param>
    /// <param name="paras">参数</param>
    /// <returns></returns>
    public static (Result1?, Result2?) QueryFirst<Result1, Result2>(this IDbConnection conn, string sql, IDbTransaction? transaction = null, params IDbDataParameter[] paras)
    {
        if (string.IsNullOrWhiteSpace(sql)) return default;
        if (conn.State == ConnectionState.Closed) conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.Transaction = transaction;
        foreach (var para in paras) cmd.Parameters.Add(para);
        cmd.CommandText = sql;
        Result1? r1 = default;
        Result2? r2 = default;
        using var reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
        if (reader.Read())
        {
            r1 = MapperUtils.GetValue<Result1?>(reader.GetValue(0));
            if(reader.FieldCount > 1) r2 = MapperUtils.GetValue<Result2?>(reader.GetValue(1));
        }
        return (r1, r2);
    }

    /// <summary>
    /// 查询单行数据
    /// </summary>
    /// <typeparam name="T">返回的类型</typeparam>
    /// <param name="conn">连接</param>
    /// <param name="sql">sql语句</param>
    /// <param name="transaction">事务</param>
    /// <param name="paras">参数</param>
    /// <returns></returns>
    public static (Result1?, Result2?, Result3?) QueryFirst<Result1, Result2, Result3>(this IDbConnection conn, string sql, IDbTransaction? transaction = null, params IDbDataParameter[] paras)
    {
        if (string.IsNullOrWhiteSpace(sql)) return default;
        if (conn.State == ConnectionState.Closed) conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.Transaction = transaction;
        foreach (var para in paras) cmd.Parameters.Add(para);
        cmd.CommandText = sql;
        Result1? r1 = default;
        Result2? r2 = default;
        Result3? r3 = default;
        using var reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
        if (reader.Read())
        {
            r1 = MapperUtils.GetValue<Result1?>(reader.GetValue(0));
            if (reader.FieldCount > 1) r2 = MapperUtils.GetValue<Result2?>(reader.GetValue(1));
            if (reader.FieldCount > 2) r3 = MapperUtils.GetValue<Result3?>(reader.GetValue(2));
        }
        return (r1, r2, r3);
    }

    /// <summary>
    /// 查询单行数据
    /// </summary>
    /// <typeparam name="T">返回的类型</typeparam>
    /// <param name="conn">连接</param>
    /// <param name="sql">sql语句</param>
    /// <param name="transaction">事务</param>
    /// <param name="paras">参数</param>
    /// <returns></returns>
    public static (Result1?, Result2?, Result3?, Result4?) QueryFirst<Result1, Result2, Result3, Result4>(this IDbConnection conn, string sql, IDbTransaction? transaction = null, params IDbDataParameter[] paras)
    {
        if (string.IsNullOrWhiteSpace(sql)) return default;
        if (conn.State == ConnectionState.Closed) conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.Transaction = transaction;
        foreach (var para in paras) cmd.Parameters.Add(para);
        cmd.CommandText = sql;
        Result1? r1 = default;
        Result2? r2 = default;
        Result3? r3 = default;
        Result4? r4 = default;
        using var reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
        if (reader.Read())
        {
            r1 = MapperUtils.GetValue<Result1?>(reader.GetValue(0));
            if (reader.FieldCount > 1) r2 = MapperUtils.GetValue<Result2?>(reader.GetValue(1));
            if (reader.FieldCount > 2) r3 = MapperUtils.GetValue<Result3?>(reader.GetValue(2));
            if (reader.FieldCount > 3) r4 = MapperUtils.GetValue<Result4?>(reader.GetValue(3));
        }
        return (r1, r2, r3, r4);
    }

    /// <summary>
    /// 查询单行数据
    /// </summary>
    /// <typeparam name="T">返回的类型</typeparam>
    /// <param name="conn">连接</param>
    /// <param name="sql">sql语句</param>
    /// <param name="transaction">事务</param>
    /// <param name="paras">参数</param>
    /// <returns></returns>
    public static (Result1?, Result2?, Result3?, Result4?, Result5?) QueryFirst<Result1, Result2, Result3, Result4, Result5>(this IDbConnection conn, string sql, IDbTransaction? transaction = null, params IDbDataParameter[] paras)
    {
        if (string.IsNullOrWhiteSpace(sql)) return default;
        if (conn.State == ConnectionState.Closed) conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.Transaction = transaction;
        foreach (var para in paras) cmd.Parameters.Add(para);
        cmd.CommandText = sql;
        Result1? r1 = default;
        Result2? r2 = default;
        Result3? r3 = default;
        Result4? r4 = default;
        Result5? r5 = default;
        using var reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
        if (reader.Read())
        {
            r1 = MapperUtils.GetValue<Result1?>(reader.GetValue(0));
            if (reader.FieldCount > 1) r2 = MapperUtils.GetValue<Result2?>(reader.GetValue(1));
            if (reader.FieldCount > 2) r3 = MapperUtils.GetValue<Result3?>(reader.GetValue(2));
            if (reader.FieldCount > 3) r4 = MapperUtils.GetValue<Result4?>(reader.GetValue(3));
            if (reader.FieldCount > 4) r5 = MapperUtils.GetValue<Result5?>(reader.GetValue(4));
        }
        return (r1, r2, r3, r4, r5);
    }
}
