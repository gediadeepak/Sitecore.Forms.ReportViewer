using Sitecore.Forms.Report.Viewer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Forms.Report.Viewer.DataProvider
{
    public interface ISqlDataProvider
    {
        DataTable GetAllForms(string ids);
        DataTable GetAllFormsId();
        DataTable GetFormLanguageByFormId(string formItemId);
        DataTable GetFormsLanguage(string formId);
        DataSet GetFormReportData(FormReportRequestModel request);
    }
}
