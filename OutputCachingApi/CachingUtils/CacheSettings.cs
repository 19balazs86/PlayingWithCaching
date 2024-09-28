﻿namespace OutputCachingApi.CachingUtils;

public sealed class CacheSettings
{
    public int DefaultExpiration { get; init; }
    public int Expire1min { get; init; }
}
