using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;
using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace Mzayad.Web.Core.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly TimeSpan _expiration = TimeSpan.FromMinutes(5);

        private ConnectionMultiplexer _cacheConnection;
        protected ConnectionMultiplexer CacheConnection
        {
            get
            {
                if (_cacheConnection != null && _cacheConnection.IsConnected)
                {
                    return _cacheConnection;
                }
                
                return _cacheConnection = ConnectionMultiplexer.Connect(_connectionString);
            }
        }

        private IDatabase _cacheDatabase;
        protected IDatabase CacheDatabase
        {
            get { return _cacheDatabase ?? (_cacheDatabase = CacheConnection.GetDatabase()); }
        }

        private readonly string _connectionString;
        private readonly string _cacheKeyPrefix;
        
        public RedisCacheService(string connectionString, string cacheKeyPrefix)
        {
            _connectionString = connectionString;
            _cacheKeyPrefix = cacheKeyPrefix;
        }

        public bool Exists(string key)
        {
            key = GetKey(key);
            return CacheDatabase.KeyExists(key);
        }

        public void AddToSet(string key, string value)
        {
            key = GetKey(key);
            CacheDatabase.SetAdd(key, value);
        }

        public IEnumerable<string> GetSetMembers(string key)
        {
            key = GetKey(key);
            
            var redisValues = CacheDatabase.SetMembers(key);
            return redisValues.Select(i => i.ToString());
        }

        public void RemoveFromSet(string key, string value)
        {
            key = GetKey(key);
            CacheDatabase.SetRemove(key, value);
        }

        public T Get<T>(string key) where T : class
        {
            key = GetKey(key);
            return Deserialize<T>(CacheDatabase.StringGet(key));
        }

        public T TryGet<T>(string key, Func<T> getValue, TimeSpan expiry) where T : class
        {
            key = GetKey(key);

            var value = Get<T>(key);
            if (value == null)
            {
                value = getValue();
                Set(key, value, expiry);
            }
            else if (value is ICollection)
            {
                var collection = value as ICollection;
                if (collection.Count == 0)
                {
                    value = getValue();
                    Set(key, value, expiry);
                }
            } 

            return value;
        }

        public async Task<T> TryGet<T>(string key, Func<Task<T>> getValue, TimeSpan expiry) where T : class
        {
            key = GetKey(key);
            var value = Get<T>(key);
            if (value == null)
            {
                value = await getValue();
                Set(key, value, expiry);
            }
            else if (value is ICollection)
            {
                var collection = value as ICollection;
                if (collection.Count == 0)
                {
                    value = await getValue();
                    Set(key, value, expiry);
                }
            }

            return value;
        }

        public IReadOnlyCollection<T> GetList<T>(string key) where T : class
        {
            key = GetKey(key);
            return Deserialize<IReadOnlyCollection<T>>(CacheDatabase.StringGet(key));
        }

        public async Task<IReadOnlyCollection<T>> TryGetList<T>(string key, Func<Task<IReadOnlyCollection<T>>> getValue) where T : class
        {
            key = GetKey(key);

            var list = GetList<T>(key);
            if (list == null)
            {
                list = await getValue();
                SetList(key, list);
            }

            return list;
        }

        public void Set(string key, object value)
        {
            key = GetKey(key);
            CacheDatabase.StringSet(key, Serialize(value));
        }

        public void Set(string key, object value, TimeSpan expiry)
        {
            key = GetKey(key);
            CacheDatabase.StringSet(key, Serialize(value), expiry);
        }

        public void SetList<T>(string key, IEnumerable<T> list)
        {
            key = GetKey(key);
            CacheDatabase.StringSet(key, Serialize(list), _expiration);
        }

        public async Task Delete(string key)
        {
            key = GetKey(key);
            await CacheDatabase.KeyDeleteAsync(key);
        }

        private string GetKey(string key)
        {
            return (_cacheKeyPrefix + ":" + key).ToLowerInvariant();
        }

        private static byte[] Serialize(object o)
        {
            if (o == null)
            {
                return null;
            }
            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, o);
                var objectDataAsStream = memoryStream.ToArray();
                return objectDataAsStream;
            }
        }

        private static T Deserialize<T>(byte[] stream)
        {
            var binaryFormatter = new BinaryFormatter();
            if (stream == null)
            {
                return default(T);
            }

            using (var memoryStream = new MemoryStream(stream))
            {
                try
                {
                    return (T)binaryFormatter.Deserialize(memoryStream);
                }
                catch (SerializationException)
                {
                    return default(T);
                }
            }
        }
    }
}