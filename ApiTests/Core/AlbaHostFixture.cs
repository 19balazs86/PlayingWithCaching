using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using OutputCachingApi.CachingUtils;

namespace ApiTests.Core;

public sealed class AlbaHostFixture : IAsyncLifetime
{
    public IAlbaHost? AlbaWebHost { get; set; }

    public static readonly TimeSpan DefaultDelay = 1.Seconds();

    public static readonly CacheSettings CacheSettings = new CacheSettings { DefaultExpiration = 5, Expire1min = 10 };

    public async Task InitializeAsync()
    {
        //var securityStub = new CustomAuthenticationStub(() => TestUser?.ToClaims());
        //AlbaWebHost = await AlbaHost.For<PlayingWithTestHost.Program>(configureWebHostBuilder, securityStub);

        AlbaWebHost = await AlbaHost.For<OutputCachingApi.Program>(configureWebHostBuilder);
    }

    private void configureWebHostBuilder(IWebHostBuilder webHostBuilder)
    {
        webHostBuilder.ConfigureAppConfiguration(configureAppConfiguration);
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
