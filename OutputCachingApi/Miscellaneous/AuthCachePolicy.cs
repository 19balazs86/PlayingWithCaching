using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Primitives;

namespace OutputCachingApi.Miscellaneous;

/// <summary>
/// This is a copy of the DefaultPolicy, but it allows authenticated GET and HEAD methods to be cached
/// </summary>
public sealed class AuthCachePolicy : IOutputCachePolicy
{
    public static readonly AuthCachePolicy Instance = new AuthCachePolicy();

    public AuthCachePolicy()
    {
    }

    public ValueTask CacheRequestAsync(OutputCacheContext context, CancellationToken cancellationToken)
    {
        bool attemptOutputCaching = AuthCachePolicy.attemptOutputCaching(context);

        context.AllowCacheLookup    = attemptOutputCaching;
        context.AllowCacheStorage   = attemptOutputCaching;
        context.EnableOutputCaching = true;
        context.AllowLocking        = true;

        // Vary by any query by default
        context.CacheVaryByRules.QueryKeys = "*";

        return ValueTask.CompletedTask;
    }

    public ValueTask ServeFromCacheAsync(OutputCacheContext context, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask ServeResponseAsync(OutputCacheContext context, CancellationToken cancellationToken)
    {
        var response = context.HttpContext.Response;

        // Verify existence of cookie headers
        if (!StringValues.IsNullOrEmpty(response.Headers.SetCookie))
        {
            context.AllowCacheStorage = false;
            return ValueTask.CompletedTask;
        }

        // Check response code
        if (response.StatusCode != StatusCodes.Status200OK)
        {
            context.AllowCacheStorage = false;
            return ValueTask.CompletedTask;
        }

        return ValueTask.CompletedTask;
    }

    private static bool attemptOutputCaching(OutputCacheContext context)
    {
        // Check if the current request fulfills the requirements to be cached

        var request = context.HttpContext.Request;

        // Verify the method
        if (!HttpMethods.IsGet(request.Method) && !HttpMethods.IsHead(request.Method))
        {
            return false;
        }

        // Verify existence of authorization headers
        //if (!StringValues.IsNullOrEmpty(request.Headers.Authorization) || request.HttpContext.User?.Identity?.IsAuthenticated == true)
        //{
        //    return false;
        //}

        return true;
    }
}
