using EndpointResponse = (string ResponseText1, string ResponseText2);

namespace ApiTests;

public sealed class AnonimousTests(AlbaHostFixture albaHostFixture) : IClassFixture<AlbaHostFixture>
{
    private IAlbaHost _albaHost => albaHostFixture.AlbaWebHost ?? throw new NullReferenceException("AlbaWebHost is null");

    [Fact]
    public async Task Response_NotEqual_When_NotCached()
    {
        // Act
        EndpointResponse response = await initiateHttpCalls(TestEndpoints.PathBase);

        // Assert
        Assert.NotEqual(response.ResponseText1, response.ResponseText2);
    }

    [Fact]
    public async Task Response_Equal_When_Cached()
    {
        // Act
        EndpointResponse response = await initiateHttpCalls(TestEndpoints.PathDefault);

        // Assert
        Assert.Equal(response.ResponseText1, response.ResponseText2);
    }

    private async Task<EndpointResponse> initiateHttpCalls(string urlPath, TimeSpan? delay = null)
    {
        string responseText1 = await _albaHost.GetAsText(urlPath);

        await Task.Delay(delay ?? TimeSpan.FromSeconds(1));

        string responseText2 = await _albaHost.GetAsText(urlPath);

        return (responseText1, responseText2);
    }
}