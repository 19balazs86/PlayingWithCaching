using Microsoft.AspNetCore.OutputCaching;

namespace OutputCachingApi.Miscellaneous;

public sealed class CustomTagsPolicy : IOutputCachePolicy
{
    private readonly Func<HttpContext, string[]> _tagsFunc;

    public CustomTagsPolicy(Func<HttpContext, string[]> tagsFunc)
    {
        _tagsFunc = tagsFunc;
    }

    public ValueTask CacheRequestAsync(OutputCacheContext context, CancellationToken cancellation)
    {
        foreach (string tag in _tagsFunc(context.HttpContext))
        {
            context.Tags.Add(tag);
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask ServeFromCacheAsync(OutputCacheContext context, CancellationToken cancellation)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask ServeResponseAsync(OutputCacheContext context, CancellationToken cancellation)
    {
        return ValueTask.CompletedTask;
    }
}
