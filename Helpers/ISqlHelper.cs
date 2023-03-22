using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Forms.Report.Viewer.Helpers
{
    public interface ISqlHelper
    {
        DataSet GetSqlRecords(string connectionString, string sqlQuery, SqlParameter[] parameters);

    }
}
