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
        response.AssertNotEqual();
    }

    [Fact]
    public async Task Response_Equal_When_Cached_Default()
    {
        // Act
        EndpointResponse response = await initiateHttpCalls(TestEndpoints.PathRequireAuth_Default);

        // Assert
        response.AssertEqual();
    }

    [Fact]
    public async Task Response_NotEqual_When_Cache_Expired_Default()
    {
        // Arrange
        TimeSpan delay = TimeSpan.FromSeconds(CacheDefaultExpiration + 1);

        // Act
        EndpointResponse response = await initiateHttpCalls(TestEndpoints.PathRequireAuth_Default, delay);

        // Assert
        response.AssertNotEqual();
    }

    [Fact]
    public async Task Response_Equal_When_Cached_and_different_Users()
    {
        // Act
        EndpointResponse response = await initiateHttpCalls_With2Users(TestEndpoints.PathRequireAuth_Default);

        // Assert
        response.AssertEqual();
    }

    [Fact]
    public async Task Response_NotEqual_When_Cached_and_different_Users()
    {
        // Act
        EndpointResponse response = await initiateHttpCalls_With2Users(TestEndpoints.PathRequireAuth_VaryByUser);

        // Assert
        response.AssertNotEqual();
    }

    private async Task<EndpointResponse> initiateHttpCalls_With2Users(string urlPath)
    {
        string response1 = await _albaHost.GetAsText(urlPath);

        await Task.Delay(DefaultDelay);

        _albaHostFixture.TestUserClaims = TestUsers.User2Claims;

        string response2 = await _albaHost.GetAsText(urlPath);

        return new EndpointResponse(response1, response2);
    }
}