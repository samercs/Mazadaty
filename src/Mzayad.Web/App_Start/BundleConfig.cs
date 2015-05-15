using System.Web.Optimization;

namespace Mzayad.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.UseCdn = true;

            var jquery = new ScriptBundle("~/js/jquery", 
                "//ajax.googleapis.com/ajax/libs/jquery/2.1.3/jquery.min.js").Include(
                "~/scripts/jquery-{version}.js");
            jquery.CdnFallbackExpression = "window.jQuery";
            bundles.Add(jquery);

            var jqueryui = new ScriptBundle("~/js/jquery-ui",
                "//ajax.googleapis.com/ajax/libs/jqueryui/1.11.2/jquery-ui.min.js").Include(
                "~/scripts/jquery-ui-{version}.js");
            jqueryui.CdnFallbackExpression = "window.jQuery.ui";
            bundles.Add(jqueryui);

            var knockout = new ScriptBundle("~/js/knockout",
                "//ajax.aspnetcdn.com/ajax/knockout/knockout-3.3.0.js").Include(
                "~/scripts/knockout-{version}.js");
            knockout.CdnFallbackExpression = "window.ko";
            bundles.Add(knockout);

            var signalR = new ScriptBundle("~/js/signalr",
                "//ajax.aspnetcdn.com/ajax/signalr/jquery.signalr-2.2.0.min.js").Include(
                "~/scripts/jquery.signalR-{version}.js");
            signalR.CdnFallbackExpression = "window.signalR";
            bundles.Add(signalR);

            bundles.Add(new ScriptBundle("~/js/site").Include(
                "~/scripts/bootstrap.js",
                "~/scripts/moment.js",
                "~/scripts/wow.js",
                "~/scripts/site.js"));

            bundles.Add(new ScriptBundle("~/js/validate").Include("~/scripts/jquery.validate*"));
            bundles.Add(new ScriptBundle("~/js/slugify").Include("~/scripts/jquery.slugify*"));

            bundles.Add(new StyleBundle("~/css").Include(
                "~/content/bootstrap.css",
                "~/content/bootstrap-theme.css",
                "~/content/font-awesome.css",
                "~/content/animate.css",
                "~/content/site-forms.css",
                "~/content/site-layout.css",
                "~/content/site-arabic.css",
                "~/content/site-home.css",
                "~/content/site.css"));

            bundles.Add(new StyleBundle("~/css/admin").Include(
                "~/content/kendo/kendo.common.css",
                "~/content/kendo/kendo.css",
                "~/content/site-kendo.css",
                "~/content/site-admin.css"));

            bundles.Add(new ScriptBundle("~/js/image-uploader").Include("~/scripts/dropzone.js", "~/scripts/image-uploader.js"));
            bundles.Add(new StyleBundle("~/css/image-uploader").Include("~/content/image-uploader.css"));
        }
    }
}
