namespace Abstraction.KeyValueStore;

public interface IKeyValueStore<TKey, TValue>
    where TKey : notnull
{
    Task<TValue?> TryGetValueAsync(TKey key);

    Task UpsertValueAsync(TKey key, TValue value);
}
