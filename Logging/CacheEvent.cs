namespace Wimt.CachingFramework.Logging
{
    public enum CacheEvent
    {
        /// <summary>
        /// The item does not exist in the cache.
        /// </summary>
        CacheMiss,

        /// <summary>
        /// The item exists in and is retrieved from the cache.
        /// </summary>
        CacheHit,

        /// <summary>
        /// The item was removed from the cache during a read.  Essentially a cache miss.
        /// </summary>
        CacheRaceCondition,

        /// <summary>
        /// The current cache is disabled.  Caching ignored.
        /// </summary>
        CacheIgnored,

        /// <summary>
        /// The item has a null value and is subsequently not stored in the cache.
        /// </summary>
        CacheNull
    }
}