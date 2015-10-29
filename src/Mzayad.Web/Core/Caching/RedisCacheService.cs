using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Mzayad.Web.Core.Caching
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

        public T Get<T>(string key) where T : class
        {
            return Deserialize<T>(CacheDatabase.StringGet(key));
        }

        public async Task<T> TryGet<T>(string key, Func<Task<T>> getValue) where T : class
        {
            key = GetKey(key);
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
            try
            {
                return Deserialize<IReadOnlyCollection<T>>(CacheDatabase.StringGet(key));
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                return null;
            }
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
            CacheDatabase.StringSet(key, Serialize(value), _expiration);
        }

        public void SetList<T>(string key, IEnumerable<T> list)
        {
            key = GetKey(key);

            try
            {         
                CacheDatabase.StringSet(key, Serialize(list), _expiration);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }
        }

        public void Delete(string key)
        {
            key = GetKey(key);

            CacheDatabase.KeyDelete(key);
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