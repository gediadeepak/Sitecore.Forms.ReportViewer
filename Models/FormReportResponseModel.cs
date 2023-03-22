using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Sitecore.Forms.Report.Viewer.Models
{
    public class FormReportResponseModel
    {
        public bool IsSuccess { get; set; }
        public string ResponseMessage { get; set; }
        public string Data { get; set; }
    }

}