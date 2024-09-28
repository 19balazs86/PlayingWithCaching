using System.Collections.Immutable;
using System.Security.Claims;

namespace ApiTests.Core;

public static class TestUsers
{
    public static readonly ImmutableArray<Claim> AnonimousClaims = [];
    public static readonly ImmutableArray<Claim> User1Claims     = [new Claim(ClaimTypes.Name, "User1")];
    public static readonly ImmutableArray<Claim> User2Claims     = [new Claim(ClaimTypes.Name, "User2")];
}
