using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mzayad.Web.Core.Services
{
    public interface ICacheService
    {
        bool Exists(string key);
        void AddToSet(string key, string value);
        void RemoveFromSet(string key, string value);
        IEnumerable<string> GetSetMembers(string key);
        
        T Get<T>(string key) where T : class;
        T TryGet<T>(string key, Func<T> getValue, TimeSpan expiry) where T : class;
        Task<T> TryGet<T>(string key, Func<Task<T>> getValue, TimeSpan expiry) where T : class;
        void Set(string key, object value);
        void Set(string key, object value, TimeSpan expiry);
        Task Delete(string key);
    }
}