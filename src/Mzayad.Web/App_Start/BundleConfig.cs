using System.Web.Optimization;

namespace Mzayad.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = true;

            bundles.UseCdn = true;

            var jquery = new ScriptBundle("~/js/jquery", "https://cdnjs.cloudflare.com/ajax/libs/jquery/2.1.4/jquery.min.js").Include(
                "~/scripts/jquery-{version}.js");
            jquery.CdnFallbackExpression = "window.jQuery";
            bundles.Add(jquery);

            var jqueryui = new ScriptBundle("~/js/jquery-ui", "https://cdnjs.cloudflare.com/ajax/libs/jqueryui/1.11.4/jquery-ui.min.js").Include(
                "~/scripts/jquery-ui-{version}.js");
            jqueryui.CdnFallbackExpression = "window.jQuery.ui";
            bundles.Add(jqueryui);

            var bootstrap = new ScriptBundle("~/js/bootstrap", "https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.3.5/js/bootstrap.min.js").Include(
                "~/scripts/bootstrap.js");
            bootstrap.CdnFallbackExpression = "$.fn.modal";
            bundles.Add(bootstrap);

            var knockout = new ScriptBundle("~/js/knockout",
                "https://cdnjs.cloudflare.com/ajax/libs/knockout/3.4.0/knockout-min.js").Include(
                "~/scripts/knockout-{version}.js");
            knockout.CdnFallbackExpression = "window.ko";
            bundles.Add(knockout);

            var signalR = new ScriptBundle("~/js/signalr",
                "https://cdnjs.cloudflare.com/ajax/libs/signalr.js/2.2.1/jquery.signalR.min.js").Include(
                "~/scripts/jquery.signalR-{version}.js");
            signalR.CdnFallbackExpression = "window.signalR";
            bundles.Add(signalR);

            bundles.Add(new ScriptBundle("~/js/site").Include(
                "~/scripts/moment.js",
                "~/scripts/site.js",
                "~/scripts/site-kendo.js"));

            bundles.Add(new ScriptBundle("~/js/nivo").Include("~/scripts/nivo/nivo-lightbox.min.js"));
            bundles.Add(new ScriptBundle("~/js/validate").Include("~/scripts/jquery.validate*"));
            bundles.Add(new ScriptBundle("~/js/slugify").Include("~/scripts/jquery.slugify*"));

            bundles.Add(new StyleBundle("~/css").Include("~/content/site.css"));

            bundles.Add(new StyleBundle("~/css/admin").Include(
                "~/content/kendo/kendo.common.css",
                "~/content/kendo/kendo.css",
                "~/content/site-kendo.css",
                "~/content/site-admin.css"));

            bundles.Add(new ScriptBundle("~/js/image-uploader").Include("~/scripts/dropzone.js", "~/scripts/image-uploader.js"));
            bundles.Add(new StyleBundle("~/css/image-uploader").Include("~/content/image-uploader.css"));

            bundles.Add(new StyleBundle("~/css/nivo").Include(
                "~/content/nivo/nivo-lightbox.css",
                "~/content/nivo/default/default.css"));

            bundles.Add(new ScriptBundle("~/js/zoom").Include("~/scripts/zoom/jquery.elevateZoom.js"));
        }
    }
}
