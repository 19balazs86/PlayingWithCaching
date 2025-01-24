# Playing with Caching
You can find a solution for experimenting with output caching in this repository.

Output caching middleware is simple and requires only a few lines of code.
However, things become complicated when authentication comes into play.
The DefaultPolicy does not allow caching for endpoints that require authorization.

You can create a custom policy based on the built-in default, but you need to ensure the default policy is excluded, which can be easy to overlook.

I believe the solution lies in the extension method.
I created [some methods to help manage output caching](OutputCachingApi/Core/Extensions.cs) in the same way as the built-in version.
I also added an example that tags the cache with the username. This feature not present in the built-in version, but greatly needed.

## In the example the set-up is the following

- By default, caching is disabled, so you need to explicitly enable it on the desired endpoint
- Tag all cache entries with 'tag-all'
- If the endpoint requires authorization, tag it by username and with 'tag-auth'
- With the extension methods, it is easy to apply the [custom policy](OutputCachingApi/Core/DefaultAuthCachePolicy.cs) that allows caching for authenticated endpoints


## Resources

#### Output caching

- [Output caching middleware](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/output) 📚*MS-Learn*
- [Ultimate guide for Output Caching](https://youtu.be/BMXgJxSaDSo) 📽️*23 min - Milan*

#### FusionCache

- [FusionCache](https://github.com/ZiggyCreatures/FusionCache) 👤*ZiggyCreatures*
- [Introduction to FusionCache](https://youtu.be/SNoJtwiY4c4) 📽️*12 min - Nick*

#### HybridCache

- [HybridCache](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/hybrid) 📚*MS-Learn*
- [HybridCache](https://www.milanjovanovic.tech/blog/hybrid-cache-in-aspnetcore-new-caching-library) 📓*Milan's newsletter*
- [Caching](https://www.milanjovanovic.tech/blog/caching-in-aspnetcore-improving-application-performance) *(Memory, Distributed, Hybrid)* 📓*Milan's newsletter*
- [Caching](https://youtu.be/7DSNFwsYR8E) *(Memory, Distributed, Hybrid)* 📽️*10 min - Coding Shorts*
