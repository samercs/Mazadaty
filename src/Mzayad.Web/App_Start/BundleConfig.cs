using System.Web.Optimization;

namespace Mzayad.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.UseCdn = true;

            var jquery = new ScriptBundle("~/js/jquery", "//ajax.googleapis.com/ajax/libs/jquery/2.1.3/jquery.min.js").Include(
                "~/scripts/jquery-{version}.js");
            jquery.CdnFallbackExpression = "window.jQuery";
            bundles.Add(jquery);

            bundles.Add(new ScriptBundle("~/js/site").Include(
                "~/scripts/bootstrap.js",
                "~/scripts/site.js"));

            bundles.Add(new StyleBundle("~/css").Include(
                "~/content/bootstrap.css",
                "~/content/bootstrap-theme.css",
                "~/content/font-awesome.css",
                "~/content/site-forms.css",
                "~/content/site-layout.css",
                "~/content/site-arabic.css",
                "~/content/site.css"));
        }
    }
}
