using Newtonsoft.Json;
using OrangeJetpack.Base.Web.Caching;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mzayad.Web.Core.Caching
{
    public class LocalRedisCacheService : ICacheService
    {
        private const string ConnectionString = "localhost";

        private IDatabase _cacheDatabase;
        private readonly TimeSpan _expiration = TimeSpan.FromMinutes(60);

        private static readonly Lazy<ConnectionMultiplexer> LazyConnection =
            new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(ConnectionString));

        protected static ConnectionMultiplexer CacheConnection => LazyConnection.Value;
        protected IDatabase CacheDatabase => _cacheDatabase ?? (_cacheDatabase = CacheConnection.GetDatabase());

        /// <summary>
        /// Initializes RedisCacheService with default expiration (5 minutes).
        /// </summary>
        public LocalRedisCacheService()
        {
        }

        /// <summary>
        /// Initializes RedisCacheService with specified expiration.
        /// </summary>
        public LocalRedisCacheService(TimeSpan defaultExpiration)
        {
            _expiration = defaultExpiration;
        }

        public T Get<T>(string key) where T : class
        {
            key = GetKey(key);

            try
            {
                return Deserialize<T>(CacheDatabase.StringGet(key));
            }
            catch (TimeoutException)
            {
                return null;
            }
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
            key = GetKey(key);

            try
            {
                return Deserialize<IReadOnlyCollection<T>>(CacheDatabase.StringGet(key));
            }
            catch (TimeoutException)
            {
                return null;
            }
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
            SetList(key, list, expiry);

            return list;
        }

        public void Set(string key, object value)
        {
            Set(key, value, _expiration);
        }

        public void Set(string key, object value, TimeSpan expiry)
        {
            key = GetKey(key);
            try
            {
                CacheDatabase.StringSet(key, Serialize(value), expiry);
            }
            catch (TimeoutException)
            {
                // do nothing
            }
        }

        public void SetList<T>(string key, IEnumerable<T> list)
        {
            SetList(key, list, _expiration);
        }

        public void SetList<T>(string key, IEnumerable<T> list, TimeSpan expiry)
        {
            key = GetKey(key);
            try
            {
                CacheDatabase.StringSet(key, Serialize(list), expiry);
            }
            catch (TimeoutException)
            {
                // do nothing
            }
        }

        private static string GetKey(string key)
        {
            return ("mzayad:" + key).ToLowerInvariant();
        }

        private static string Serialize(object o)
        {
            return o == null ? null : JsonConvert.SerializeObject(o);
        }

        private static T Deserialize<T>(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return default(T);
            }

            return JsonConvert.DeserializeObject<T>(value);
        }

        public void Delete(string key)
        {
            CacheDatabase.KeyDelete(key);
        }
    }
}