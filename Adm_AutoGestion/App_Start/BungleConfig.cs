using System;
using System.Web.Optimization;

namespace Adm_AutoGestion.App_Start
{
    public class BungleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        { 
            bundles.Add(new ScriptBundle("~/bundles/scripts/desktop").Include
                ("~/Scripts/jquery-3.0.0.js").IncludeDirectory("~/Scripts",".js"));

            bundles.Add(new StyleBundle("~/bundles/css/desktop").Include
           ("~/Content/bootstrap.css").IncludeDirectory("~/Content", ".css"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
        "~/Scripts/jquery.unobtrusive*",
        "~/Scripts/jquery.validate*"));


            bundles.Add(new ScriptBundle("~/Scripts/umd").Include(
                     "~/Scripts/umd/bootstrap.js",
                     "~/Scripts/umd/bootstrap.min.js",
                     "~/Scripts/umd/jquery-3.4.1.js"));

            bundles.Add(new StyleBundle("~/Content").Include(
                                  //"~/Content/bootstrap-datepicker.css",
                                  //"~/Content/bootstrap-theme.css",
                                  //"~/Content/bootstrap-theme.min.css",
                                  "~/Contents/bootstrap.css",
                                  "~/Content/bootstrap.min.css"));
        }
    }
}