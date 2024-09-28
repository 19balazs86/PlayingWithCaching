namespace ApiTests;

// These test cases can be run for both anonymous and authenticated users
public abstract class CommonTests(AlbaHostFixture albaHostFixture) : EndpointTestBase(albaHostFixture)
{
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
}