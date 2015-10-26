using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mzayad.Web.Core.Caching
{
    public interface ICacheService
    {
        //bool Exists(string key);
        //void AddToSet(string key, string value);
        //void RemoveFromSet(string key, string value);
        //IEnumerable<string> GetSetMembers(string key);
        
        T Get<T>(string key) where T : class;
        //T TryGet<T>(string key, Func<T> getValue, TimeSpan expiry) where T : class;
        Task<T> TryGet<T>(string key, Func<Task<T>> getValue) where T : class;
        IReadOnlyCollection<T> GetList<T>(string key) where T : class;
        Task<IReadOnlyCollection<T>> TryGetList<T>(string key, Func<Task<IReadOnlyCollection<T>>> getValue) where T : class;

        void Set(string key, object value);
        void SetList<T>(string key, IEnumerable<T> list);

        void Delete(string key);
    }
}