namespace ApiTests;

[Collection(nameof(SharedCollectionFixture))]
public sealed class AuthenticatedTests : CommonTests
{
    public AuthenticatedTests(AlbaHostFixture albaHostFixture) : base(albaHostFixture)
    {
        _albaHostFixture.TestUserClaims = TestUsers.User1Claims;
    }

    [Fact]
    public async Task Response_Equal_When_Cached_on_Auth()
    {
        // Act
        EndpointResponse response = await initiateHttpCalls(TestEndpoints.PathRequireAuth);

        // Assert
        Assert.Equal(response.ResponseText1, response.ResponseText2);
    }

    [Fact]
    public async Task Response_NotEqual_When_Cache_Expired_on_Auth()
    {
        // Arrange
        TimeSpan delay = TimeSpan.FromSeconds(AlbaHostFixture.CacheSettings.DefaultExpiration + 1);

        // Act
        EndpointResponse response = await initiateHttpCalls(TestEndpoints.PathRequireAuth, delay);

        // Assert
        Assert.NotEqual(response.ResponseText1, response.ResponseText2);
    }
}