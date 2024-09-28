using Microsoft.AspNetCore.Http;

namespace ApiTests;

[Collection(nameof(SharedCollectionFixture))]
public sealed class AnonymousTests : CommonTests
{
    public AnonymousTests(AlbaHostFixture albaHostFixture) : base(albaHostFixture)
    {
        _albaHostFixture.TestUserClaims = TestUsers.AnonymousClaims;
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
}