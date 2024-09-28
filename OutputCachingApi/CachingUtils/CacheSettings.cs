namespace OutputCachingApi.CachingUtils;

public sealed class CacheSettings
{
    public int DefaultExpirationInSeconds { get; init; }
    public int Expire1min { get; init; }
}
