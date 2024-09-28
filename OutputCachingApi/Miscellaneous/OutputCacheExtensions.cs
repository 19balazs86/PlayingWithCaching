using Microsoft.AspNetCore.OutputCaching;

namespace OutputCachingApi.Miscellaneous;

public static class OutputCacheExtensions
{
    public static TBuilder CacheOutputAuth<TBuilder>(this TBuilder builder) where TBuilder : IEndpointConventionBuilder
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Enable caching if this method is invoked on an endpoint, extra policies can disable it

        builder.Add(endpointBuilder => endpointBuilder.Metadata.Add(AuthCachePolicy.Instance));

        return builder;
    }

    public static TBuilder CacheOutputAuth<TBuilder>(this TBuilder builder, Action<OutputCachePolicyBuilder> builderAction)
        where TBuilder : IEndpointConventionBuilder
    {
        Action<OutputCachePolicyBuilder> innerAction = policyBuilder =>
        {
            policyBuilder.AddAuthCachePolicy();

            builderAction(policyBuilder);
        };

        return builder.CacheOutput(builderAction, excludeDefaultPolicy: true);
    }

    public static void AddPolicyAuth(this OutputCacheOptions cacheOptions, string name, Action<OutputCachePolicyBuilder> builderAction)
    {
        cacheOptions.AddPolicy(name, builder =>
        {
            builder.AddAuthCachePolicy();

            builderAction(builder);
        }, excludeDefaultPolicy: true);
    }

    public static OutputCachePolicyBuilder AddAuthCachePolicy(this OutputCachePolicyBuilder builder)
    {
        return builder.AddPolicy<AuthCachePolicy>();
    }
}
