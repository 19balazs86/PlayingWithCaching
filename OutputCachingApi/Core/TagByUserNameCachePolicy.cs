using Microsoft.AspNetCore.OutputCaching;

namespace OutputCachingApi.Core;

public sealed class TagByUserNameCachePolicy : IOutputCachePolicy
{
    private readonly Func<string, string[]> _tagsFunc;

    public TagByUserNameCachePolicy(Func<string, string[]> tagsFunc)
    {
        _tagsFunc = tagsFunc;
    }

    public ValueTask CacheRequestAsync(OutputCacheContext context, CancellationToken cancellation)
    {
        string userName = getUserName(context.HttpContext);

        foreach (string tag in _tagsFunc(userName))
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

    private static string getUserName(HttpContext httpContext)
    {
        return httpContext.User.Identity?.Name ?? "Anonymous";
    }
}
