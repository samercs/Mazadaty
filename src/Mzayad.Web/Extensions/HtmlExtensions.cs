using System.Web.Mvc;

namespace Mzayad.Web.Extensions
{
    public static class HtmlExtensions
    {
        public static string LanguageCode(this HtmlHelper htmlHelper)
        {
            var languagCode = htmlHelper.ViewBag.LanguageCode ?? "en";

            return languagCode.ToString().ToLowerInvariant();
        }

        public static bool IsArabic(this HtmlHelper htmlHelper)
        {
            return htmlHelper.LanguageCode().Equals("ar");
        }
    }
}