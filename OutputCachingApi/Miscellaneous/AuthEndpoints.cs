using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;

namespace OutputCachingApi.Miscellaneous;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth");

        group.MapGet("/login",  handleLogin);
        group.MapGet("/logout", handleLogout).RequireAuthorization();
    }

    private static SignInHttpResult handleLogin()
    {
        string randomUserName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());

        Claim[] claims = [new Claim(ClaimTypes.Name, randomUserName)];

        var claimsIdentity = new ClaimsIdentity(claims, Program.AuthScheme);

        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        return TypedResults.SignIn(claimsPrincipal, authenticationScheme: Program.AuthScheme);
    }

    private static SignOutHttpResult handleLogout()
    {
        return TypedResults.SignOut(authenticationSchemes: [Program.AuthScheme]);
    }
}
