using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Resources;
using System.Web;
using Mzayad.Web.Core.Formatting;

namespace Mzayad.Web.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum enumValue)
        {
            return EnumFormatter.Description(enumValue);
        }

        public static string GetDescription(this Enum enumValue, ResourceManager resourceManager)
        {
            var fi = enumValue.GetType().GetField(enumValue.ToString());

            var attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (!attributes.Any())
            {
                return enumValue.ToString();
            }

            return resourceManager.GetString(attributes[0].Description);
        }
    }
}