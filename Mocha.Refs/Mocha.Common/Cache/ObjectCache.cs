using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Common.Cache
{
    public class ObjectCache<T>: IObjectCache<T>
    {
        private string _prefix;

        private ObjectCache _Cache {
            get { return MemoryCache.Default; }
        }

        public ObjectCache()
        {
            _prefix = GetPrefix();
        }

        public ObjectCache(string name)
        {
            _prefix = name + "_" + GetPrefix();
        }

        public bool Exists(string key)
        {
            key = GetPrefix() + key;
            return _Cache[key] != null;
        }

        public T Get(string key)
        {
            key = GetPrefix() + key;
            return (T)_Cache[key];
        }

        public void Add(string key, T data, int cacheMinutes)
        {
            key = GetPrefix() + key;
            var policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(cacheMinutes);
            _Cache.Add(new CacheItem(key, data), policy);
        }

        public void Remove(string key)
        {
            key = GetPrefix() + key;
            _Cache.Remove(key);
        }

        private string GetPrefix()
        {
            if (_prefix == null)
            {
                _prefix = typeof(T).FullName;
            }
            return _prefix;
        }
    }
}

