using System;
using System.Diagnostics;

namespace Wimt.CachingFramework.Logging
{
    public class DebugCacheLogger : ICacheLogger
    {
        public void LogInfo(string message)
        {
            Debug.WriteLine(String.Format("INFO. {0}.", message));
        }

        public void LogInfo(string cacheName, string message)
        {
            Debug.WriteLine(String.Format("INFO. Cache: {0}. {1}.", cacheName, message));
        }

        public void LogCacheEvent(string cacheName, CacheEvent cacheEvent, int elapsedMilliseconds, string key)
        {
            Debug.WriteLine(String.Format("EVENT. Cache: {0}. Event: {1}. ElapsedMilliseconds: {2}. Key: {3}.", cacheName, cacheEvent.ToString(), elapsedMilliseconds, key));
        }

        public void LogWarning(string cacheName, string message)
        {
            Debug.WriteLine(String.Format("WARNING. Cache: {0}. {1}.", cacheName, message));
        }

        public void LogFatal(string cacheName, string message)
        {
            Debug.WriteLine(String.Format("FATAL. Cache: {0}. {1}.", cacheName, message));
        }

        public void LogFatal(string cacheName, string message, Exception exception)
        {
            Debug.WriteLine(String.Format("FATAL. Cache: {0}. {1}. {2}", cacheName, message, exception.ToString()));
        }
    }
}