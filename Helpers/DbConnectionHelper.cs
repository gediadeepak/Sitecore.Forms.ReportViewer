using Sitecore.Forms.Report.Viewer.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Forms.Report.Viewer.Helpers
{
    public static class DbConnectionHelper
    {
        public static string MasterConnectionString =
            Sitecore.Configuration.Settings.GetConnectionString(SqlProviderConstants.MasterDbName) != null
                ? Sitecore.Configuration.Settings.GetConnectionString(SqlProviderConstants.MasterDbName)
                : string.Empty;

        public static string FormsConnectionString =
            Sitecore.Configuration.Settings.GetConnectionString(SqlProviderConstants.FormsDbName) != null
                ? Sitecore.Configuration.Settings.GetConnectionString(SqlProviderConstants.FormsDbName)
                : string.Empty;
    }
}
