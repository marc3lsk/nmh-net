using System.Collections.Concurrent;
using Abstraction.KeyValueStore;

namespace Infrastructure.KeyValueStore;

public class KeyValueStoreInMemory<TKey, TValue> : IKeyValueStore<TKey, TValue>
    where TKey : notnull
{
    ConcurrentDictionary<TKey, TValue> _dictionary = new();

    public Task<TValue?> TryGetValueAsync(TKey key) =>
        Task.FromResult(_dictionary.TryGetValue(key, out var val) ? val : default);

    public Task UpsertValueAsync(TKey key, TValue value)
    {
        _dictionary.AddOrUpdate(key, value, (key, oldValue) => value);
        return Task.CompletedTask;
    }
}
