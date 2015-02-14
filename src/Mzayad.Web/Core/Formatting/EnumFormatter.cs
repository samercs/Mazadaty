using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Mzayad.Web.Core.Formatting
{
    public class EnumFormatter
    {
        public static string Description(Enum enumValue)
        {
            if (enumValue == null)
            {
                return "";
            }

            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            if (fieldInfo == null)
            {
                return "";
            }

            var attributes =
                (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0
                ? attributes[0].Description
                : StringFormatter.FormatCamelCase(enumValue.ToString());
        }
    }
}