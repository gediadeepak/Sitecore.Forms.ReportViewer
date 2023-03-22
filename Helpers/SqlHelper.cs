using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Sitecore.Forms.Report.Viewer.Helpers
{
    public class SqlHelper : ISqlHelper
    {
        public DataSet GetSqlRecords(string connectionString, string sqlQuery, SqlParameter[] parameters)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            DataSet ds = new DataSet();
            try
            {
                SqlCommand command = new SqlCommand(sqlQuery, conn);
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable tbl = new DataTable();
                adapter.Fill(tbl);
                if (tbl.Rows.Count > 0)
                {
                    ds.Tables.Add(tbl);
                }
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error($"Exception while getting data from database.", ex, this);
            }
            finally
            {
                if ((conn.State == ConnectionState.Open))
                {
                    conn.Close();
                }
            }
            return ds;
        }
    }
}