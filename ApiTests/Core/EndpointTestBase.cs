using OutputCachingApi.Core;

namespace ApiTests.Core;

public abstract class EndpointTestBase : IAsyncLifetime
{
    public static readonly TimeSpan DefaultDelay = 1.Seconds();

    public const int CacheDefaultExpiration = 5; // seconds

    protected readonly AlbaHostFixture _albaHostFixture;

    protected IAlbaHost _albaHost => _albaHostFixture.AlbaWebHost ?? throw new NullReferenceException("AlbaWebHost is null");

    public EndpointTestBase(AlbaHostFixture albaHostFixture)
    {
        _albaHostFixture = albaHostFixture;
    }

    protected async Task<EndpointResponse> initiateHttpCalls(string urlPath, TimeSpan? delay = null)
    {
        string response1 = await _albaHost.GetAsText(urlPath);

        await Task.Delay(delay ?? DefaultDelay);

        string response2 = await _albaHost.GetAsText(urlPath);

        return new EndpointResponse(response1, response2);
    }

    // To get a fresh start with an empty cache, restart the server
    public async Task InitializeAsync()
    {
        await _albaHost.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _albaHost.StopAsync();
    }
}
