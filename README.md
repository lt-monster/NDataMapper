# NDataMapper
基于.NET6的以Ado .net Providers工作的一个简单ORM工具，扩展了IDbConnection接口。

## 使用方式
### 查询单行
返回单一类型、数组、对象模型
```csharp
public static T? QueryFirst<T>(this IDbConnection conn, string sql, IDbTransaction? transaction = null, params IDbDataParameter[] paras);
```
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
}
```

返回动态类型，可新增或者移除动态属性，大小写不敏感
```csharp
public static dynamic? QueryFirst(this IDbConnection conn, string sql, IDbTransaction? transaction = null, params IDbDataParameter[] paras);
```
```csharp
sql = $@"select * from public.people where id=:id";
var p = conn.QueryFirst(sql, paras: new NpgsqlParameter("id", 2));
if (p is not null)
{
    p.occupation = "程序员";//添加一组值
    p.Remove("id");//移除名为id动态属性
}
```

### 👏查询元组
返回元组，最多支持2-5个
```csharp
public static (Result1?, Result2?) QueryFirst<Result1, Result2>(this IDbConnection conn, string sql, IDbTransaction? transaction = null, params IDbDataParameter[] paras);
```
```csharp
sql = $@"select name,age from public.people where id=:id";
var (name, age) = conn.QueryFirst<string, int?>(sql, paras: new NpgsqlParameter("id", 2));
```

### 查询集合
返回单一类型集合
```csharp
public static IEnumerable<T?> Query<T>(this IDbConnection conn, string sql, IDbTransaction? transaction = null, params IDbDataParameter[] paras)
```
```csharp
sql = $@"select name from public.people limit 15";
IEnumerable<string?> names = conn.Query<string>(sql);

sql = $@"select * from public.people limit 15";
IEnumerable<People?> peoples = conn.Query<People>(sql);
```

返回动态类型集合
```csharp
public static IEnumerable<dynamic?> Query(this IDbConnection conn, string sql, IDbTransaction? transaction = null, params IDbDataParameter[] paras)
```
```csharp
sql = $@"select * from public.people limit 15";
IEnumerable<dynamic?> peoples = conn.Query(sql);
```

### 自定义字段别名
支持.NET自带的字段特性System.ComponentModel.DataAnnotations.Schema.ColumnAttribute或者NDataMapper.NColumnAttribute
```csharp
public class People
{
    public int Id { get; set; }
    [Column("realname")]
    //[NColumn("realname")]
    public string? Name { get; set; }
}
```

### 忽略字段
支持.NET自带的字段特性System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute或者NDataMapper.IgnoreColumnAttribute
```csharp
public class People
{
    [IgnoreColumn]
    public int Id { get; set; }
    [NotMapped]
    public string? Name { get; set; }
}
```
