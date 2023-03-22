using Sitecore;
using Sitecore.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace Sitecore.Forms.Report.Viewer.Pipelines
{
    public class InitializeRoutes : Sitecore.Mvc.Pipelines.Loader.InitializeRoutes
    {
        public override void Process(PipelineArgs args)
        {
            if (!Context.IsUnitTesting)
                RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}