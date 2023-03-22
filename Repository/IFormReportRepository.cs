using Sitecore.Forms.Report.Viewer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Forms.Report.Viewer.Repository
{
    public interface IFormReportRepository
    {
        string GetFormReportJsonData(FormReportRequestModel request);
        FormReportResponseModel GetAllForms();
        FormReportResponseModel GetFormsLanguage(string formId);
    }
}
