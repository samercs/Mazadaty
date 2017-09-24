using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mazadaty.Services.Caching
{
    public interface ICachService
    {
        T Get<T>(string key) where T : class;
        T TryGet<T>(string key, Func<T> getValue) where T : class;
        T TryGet<T>(string key, Func<T> getValue, TimeSpan expiry) where T : class;
        Task<T> TryGetAsync<T>(string key, Func<Task<T>> getValue) where T : class;
        Task<T> TryGetAsync<T>(string key, Func<Task<T>> getValue, TimeSpan expiry) where T : class;
        IReadOnlyCollection<T> GetList<T>(string key) where T : class;
        IReadOnlyCollection<T> TryGetList<T>(string key, Func<IReadOnlyCollection<T>> getValue) where T : class;
        IReadOnlyCollection<T> TryGetList<T>(string key, Func<IReadOnlyCollection<T>> getValue, TimeSpan expiry) where T : class;
        Task<IReadOnlyCollection<T>> TryGetListAsync<T>(string key, Func<Task<IReadOnlyCollection<T>>> getValue) where T : class;
        Task<IReadOnlyCollection<T>> TryGetListAsync<T>(string key, Func<Task<IReadOnlyCollection<T>>> getValue, TimeSpan expiry) where T : class;
        void Set(string key, object value);
        void Set(string key, object value, TimeSpan expiry);
        void SetList<T>(string key, IEnumerable<T> list);
        void SetList<T>(string key, IEnumerable<T> list, TimeSpan expiry);
        void Delete(string key);
    }
}