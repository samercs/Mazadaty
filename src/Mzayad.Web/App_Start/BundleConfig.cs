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
            jquery.CdnFallbackExpression = "window.jQuery.ui";
            bundles.Add(jqueryui);

            bundles.Add(new ScriptBundle("~/js/site").Include(
                "~/scripts/bootstrap.js",
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
                "~/content/site.css"));

            bundles.Add(new StyleBundle("~/css/admin").Include(
                "~/content/kendo/kendo.common.css",
                "~/content/kendo/kendo.css",
                "~/content/site-kendo.css",
                "~/content/site-admin.css"));
        }
    }
}
