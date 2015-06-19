using System;

namespace Wimt.CachingFramework
{
    [Serializable]
    public abstract class BaseCache : IDisposable
    {
        public string Name { get; private set; }

        public bool IsEnabled { get; private set; }

        public BaseCache(string name, bool isEnabled)
        {
            Name = name;
            IsEnabled = isEnabled;
        }

        /// <summary>
        /// Insert or update a cache value
        /// </summary>
        public abstract void Set(string key, object value);

        /// <summary>
        /// Insert or update a cache value with an expiry date
        /// </summary>
        public abstract void Set(string key, object value, DateTime expiresAt);

        /// <summary>
        /// Insert or update a cache value with a fixed lifetime
        /// </summary>
        public abstract void Set(string key, object value, TimeSpan validFor);

        /// <summary>
        /// Retrieve a value from cache
        /// </summary>
        /// <returns>Cached value or null</returns>
        public abstract object Get(string key);

        /// <summary>
        /// Removes the value for the given key from the cache
        /// </summary>
        public abstract void Clear();

        /// <summary>
        /// Clears all items from the cache
        /// </summary>
        public abstract void Remove(string key);

        /// <summary>
        /// Returns whether the cache contains a value for the given key
        /// </summary>
        public abstract bool Exists(string key);

        public abstract void Dispose();
    }
}