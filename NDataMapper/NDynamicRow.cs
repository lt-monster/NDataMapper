using System.Diagnostics.CodeAnalysis;
using System.Dynamic;

namespace NDataMapper;

public class NDynamicRow : DynamicObject, IDictionary<string, object>
{
    private readonly Dictionary<string, object> container = new();

    public ICollection<string> Keys => container.Keys;

    public ICollection<object> Values => container.Values;

    public int Count => container.Count;

    public bool IsReadOnly => false;

    public object this[string key] { get => container[key??"__value"]; set => container[key??"__value"] = value; }

    #region DynamicObject接口实现
    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        return container.TryGetValue(binder.Name.ToLower(), out result);
    }

    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        return container.TryAdd(binder.Name.ToLower(), value!);
    }

    public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object? value)
    {
        return container.TryAdd(indexes[0]?.ToString()?.ToLower()??"__value", value!); ;
    }

    public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object? result)
    {
        return container.TryGetValue(indexes[0]?.ToString()?.ToLower()?? "__value", out result);
    }

    public override bool TryConvert(ConvertBinder binder, out object? result)
    {
        result = container;
        return true;
    }

    public override bool TryInvoke(InvokeBinder binder, object?[]? args, out object? result)
    {
        return base.TryInvoke(binder, args, out result);
    }

    public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result)
    {
        if (binder.Name == "Remove")
        {
            args?.ToList()?.ForEach(arg =>
            {
                container.Remove(arg?.ToString()?.ToLower()??"__value");
            });
        }
        result = this;
        return true;
    }

    #endregion


    #region IDictionary接口实现
    public void Add(string key, object value) => container.Add(key, value);

    public bool ContainsKey(string key) => container.ContainsKey(key);

    public bool Remove(string key) => container.Remove(key);

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out object value)
        => container.TryGetValue(key, out value);

    public void Add(KeyValuePair<string, object> item) => container.Add(item.Key, item.Value);

    public void Clear() => container.Clear();

    public bool Contains(KeyValuePair<string, object> item)
    {
        if (item.Value?.GetType()?.IsValueType == true || item.Value?.GetType() == typeof(string))
        {
            return container.ContainsKey(item.Key) && container[item.Key].Equals(item.Value);
        }
        return container.ContainsKey(item.Key) && ReferenceEquals(container[item.Key], item.Value);
    }

    public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
    {
        if (arrayIndex < 0 || arrayIndex > container.Count - 1)
        {
            throw new ArgumentException("arrayIndex error");
        }
        array = container.Take(arrayIndex..).ToArray();
    }

    public bool Remove(KeyValuePair<string, object> item)
        => Contains(item) && container.Remove(item.Key);

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        => container.GetEnumerator();

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        => GetEnumerator();
    #endregion
}
