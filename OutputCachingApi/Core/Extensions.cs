using Microsoft.AspNetCore.OutputCaching;
using System.Runtime.CompilerServices;

namespace OutputCachingApi.Core;

public static class EndpointConvention_Extensions
{
    public static TBuilder CacheOutputAuth<TBuilder>(this TBuilder builder) where TBuilder : IEndpointConventionBuilder
    {
        return builder.WithMetadata(DefaultAuthCachePolicy.Instance);
    }

    public static TBuilder CacheOutputAuth<TBuilder>(this TBuilder builder, Action<OutputCachePolicyBuilder> builderAction)
        where TBuilder : IEndpointConventionBuilder
    {
        Action<OutputCachePolicyBuilder> innerAction = policyBuilder =>
        {
            policyBuilder.AddDefaultAuthPolicy();

            builderAction(policyBuilder);
        };

        return builder.CacheOutput(innerAction, excludeDefaultPolicy: true);
    }
}

public static class OutputCacheOptions_Extensions
{
    public static void AddPolicyAuth(this OutputCacheOptions cacheOptions, string name, Action<OutputCachePolicyBuilder> builderAction)
    {
        cacheOptions.AddPolicy(name, builder =>
        {
            builder.AddDefaultAuthPolicy();

            builderAction(builder);
        }, excludeDefaultPolicy: true);
    }
}

public static class OutputCachePolicyBuilder_Extensions
{
    public static OutputCachePolicyBuilder AddDefaultAuthPolicy(this OutputCachePolicyBuilder builder)
    {
        return builder.AddPolicy<DefaultAuthCachePolicy>();
    }

    public static OutputCachePolicyBuilder VaryByUserName(this OutputCachePolicyBuilder builder, string key = "UserName")
    {
        return builder.VaryByValue(httpContext => new KeyValuePair<string, string>(key, httpContext.User.Identity?.Name ?? "Anonymous"));
    }

    public static OutputCachePolicyBuilder TagByUserName(this OutputCachePolicyBuilder builder)
    {
        return builder.TagByUserName(userName => [userName]);
    }

    public static OutputCachePolicyBuilder TagByUserName(this OutputCachePolicyBuilder builder, Func<string, string[]> tagsFunc)
    {
        var policy = new TagByUserNameCachePolicy(tagsFunc);

        return builder.AddCustomPolicy(policy);
    }

    // OutputCachePolicyBuilder.AddPolicy(IOutputCachePolicy policy) method is internal
    // Dev Leader video #1: https://youtu.be/MCe-q5y59Oc
    // Dev Leader video #2: https://youtu.be/jp-PAViwbgg
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "AddPolicy")]
    public extern static OutputCachePolicyBuilder AddCustomPolicy(this OutputCachePolicyBuilder builder, IOutputCachePolicy policy);
}

public static class Extra_Extensions
{
    public static TimeSpan Seconds(this int seconds)
    {
        return TimeSpan.FromSeconds(seconds);
    }
}
