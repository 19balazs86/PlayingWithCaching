using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OutputCachingApi.CachingUtils;
using System.Security.Claims;

namespace ApiTests.Core;

public sealed class AlbaHostFixture : IAsyncLifetime
{
    public IAlbaHost? AlbaWebHost { get; private set; }

    public static readonly TimeSpan DefaultDelay = 1.Seconds();

    public static readonly CacheSettings CacheSettings = new CacheSettings { DefaultExpiration = 5, Expire1min = 10 };

    public IEnumerable<Claim> TestUserClaims { get; set; } = TestUsers.AnonymousClaims;

    public async Task InitializeAsync()
    {
        AlbaWebHost = await AlbaHost.For<OutputCachingApi.Program>(configureWebHostBuilder);
    }

    private void configureWebHostBuilder(IWebHostBuilder webHostBuilder)
    {
        webHostBuilder.ConfigureTestServices(configureTestServices);

        webHostBuilder.ConfigureAppConfiguration(configureAppConfiguration);
    }

    private void configureTestServices(IServiceCollection services)
    {
        services.AddTestAuthentication(() => TestUserClaims);
    }

    private static void configureAppConfiguration(IConfigurationBuilder configurationBuilder)
    {
        var configurationOverridden = new Dictionary<string, string?>
        {
            ["CacheSettings:DefaultExpiration"] = CacheSettings.DefaultExpiration.ToString(),
            ["CacheSettings:Expire1min"]        = CacheSettings.Expire1min.ToString()
        };

        configurationBuilder.AddInMemoryCollection(configurationOverridden);
    }

    public async Task DisposeAsync()
    {
        if (AlbaWebHost is not null)
        {
            await AlbaWebHost.DisposeAsync();
        }
    }
}
