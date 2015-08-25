using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

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

        public IReadOnlyCollection<T> GetList<T>(string key) where T : class
        {
            var value = Get<string>(key);
            if (value == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<IReadOnlyCollection<T>>(value);
        }

        public async Task<IReadOnlyCollection<T>> TryGetList<T>(string key, Func<Task<IReadOnlyCollection<T>>> getValue) where T : class
        {
            var value = GetList<T>(key);
            if (value == null)
            {
                value = await getValue();
                SetList(key, value);
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

        public void SetList<T>(string key, IEnumerable<T> list)
        {
            Trace.TraceInformation("SetList()");

            if (list == null || !list.Any())
            {
                Trace.TraceWarning("SetList(): Empty list.");
                return;
            }

            try
            {
                var json = JsonConvert.SerializeObject(list);

                Trace.TraceInformation("SetList(): " + json);

                HttpRuntime.Cache[key] = json;
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }
        }

        public async Task Delete(string key)
        {
            await Task.Run(() => HttpRuntime.Cache.Remove(key));
        }
    }
}