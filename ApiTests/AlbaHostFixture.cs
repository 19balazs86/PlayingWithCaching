using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace ApiTests;

public sealed class AlbaHostFixture : IAsyncLifetime
{
    public IAlbaHost? AlbaWebHost { get; set; }

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
            ["TestConfig:Key1"] = "TestConfigValue"
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
