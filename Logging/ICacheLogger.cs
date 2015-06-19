using System;

namespace Wimt.CachingFramework.Logging
{
    public interface ICacheLogger
    {
        void LogInfo(string message);

        void LogInfo(string cacheName, string message);

        void LogCacheEvent(string cacheName, CacheEvent cacheEvent, int elapsedMilliseconds, string key);

        void LogWarning(string cacheName, string message);

        void LogFatal(string cacheName, string message);

        void LogFatal(string cacheName, string message, Exception exception);
    }
}