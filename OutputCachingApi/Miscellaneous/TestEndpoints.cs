using Microsoft.AspNetCore.OutputCaching;
using OutputCachingApi.CachingUtils;

namespace OutputCachingApi.Miscellaneous;

public static class TestEndpoints
{
    public const string PathBase        = "/";
    public const string PathDefault     = "/default";
    public const string PathExpire1min  = "/expire1min";
    public const string PathRequireAuth = "/require-auth";

    public static void MapTestEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(PathBase, Gravatar.WriteGravatar); // NoCache by default without .CacheOutput()

        app.MapGet(PathDefault, Gravatar.WriteGravatar).CacheOutputAuth(); // It uses the AuthCachePolicy.Instance and BasePolicy

        app.MapGet(PathExpire1min, Gravatar.WriteGravatar).CacheOutput(Program.CachePolicyName_Expire1min);

        app.MapGet(PathRequireAuth, Gravatar.WriteGravatar).RequireAuthorization().CacheOutputAuth(); // It uses the AuthCachePolicy.Instance and BasePolicy

        app.MapGet("/evict/{tag}", handleEvictByTag);
    }

    private static async Task handleEvictByTag(string tag, IOutputCacheStore cache)
    {
        await cache.EvictByTagAsync(tag, default);

        if ("tag-all".Equals(tag))
        {
            await cache.EvictByTagAsync("tag-auth", default);
        }
    }
}
