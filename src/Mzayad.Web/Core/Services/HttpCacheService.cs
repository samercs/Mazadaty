using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace Mzayad.Web.Core.Services
{
    public class HttpCacheService : ICacheService
    {
        public T Get<T>(string key) where T : class
        {         
            return HttpRuntime.Cache[key] as T;
        }

        public async Task<T> TryGet<T>(string key, Func<Task<T>> getValue) where T : class
        {
            var value = Get<T>(key);
            if (value == null)
            {
                value = await getValue();
                Set(key, value);
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

        public void SetList<T>(string key, IEnumerable<T> list)
        {
            var json = JsonConvert.SerializeObject(list);

            HttpRuntime.Cache[key] = json;
        }
    }
}