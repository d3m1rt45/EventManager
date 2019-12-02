using System.Web;
using System.Web.Optimization;

namespace KonneyTM
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-3.3.1.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-2.8.3.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js"));

            bundles.Add(new StyleBundle("~/SharedStyles").Include(
                      "~/Content/Bootstrap/bootstrap.min.css",
                      "~/Content/SharedStyles.css"));

            bundles.Add(new StyleBundle("~/HomeStyles").Include("~/Content/HomeStyles.css"));

            bundles.Add(new StyleBundle("~/AccountStyles").Include("~/Content/AccountStyles.css"));

            bundles.Add(new StyleBundle("~/PanelStyles").Include("~/Content/PanelStyles.css"));
        }
    }
}
