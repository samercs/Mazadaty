using System;
using System.Text;

namespace Mazadaty.Services.Trophies
{
    public static class EnumExtensions
    {
        public static string Name(this Enum value)
        {
            var output = new StringBuilder("");
            foreach (var c in value.ToString().ToCharArray())
            {
                if (char.IsUpper(c))
                {
                    output.Append(" ");
                }
                output.Append(c);
            }
            return output.ToString();
        }
    }
}
