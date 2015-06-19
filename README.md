# NgPakshare3

We have built a caching framework to use across projects.  The framework encapsulates a lot of complexity (especially with regards to Azure colocation).

It is available on our private NuGet repo:

WimtCachingFramework1Nuget.png

Wimt.CachingFramework is configured in your projects' web.config using the <cachingFramework> section. Example:

WimtCachingFramework2Config.png

In the above example, two caches have been set up:

    An Azure in-role, co-located cache (named ScheduledData), and
    An in-memory (local RAM) cache (named ContextData).

You can set up many different types of caches with different initialisation parameters.  Any names can be used, as long as they are unique.  E.g.  the ScheduledData cache needs to handle large sets of data and must not expire.

Right now, there are only two types of caches that can be created: 

    inMemoryCache:  Caches data to the local memory of the machine (not shared across roles).  Good for rapid access, short-term data, especially during a single http session (e.g. a repository/service query that needs to occur twice within the same http session for some reason)
    inRoleCache:  Caches data using Azure colocation (data is cached and shared across all roles).  Good for caching which isn't context specific (e.g.  all active stops)

Caching a the return data from a service or repository method can be done using the CacheAttribute, like this:​

WimtCachingFramework3Attribute.png

This will use the ContextData cache (as set up in the config) and will have a lifespan of 5 minutes.

Basic class design for those interested:

Wimt.CachingFramework4.png
