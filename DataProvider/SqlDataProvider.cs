using Sitecore.Configuration;
using Sitecore.Forms.Report.Viewer.Helpers;
using Sitecore.Forms.Report.Viewer.Models;
using Sitecore.Shell.Framework.Commands.Masters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using static System.Configuration.ConfigurationManager;

namespace Sitecore.Forms.Report.Viewer.DataProvider
{
    public class SqlDataProvider : ISqlDataProvider
    {
        private readonly ISqlHelper _helper;

        private string FormEntryTable => Settings.GetSetting("FormEntryTable") ?? "FormEntry";
        private string FormFieldTable => Settings.GetSetting("FormFieldTable") ?? "FieldData";
        private string VersionedFieldsTable => Settings.GetSetting("VersionedFieldsTable") ?? "VersionedFields";
        private string FormDefinitionIdField => Settings.GetSetting("FormDefinitionIdField") ?? "FormItemId";
        private string ItemsTable => Settings.GetSetting("ItemsTable") ?? "Items";

        public SqlDataProvider(ISqlHelper helper)
        {
            _helper = helper;
        }

        public DataTable GetAllForms(string ids)
        {
            string sqlQuery = $"select Id,Name,Created from {ItemsTable} where ID in ({ids})";
            DataSet ds = _helper.GetSqlRecords(DbConnectionHelper.MasterConnectionString,
                sqlQuery, null);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0];
            }

            return new DataTable();
        }

        public DataTable GetAllFormsId()
        {
            Sitecore.Diagnostics.Log.Debug("Executing query to get the forms id", this);

            string sqlQuery = $"select distinct {FormDefinitionIdField } from {FormEntryTable}";
            DataSet ds = _helper.GetSqlRecords(DbConnectionHelper.FormsConnectionString,
                sqlQuery, null);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0];
            }

            return new DataTable();
        }

        public DataTable GetFormLanguageByFormId(string formItemId)
        {
            string sqlQuery = $"select distinct ItemId,Language,Version from {VersionedFieldsTable} where ItemId = '{formItemId}'";

            DataSet ds = _helper.GetSqlRecords(DbConnectionHelper.MasterConnectionString,
                sqlQuery, null);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0];
            }

            return new DataTable();
        }

        public DataSet GetFormReportData(FormReportRequestModel request)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@FORMID", SqlDbType.UniqueIdentifier),
                new SqlParameter("@SDATE", SqlDbType.DateTime),
                new SqlParameter("@EDATE", SqlDbType.DateTime)
            };
            parameters[0].Value = Guid.Parse(request.FormId);
            parameters[1].Value = request.StartDate;
            parameters[2].Value = request.EndDate;

            DataSet dsFormsReportData = _helper.GetSqlRecords(DbConnectionHelper.FormsConnectionString,
                GenerateQuery(request), null);
            if (dsFormsReportData.Tables.Count > 0 && dsFormsReportData.Tables[0].Rows.Count > 0)
            {
                return dsFormsReportData;
            }

            return new DataSet();
        }

        public DataTable GetFormsLanguage(string formId)
        {
            throw new NotImplementedException();
        }

        private string GenerateQuery(FormReportRequestModel request)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" Declare @FORMITEMID as uniqueidentifier; Declare @FIELDNAME as NVARCHAR(max);Declare @STARTDATE as datetime;Declare @ENDDATE as datetime;Declare @COLSTRING as nvarchar(max);");
            sb.Append(" Declare @SQLSTRING as nvarchar(max); SET @FORMITEMID = '" + request.FormId + "'; SET @STARTDATE = '" + request.StartDate.ToString("yyyy-MM-dd hh:mm:ss") + "'; SET @ENDDATE = '" + request.EndDate.ToString("yyyy-MM-dd hh:mm:ss") + "';");
            sb.Append(" DECLARE MY_CURSOR CURSOR LOCAL STATIC READ_ONLY FORWARD_ONLY FOR select distinct t2.FieldName from " + FormEntryTable + " t1 inner join " + FormFieldTable + " t2 on t1.ID = t2.FormEntryID  where t1." + FormDefinitionIdField + " = @FORMITEMID");
            sb.Append(" OPEN MY_CURSOR FETCH NEXT FROM MY_CURSOR INTO @FIELDNAME WHILE @@FETCH_STATUS = 0 BEGIN SET @COLSTRING = CONCAT(@COLSTRING , '[',@FIELDNAME,'],')	FETCH NEXT FROM MY_CURSOR INTO @FIELDNAME END");
            sb.Append(" CLOSE MY_CURSOR DEALLOCATE MY_CURSOR set @COLSTRING = substring(@COLSTRING,1,(len(@COLSTRING)-1)) SET @SQLSTRING = N' SELECT [Id], '+ @COLSTRING +' FROM ( Select t1.ID,t2.FieldName as FormField,t2.Value as FormValue from ((select Id," + FormDefinitionIdField + ",Created from " + FormEntryTable + " where " + FormDefinitionIdField + " = '''+ Convert(nvarchar(36),@FORMITEMID) +''' and  created >= '''+ convert(VARCHAR,@STARTDATE,121) +''' and created <= '''+convert(VARCHAR,@ENDDATE,121)+''') t1 inner join (select * from " + FormFieldTable + ") t2 on t1.ID = t2.FormEntryID)) as source PIVOT ( MAX(FormValue) FOR FormField IN ('+ @COLSTRING +')) as target';");
            sb.Append("EXECUTE sp_executesql @SQLSTRING");
            return sb.ToString();

        }
    }
}