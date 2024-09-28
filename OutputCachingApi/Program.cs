using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using OutputCachingApi.CachingUtils;
using OutputCachingApi.Miscellaneous;

namespace OutputCachingApi;

public sealed class Program
{
    public const string CachePolicyName_Expire1min = "Expire-1-min";

    public const string AuthScheme = CookieAuthenticationDefaults.AuthenticationScheme;

    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        IServiceCollection services   = builder.Services;

        // Add services to the container
        {
            addCookieAuth(services);

            services.AddAuthorization();

            services.AddOutputCache(options =>
            {
                options.AddBasePolicy(builder =>
                {
                    builder.NoCache()
                           .Expire(TimeSpan.FromSeconds(20))
                           .Tag("tag-all");
                });

                options.AddPolicyAuth(CachePolicyName_Expire1min, builder => builder.Expire(TimeSpan.FromSeconds(60)));
            });
        }

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline
        {
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseOutputCache();

            app.MapAuthEndpoints();

            app.MapTestEndpoints();
        }

        app.UseAuthorization();

        app.Run();
    }

    private static void addCookieAuth(IServiceCollection services)
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
