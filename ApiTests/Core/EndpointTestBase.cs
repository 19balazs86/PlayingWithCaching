namespace ApiTests.Core;

public abstract class EndpointTestBase : IAsyncLifetime
{
    protected readonly AlbaHostFixture _albaHostFixture;

    protected IAlbaHost _albaHost => _albaHostFixture.AlbaWebHost ?? throw new NullReferenceException("AlbaWebHost is null");

    public EndpointTestBase(AlbaHostFixture albaHostFixture)
    {
        _albaHostFixture = albaHostFixture;
    }

    protected async Task<EndpointResponse> initiateHttpCalls(string urlPath, TimeSpan? delay = null)
    {
        var response = new EndpointResponse();

        response.ResponseText1 = await _albaHost.GetAsText(urlPath);

        await Task.Delay(delay ?? AlbaHostFixture.DefaultDelay);

        response.ResponseText2 = await _albaHost.GetAsText(urlPath);

        return response;
    }

    public async Task InitializeAsync()
    {
        await _albaHost.Scenario(scenario => scenario.Get.Url("/evict/tag-all"));
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
