using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.OutputCaching;

namespace PlayingWithCaching;

public static class Program
{
    public const string AuthScheme = CookieAuthenticationDefaults.AuthenticationScheme;

    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        IServiceCollection services   = builder.Services;

        // Add services to the container
        {
            services.addCookieAuth();

            services.AddAuthorization();

            services.AddOutputCache(options =>
            {
                options.AddBasePolicy(builder =>
                {
                    builder.NoCache()
                           .Expire(TimeSpan.FromSeconds(20))
                           .Tag("tag-all");
                });

                options.AddPolicy("Expire-1-min", builder => builder.Expire(TimeSpan.FromSeconds(60)));
            });
        }

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline
        {
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseOutputCache();

            app.MapAuthEndpoints();

            app.MapGet("/", Gravatar.WriteGravatar); // NoCache by default

            app.MapGet("/default", Gravatar.WriteGravatar).CacheOutput(); // It uses the BasePolicy

            app.MapGet("/expire1min", Gravatar.WriteGravatar).CacheOutput("Expire-1-min");

            app.MapGet("/evict/{tag}", handleEvictByTag);
        }

        app.UseAuthorization();

        app.Run();
    }

    private static async Task handleEvictByTag(string tag, IOutputCacheStore cache)
    {
        await cache.EvictByTagAsync(tag, default);
    }

    private static void addCookieAuth(this IServiceCollection services)
    {
        services.AddAuthentication(AuthScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "auth-cookie";
                options.Events.OnRedirectToLogin = preventRedirect;
            });
    }

    private static Task preventRedirect(RedirectContext<CookieAuthenticationOptions> context)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;

        return Task.CompletedTask;
    }
}
