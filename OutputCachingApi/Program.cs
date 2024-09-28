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
        IConfiguration configuration  = builder.Configuration;

        // Add services to the container
        {
            addCookieAuth(services);

            services.AddAuthorization();

            services.AddOutputCache(options =>
            {
                CacheSettings cacheSettings = configuration.GetSection("CacheSettings").Get<CacheSettings>() ?? throw new NullReferenceException("Missing configuration: CacheSettings");

                options.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(cacheSettings.DefaultExpirationInSeconds);

                options.AddBasePolicy(builder => builder.NoCache().Tag("tag-all"));

                options.AddPolicyAuth(CachePolicyName_Expire1min, builder => builder.Expire(TimeSpan.FromSeconds(cacheSettings.Expire1min)));
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
