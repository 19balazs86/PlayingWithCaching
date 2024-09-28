using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using OutputCachingApi.CachingUtils;
using OutputCachingApi.Endpoints;

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

                options.DefaultExpirationTimeSpan = cacheSettings.DefaultExpiration.Seconds();

                options.AddBasePolicy(builder => builder.NoCache().Tag("tag-all"));

                options.AddBasePolicy(builder =>
                {
                    builder.With(context => AuthCachePolicy.IsEndpointRequireAuthorization(context.HttpContext))
                           .TagByUserName()
                           .Tag("tag-auth");
                });

                options.AddPolicyAuth(CachePolicyName_Expire1min, builder => builder.Expire(cacheSettings.Expire1min.Seconds()));
            });

            // Install-Package Microsoft.AspNetCore.OutputCaching.StackExchangeRedis
            //services.AddStackExchangeRedisOutputCache(options =>
            //{
            //    options.Configuration = "127.0.0.1:6379";
            //    options.InstanceName  = "CachingApi";
            //});
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
