using Newtonsoft.Json;
using Sitecore.Configuration;
using Sitecore.Forms.Report.Viewer.DataProvider;
using Sitecore.Forms.Report.Viewer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace Sitecore.Forms.Report.Viewer.Repository
{
    public class FormReportRepository : IFormReportRepository
    {
        private ISqlDataProvider dataProvider;
        private string FormDefinitionIdField => Settings.GetSetting("FormDefinitionIdField") ?? "FormItemId";

        public FormReportRepository(ISqlDataProvider _dataProvider)
        {
            dataProvider = _dataProvider;
        }

        public FormReportResponseModel GetAllForms()
        {
            Sitecore.Diagnostics.Log.Debug("Got the Request for Forms names.", this);

            DataTable dt = dataProvider.GetAllFormsId();
            FormReportResponseModel response = new FormReportResponseModel();
            List<FormsRecordModel> formsRecords = new List<FormsRecordModel>();
            if (dt != null && dt.Rows.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    DataRow currentRow = dt.Rows[i];
                    sb.Append($"'{currentRow[FormDefinitionIdField]}'");
                    if (sb.ToString().Length > 0 && dt.Rows.Count - 1 != i)
                    {
                        sb.Append(",");
                    }
                }

                if (sb.ToString().Length > 0)
                {
                    DataTable dt2 = dataProvider.GetAllForms(sb.ToString());
                    if (dt2 != null && dt2.Rows.Count > 0)
                    {
                        for (int i = 0; i <= dt2.Rows.Count - 1; i++)
                        {
                            DataRow dr = dt2.Rows[i];
                            formsRecords.Add(new FormsRecordModel()
                            {
                                Id = dr["Id"].ToString(),
                                Name = dr["Name"].ToString(),
                                Created = dr["Created"].ToString()
                            });
                        }
                    }

                    if (formsRecords.Any())
                    {
                        List<FormsRecordModel> records = formsRecords.OrderBy(x => x.Name).ToList();
                        records.Insert(0, new FormsRecordModel() { Id = "0", Name = "Select Form", Created = DateTime.UtcNow.ToString() });
                        response.ResponseMessage = string.Empty;
                        response.IsSuccess = true;
                        response.Data = JsonConvert.SerializeObject(records);
                        return response;
                    }

                }

            }
            response.ResponseMessage = "unable to get all forms record.";
            response.IsSuccess = false;
            response.Data = JsonConvert.SerializeObject(formsRecords);
            return response;
        }


        public FormReportResponseModel GetFormLanguageByFormId(string formItemId)
        {
            throw new NotImplementedException();
        }

        public string GetFormReportJsonData(FormReportRequestModel request)
        {



            DataSet dsFormReportData = dataProvider.GetFormReportData(request);

            if (dsFormReportData != null && dsFormReportData.Tables.Count > 0 && dsFormReportData.Tables[0].Rows.Count > 0)
            {
                ReportDataModel reportModel = new ReportDataModel();
                reportModel.ReportData = dsFormReportData.Tables[0];
                List<string> dtColList = new List<string>();
                string dummyJsonRow = "{0}\"data\":\"{1}\",\"title\":\"{2}\"{3}";
                string dummyTableHead = "<th>{0}</th>";
                string jsonRow = string.Empty;
                string tableHead = String.Empty;

                foreach (DataColumn col in dsFormReportData.Tables[0].Columns)
                {
                    if (jsonRow.Length > 0)
                    {
                        jsonRow += ", ";
                    }

                    jsonRow += string.Format(dummyJsonRow, "{", col.ColumnName, col.ColumnName, "}");
                    tableHead += string.Format(dummyTableHead, col.ColumnName);
                }

                reportModel.TableHead = tableHead;
                reportModel.ReportColumn = jsonRow;
                return JsonConvert.SerializeObject(reportModel);
            }

            return string.Empty;
        }

        public FormReportResponseModel GetFormsLanguage(string formId)
        {
            FormReportResponseModel response = new FormReportResponseModel();
            DataTable dt = dataProvider.GetFormLanguageByFormId(formId);
            List<FormLanguageModel> formsRecords = new List<FormLanguageModel>();
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    DataRow dr = dt.Rows[i];
                    formsRecords.Add(new FormLanguageModel()
                    {
                        ItemId = dr["ItemId"].ToString(),
                        Language = dr["Language"].ToString(),
                        Version = dr["Version"].ToString()
                    });
                }
                if (formsRecords.Any())
                {
                    List<FormLanguageModel> records = formsRecords.OrderBy(x => x.Language).ToList();
                    records.Insert(0, new FormLanguageModel() { ItemId = "0", Language = "Select Language", Version = "0" });
                    response.ResponseMessage = string.Empty;
                    response.IsSuccess = true;
                    response.Data = JsonConvert.SerializeObject(records);
                    return response;
                }
            }
            response.ResponseMessage = "unable to get forms language.";
            response.IsSuccess = false;
            response.Data = JsonConvert.SerializeObject(formsRecords);
            return response;
        }
    }
}