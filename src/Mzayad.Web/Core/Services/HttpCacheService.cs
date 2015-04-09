using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace Mzayad.Web.Core.Services
{
    public class HttpCacheService : ICacheService
    {
        public bool Exists(string key)
        {
            return HttpRuntime.Cache.Get(key) != null;
        }

        public void AddToSet(string key, string value)
        {
            var items = (HttpRuntime.Cache.Get(key) as Dictionary<string, string>) ?? new Dictionary<string, string>();

            if (items.ContainsKey(key))
            {
                items[key] = value;
            }
            else
            {
                items.Add(key, value);
            }

            Set(key, items);
        }

        public IEnumerable<string> GetSetMembers(string key)
        {
            var items = (HttpRuntime.Cache.Get(key) as Dictionary<string, string>) ?? new Dictionary<string, string>();

            return items.Values;
        }

        public void RemoveFromSet(string key, string value)
        {
            var items = (HttpRuntime.Cache.Get(key) as Dictionary<string, string>) ?? new Dictionary<string, string>();
            if (items.ContainsKey(key))
            {
                items.Remove(key);
            }
        }

        public T Get<T>(string key) where T : class
        {         
            return HttpRuntime.Cache[key] as T;
        }

        public T TryGet<T>(string key, Func<T> getValue, TimeSpan expiry) where T : class
        {
            var value = Get<T>(key);
            if (value == null)
            {
                value = getValue();
                Set(key, value, expiry);
            }

            return value;
        }

        public async Task<T> TryGet<T>(string key, Func<Task<T>> getValue, TimeSpan expiry) where T : class
        {
            var value = Get<T>(key);
            if (value == null)
            {
                value = await getValue();
                Set(key, value, expiry);
            }

            return value;
        }

        public void Set(string key, object value)
        {
            HttpRuntime.Cache[key] = value;
        }

        public void Set(string key, object value, TimeSpan expiry)
        {
            HttpRuntime.Cache[key] = value;
        }

        public async Task Delete(string key)
        {
            await Task.Run(() => HttpRuntime.Cache.Remove(key));
        }
    }
}