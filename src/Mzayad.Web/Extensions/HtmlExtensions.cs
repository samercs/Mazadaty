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

        public static Css Css(this HtmlHelper htmlHelper)
        {
            return new Css(htmlHelper);
        }
    }


    public class Css
    {
        private readonly HtmlHelper _htmlHelper;

        public Css()
        {

        }

        public Css(HtmlHelper htmlHelper)
        {
            _htmlHelper = htmlHelper;
        }

        public string GridTable = "table table-bordered table-hover table-striped";
        public string LayoutTable = "table";
        public string FormWrapper = "col-lg-8 col-lg-offset-2 col-md-10 col-md-offset-1 col-sm-12";
        public string FormNarrow = "col-md-6 col-md-offset-3 col-sm-8 col-sm-offset-2";
        public string FormWide = "col-xs-12";
    }

}