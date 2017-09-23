using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using ProduceComm;
using System.Data.SqlClient;

namespace yidascan.DataAccess {
    public class PanelDb {
        public static string GetLatestPanelNo() {
            var sql = "select top 1 panelno from Panel order by sequenceno desc";
            var dt = DataAccess.CreateDataAccess.sa.Query(sql);
            if (dt == null || dt.Rows.Count < 1) {
                return "";
            }

            return (string)dt.Rows[0][0];
        }

    }
}
