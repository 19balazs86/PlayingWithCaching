namespace ApiTests;

[Collection(nameof(SharedCollectionFixture))]
public sealed class EvictCacheTests(AlbaHostFixture albaHostFixture) : EndpointTestBase(albaHostFixture)
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Response_NotEqual_When_Evict_Cache(bool isAnonymous)
    {
        // Arrange
        _albaHostFixture.TestUserClaims = isAnonymous ? TestUsers.AnonymousClaims : TestUsers.User1Claims;

        // Act
        EndpointResponse response = await initiateHttpCalls_EvictCache(TestEndpoints.PathDefault);

        // Assert
        response.AssertNotEqual();
    }

    [Theory]
    [InlineData("tag-all")]
    [InlineData("tag-auth")]
    [InlineData(TestUsers.User1Name)]
    public async Task Response_NotEqual_When_Evict_Cache_on_auth_default(string tagName)
    {
        // Arrange
        _albaHostFixture.TestUserClaims = TestUsers.User1Claims;

        // Act
        EndpointResponse response = await initiateHttpCalls_EvictCache(TestEndpoints.PathRequireAuth_Default, tagName);

        // Assert
        response.AssertNotEqual();
    }

    [Theory]
    [InlineData("tag-all")]
    [InlineData("tag-auth")]
    [InlineData(TestUsers.User1Name)]
    [InlineData($"vary-endpoint-{TestUsers.User1Name}")]
    public async Task Response_NotEqual_When_Evict_Cache_on_VaryByUser(string tagName)
    {
        // Arrange
        _albaHostFixture.TestUserClaims = TestUsers.User1Claims;

        // Act
        EndpointResponse response = await initiateHttpCalls_EvictCache(TestEndpoints.PathRequireAuth_VaryByUser, tagName);

        // Assert
        response.AssertNotEqual();
    }

    private async Task<EndpointResponse> initiateHttpCalls_EvictCache(string urlPath, string tagName = "tag-all")
    {
        string response1 = await _albaHost.GetAsText(urlPath);

        await _albaHost.GetAsText($"/evict/{tagName}");

        await Task.Delay(DefaultDelay);

        string response2 = await _albaHost.GetAsText(urlPath);

        return new EndpointResponse(response1, response2);
    }
}