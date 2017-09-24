using System.Collections.Generic;

namespace Mazadaty.Web.Extensions
{
    public static class StringExtensions
    {
        public static string Replace(this string str, Dictionary<string, string> dictionary)
        {
            foreach (var item in dictionary)
            {
                str = str.Replace(item.Key, item.Value);
            }

            return str;
        }
    }
}
