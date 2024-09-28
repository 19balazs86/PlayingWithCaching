using Microsoft.AspNetCore.OutputCaching;
using OutputCachingApi.Core;
using System.Net.Mime;

namespace OutputCachingApi.Endpoints;

public static class TestEndpoints
{
    public const string PathBase       = "/";
    public const string PathDefault    = "/default";
    public const string PathExpire1min = "/expire1min";

    public const string PathRequireAuth_NoCache    = "/require-auth";
    public const string PathRequireAuth_Default    = "/require-auth/default";
    public const string PathRequireAuth_VaryByUser = "/require-auth/vary-by-user";

    public static void MapTestEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(PathBase, writeGravatar); // NoCache by default without .CacheOutput()

        app.MapGet(PathDefault, writeGravatar).CacheOutputAuth(); // It uses the AuthCachePolicy.Instance and BasePolicy

        app.MapGet(PathExpire1min, writeGravatar).CacheOutput(Program.CachePolicyName_Expire1min);

        app.MapGet("/evict/{tag}", handleEvictByTag);

        // Maps for authenticated users
        var requireAuthGroup = app.MapGroup("/require-auth").RequireAuthorization();

        requireAuthGroup.MapGet("/", writeGravatar); // NoCache by default without .CacheOutput()

        requireAuthGroup.MapGet("/default", writeGravatar).CacheOutputAuth();

        requireAuthGroup.MapGet("/vary-by-user", writeGravatar)
                        .CacheOutputAuth(builder => builder.VaryByUserName().TagByUserName(name => [$"vary-endpoint-{name}"]));
    }

    private static async Task handleEvictByTag(string tag, IOutputCacheStore cache)
    {
        await cache.EvictByTagAsync(tag, default);

        if ("tag-all".Equals(tag))
        {
            await cache.EvictByTagAsync("tag-auth", default);
        }
    }

    private static async Task writeGravatar(HttpContext context)
    {
        string hash = Guid.NewGuid().ToString("n");

        const string _type = "monsterid"; // identicon, monsterid, wavatar
        const int _size    = 150;

        context.Response.ContentType = MediaTypeNames.Text.Html;

        await context.Response.WriteAsync($"<img src='https://www.gravatar.com/avatar/{hash}?s={_size}&d={_type}' />");

        await context.Response.WriteAsync($"<p>Generated at {DateTime.Now:hh:mm:ss.ff}</p>");
    }
}
