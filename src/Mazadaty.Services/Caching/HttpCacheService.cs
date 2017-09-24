using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace Mazadaty.Services.Caching
{
    public class HttpCacheService: ICachService
    {
        private readonly TimeSpan _expiration = TimeSpan.FromMinutes(5);

        public T Get<T>(string key) where T : class
        {
            return HttpRuntime.Cache[key] as T;
        }

        public T TryGet<T>(string key, Func<T> getValue) where T : class
        {
            var item = Get<T>(key);
            if (item != null)
            {
                return item;
            }

            item = getValue();
            Set(key, item);

            return item;
        }

        public T TryGet<T>(string key, Func<T> getValue, TimeSpan expiry) where T : class
        {
            var item = Get<T>(key);
            if (item != null)
            {
                return item;
            }

            item = getValue();
            Set(key, item, expiry);

            return item;
        }

        public async Task<T> TryGetAsync<T>(string key, Func<Task<T>> getValue) where T : class
        {
            var item = Get<T>(key);
            if (item != null)
            {
                return item;
            }

            item = await getValue();
            Set(key, item);

            return item;
        }

        public async Task<T> TryGetAsync<T>(string key, Func<Task<T>> getValue, TimeSpan expiry) where T : class
        {
            var item = Get<T>(key);
            if (item != null)
            {
                return item;
            }

            item = await getValue();
            Set(key, item, expiry);

            return item;
        }

        public IReadOnlyCollection<T> GetList<T>(string key) where T : class
        {
            return HttpRuntime.Cache[key] as IReadOnlyCollection<T>;
        }

        public IReadOnlyCollection<T> TryGetList<T>(string key, Func<IReadOnlyCollection<T>> getValue) where T : class
        {
            return TryGetList(key, getValue, _expiration);
        }

        public IReadOnlyCollection<T> TryGetList<T>(string key, Func<IReadOnlyCollection<T>> getValue, TimeSpan expiry) where T : class
        {
            var list = GetList<T>(key);
            if (list != null)
            {
                return list;
            }

            list = getValue();
            SetList(key, list, expiry);

            return list;
        }

        public async Task<IReadOnlyCollection<T>> TryGetListAsync<T>(string key, Func<Task<IReadOnlyCollection<T>>> getValue) where T : class
        {
            return await TryGetListAsync(key, getValue, _expiration);
        }

        public async Task<IReadOnlyCollection<T>> TryGetListAsync<T>(string key, Func<Task<IReadOnlyCollection<T>>> getValue, TimeSpan expiry) where T : class
        {
            var list = GetList<T>(key);
            if (list != null)
            {
                return list;
            }

            list = await getValue();
            SetList(key, list);

            return list;
        }

        public void Set(string key, object value)
        {
            Set(key, value, _expiration);
        }

        public void Set(string key, object value, TimeSpan expiry)
        {
            HttpRuntime.Cache.Insert(key, value, null, DateTime.UtcNow.Add(expiry), TimeSpan.Zero, CacheItemPriority.Normal, null);
        }

        public void SetList<T>(string key, IEnumerable<T> list)
        {
            Set(key, list, _expiration);
        }

        public void SetList<T>(string key, IEnumerable<T> list, TimeSpan expiry)
        {
            Set(key, list, expiry);
        }

        public void Delete(string key)
        {
            HttpRuntime.Cache.Remove(key);
        }
    }
}