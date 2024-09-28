namespace ApiTests;

[Collection(nameof(SharedCollectionFixture))]
public sealed class AuthenticatedTests : CommonTests
{
    public AuthenticatedTests(AlbaHostFixture albaHostFixture) : base(albaHostFixture)
    {
        _albaHostFixture.TestUserClaims = TestUsers.User1Claims;
    }

    [Fact]
    public async Task Response_NotEqual_When_NotCached_on_Auth()
    {
        // Act
        EndpointResponse response = await initiateHttpCalls(TestEndpoints.PathRequireAuth_NoCache);

        // Assert
        Assert.NotEqual(response.ResponseText1, response.ResponseText2);
    }

    [Fact]
    public async Task Response_Equal_When_Cached_Default()
    {
        // Act
        EndpointResponse response = await initiateHttpCalls(TestEndpoints.PathRequireAuth_Default);

        // Assert
        Assert.Equal(response.ResponseText1, response.ResponseText2);
    }

    [Fact]
    public async Task Response_NotEqual_When_Cache_Expired_Default()
    {
        // Arrange
        TimeSpan delay = TimeSpan.FromSeconds(AlbaHostFixture.CacheSettings.DefaultExpiration + 1);

        // Act
        EndpointResponse response = await initiateHttpCalls(TestEndpoints.PathRequireAuth_Default, delay);

        // Assert
        Assert.NotEqual(response.ResponseText1, response.ResponseText2);
    }

    [Fact]
    public async Task Response_Equal_When_Cached_and_different_Users()
    {
        // Act
        EndpointResponse response = await initiateHttpCalls_With2Users(TestEndpoints.PathRequireAuth_Default);

        // Assert
        Assert.Equal(response.ResponseText1, response.ResponseText2);
    }

    [Fact]
    public async Task Response_NotEqual_When_Cached_and_different_Users()
    {
        // Act
        EndpointResponse response = await initiateHttpCalls_With2Users(TestEndpoints.PathRequireAuth_VaryByUser);

        // Assert
        Assert.NotEqual(response.ResponseText1, response.ResponseText2);
    }

    private async Task<EndpointResponse> initiateHttpCalls_With2Users(string urlPath, TimeSpan? delay = null)
    {
        var response = new EndpointResponse();

        response.ResponseText1 = await _albaHost.GetAsText(urlPath);

        await Task.Delay(delay ?? AlbaHostFixture.DefaultDelay);

        _albaHostFixture.TestUserClaims = TestUsers.User2Claims;

        response.ResponseText2 = await _albaHost.GetAsText(urlPath);

        return response;
    }
}