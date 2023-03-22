using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Sitecore.Forms.Report.Viewer.Models
{
    public class ReportDataModel
    {
        public DataTable ReportData { get; set; }
        public string ReportColumn { get; set; }

        public string TableHead { get; set; }
    }
}