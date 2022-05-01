# NDataMapper
基于Ado .net Providers工作的ORM工具，扩展了IDbConnection接口。

# 使用方式
## 查询单行
```csharp
public static T? QueryFirst<T>(this IDbConnection conn, string sql, IDbTransaction? transaction = null, params IDbDataParameter[] paras);

public static dynamic? QueryFirst(this IDbConnection conn, string sql, IDbTransaction? transaction = null, params IDbDataParameter[] paras);

public static (Result1?, Result2?) QueryFirst<Result1, Result2>(this IDbConnection conn, string sql, IDbTransaction? transaction = null, params IDbDataParameter[] paras);
```
示例1：
```csharp
public class People
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int? Age { get; set; }
    public decimal? Height { get; set; }
    public char? Gender { get; set; }
    public DateOnly? Birthday { get; set; }
    public DateTime? CreateTime { get; set; }
    public string[]? Arr1 { get; set; }
    public int[]? Arr2 { get; set; }
}

static void Test()
{
    string sql;
    using NpgsqlConnection conn = new("Server=localhost;Port=5432;userid=postgres;Password=postgres;Database=test;");

    //查询单个数据
    sql = $@"select name from public.people where id=:id";
    string? name = conn.QueryFirst<string>(sql, paras: new NpgsqlParameter("id", 1));

    //支持可空的值类型
    sql = $@"select age from public.people where id=:id";
    int? name = conn.QueryFirst<int?>(sql, paras: new NpgsqlParameter("id", 1));

    //支持数组
    sql = $@"select arr1 from public.people where id=:id";
    int[]? name = conn.QueryFirst<int[]>(sql, paras: new NpgsqlParameter("id", 1));

    //支持对象模型
    sql = $@"select * from public.people where id=:id";
    People? people = conn.QueryFirst<People>(sql, paras: new NpgsqlParameter("id", 1));

    //支持动态对象
    sql = $@"select * from public.people where id=:id";
    var p = conn.QueryFirst(sql, paras: new NpgsqlParameter("id", 2));
    if (p is not null)
    {
        p.occupation = "程序员";
        p.Remove("id");//去掉键为id的值
    }

    //支持含2-5个值的元组
    sql = $@"select name,age from public.people where id=:id";
    var ( name, age) = conn.QueryFirst<string, int?>(sql, paras: new NpgsqlParameter("id", 2));
}
```