using System.Web.Mvc;
using OrangeJetpack.Base.Web;

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

        public static MvcHtmlString StatusMessage(this HtmlHelper htmlHelper, StatusMessage statusMessage)
        {
            var div = new TagBuilder("div");
            div.AddCssClass(statusMessage.GetCssClass());
            div.InnerHtml += GetStatusMessageIcon(statusMessage.StatusMessageType);
            div.InnerHtml += "<span>" + statusMessage.Message + "</span>";
            div.InnerHtml += "<div class='clearfix'></div>";

            return MvcHtmlString.Create(div.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString StatusMessage(this HtmlHelper htmlHelper, string message,
            StatusMessageType statusMessageType = StatusMessageType.Success,
            StatusMessageFormat statusMessageFormat = StatusMessageFormat.Normal)
        {
            var statusMessage = new StatusMessage(message, statusMessageType, statusMessageFormat, false);
            return StatusMessage(htmlHelper, statusMessage);
        }

        private static string GetStatusMessageIcon(StatusMessageType statusMessageType)
        {
            var icon = new TagBuilder("i");
            icon.AddCssClass("fa fa-3x pull-left");
            switch (statusMessageType)
            {
                case StatusMessageType.Success:
                    icon.AddCssClass("fa-check-square-o");
                    break;
                case StatusMessageType.Information:
                    icon.AddCssClass("fa-question-circle");
                    break;
                case StatusMessageType.Warning:
                    icon.AddCssClass("fa-warning");
                    break;
                case StatusMessageType.Error:
                    icon.AddCssClass("fa-exclamation-triangle");
                    break;
            }
            return icon.ToString();
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