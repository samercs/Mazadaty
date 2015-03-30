using System;
using System.Threading.Tasks;

namespace Mzayad.Web.Core.Services
{
    public interface ICacheService
    {
        T Get<T>(string key) where T : class;
        T TryGet<T>(string key, Func<T> getValue, TimeSpan expiry) where T : class;
        Task<T> TryGet<T>(string key, Func<Task<T>> getValue, TimeSpan expiry) where T : class;
        void Set(string key, object value);
        void Set(string key, object value, TimeSpan expiry);
        Task Delete(string key);
    }
}