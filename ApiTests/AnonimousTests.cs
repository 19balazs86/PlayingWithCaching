using Microsoft.AspNetCore.Http;
using EndpointResponse = (string ResponseText1, string ResponseText2);

namespace ApiTests;

public sealed class AnonimousTests : IClassFixture<AlbaHostFixture>
{
    private readonly AlbaHostFixture _albaHostFixture;

    private IAlbaHost _albaHost => _albaHostFixture.AlbaWebHost ?? throw new NullReferenceException("AlbaWebHost is null");

    public AnonimousTests(AlbaHostFixture albaHostFixture)
    {
        _albaHostFixture = albaHostFixture;

        _albaHostFixture.TestUserClaims = TestUsers.AnonimousClaims;
    }

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

    [Fact]
    public async Task Response_NotEqual_When_Cache_Expired()
    {
        // Arrange
        TimeSpan delay = TimeSpan.FromSeconds(AlbaHostFixture.CacheSettings.DefaultExpiration + 1);

        // Act
        EndpointResponse response = await initiateHttpCalls(TestEndpoints.PathDefault, delay);

        // Assert
        Assert.NotEqual(response.ResponseText1, response.ResponseText2);
    }

    [Fact]
    public async Task Should_Unauthorized_StatusCode()
    {
        // Act + Assert
        await _albaHost.Scenario(scenario =>
        {
            scenario.Get.Url(TestEndpoints.PathRequireAuth);

            scenario.StatusCodeShouldBe(StatusCodes.Status401Unauthorized);
        });
    }

    private async Task<EndpointResponse> initiateHttpCalls(string urlPath, TimeSpan? delay = null)
    {
        string responseText1 = await _albaHost.GetAsText(urlPath);

        await Task.Delay(delay ?? AlbaHostFixture.DefaultDelay);

        string responseText2 = await _albaHost.GetAsText(urlPath);

        return (responseText1, responseText2);
    }
}