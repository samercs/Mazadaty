using System;

namespace Mazadaty.Web.Core.Services
{
    public interface ICookieService
    {
        string Get(string name);
        bool TryGetBool(string name, bool defaultValue = false);
        void Add(string name, object value, DateTime? expiration = null, string path = null);
        void Add(string name, string value, DateTime? expiration = null, string path = null);
    }
}
