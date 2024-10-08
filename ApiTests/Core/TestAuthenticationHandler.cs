﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace ApiTests.Core;

public static class TestAuthenticationExtensions
{
    public static IServiceCollection AddTestAuthentication(this IServiceCollection services, Func<IEnumerable<Claim>> testUserClaimsFunc)
    {
        return services.AddTestAuthentication(options => options.TestUserClaimsFunc = testUserClaimsFunc);
    }

    public static IServiceCollection AddTestAuthentication(this IServiceCollection services, Action<TestAuthenticationOptions> configureOptions)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = Consts.AuthScheme;
            options.DefaultChallengeScheme    = Consts.AuthScheme;
        })
        .addTestAuthenticationScheme(configureOptions);

        return services;
    }

    private static AuthenticationBuilder addTestAuthenticationScheme(this AuthenticationBuilder builder, Action<TestAuthenticationOptions> configureOptions)
    {
        return builder.AddScheme<TestAuthenticationOptions, TestAuthenticationHandler>(Consts.AuthScheme, configureOptions);
    }
}

public sealed class TestAuthenticationHandler : AuthenticationHandler<TestAuthenticationOptions>
{
    public TestAuthenticationHandler(
        IOptionsMonitor<TestAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder) : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // You have access to the following properties: Request, Context
        // Like in this solution: https://youtu.be/jcid26MuhX8 | But I prefer my workaround.

        ClaimsIdentity? claimsIdentity = Options.Identity();

        if (claimsIdentity is null)
            return Task.FromResult(AuthenticateResult.NoResult());

        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var authenticationTicket = new AuthenticationTicket(claimsPrincipal, Consts.AuthScheme);

        return Task.FromResult(AuthenticateResult.Success(authenticationTicket));
    }
}

public sealed class TestAuthenticationOptions : AuthenticationSchemeOptions
{
    public Func<IEnumerable<Claim>> TestUserClaimsFunc { get; set; } = () => [];

    public ClaimsIdentity? Identity()
    {
        IEnumerable<Claim> claims = TestUserClaimsFunc.Invoke();

        if (claims.Any())
        {
            return new ClaimsIdentity(claims, Consts.AuthScheme);
        }

        return null;
    }
}

file static class Consts
{
    public const string AuthScheme = "TestAuthScheme";
}
