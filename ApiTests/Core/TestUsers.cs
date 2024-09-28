using System.Collections.Immutable;
using System.Security.Claims;

namespace ApiTests.Core;

public static class TestUsers
{
    public const string User1Name = "User1";
    public const string User2Name = "User2";

    public static readonly ImmutableArray<Claim> AnonymousClaims = [];
    public static readonly ImmutableArray<Claim> User1Claims     = [new Claim(ClaimTypes.Name, User1Name)];
    public static readonly ImmutableArray<Claim> User2Claims     = [new Claim(ClaimTypes.Name, User2Name)];
}
