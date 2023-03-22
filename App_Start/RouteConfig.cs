using Sitecore.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Sitecore.Forms.Report.Viewer
{
    public class RouteConfig
    {

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Sitecore.Forms.Report.Viewer", "api/v1/forms/report", new
            {
                controller = "FormReport",
                action = "GetFormReportData"
            });

            routes.MapRoute("Sitecore.Forms.Report.Viewer.AllForms", "api/v1/forms", new
            {
                controller = "FormReport",
                action = "GetAllForms"
            });

            routes.MapRoute("Sitecore.Forms.Report.Viewer.LanguageByFormId", "api/v1/forms/langByFormId", new
            {
                controller = "FormReport",
                action = "GetFormLanguageById"
            });
        }
    }
}
