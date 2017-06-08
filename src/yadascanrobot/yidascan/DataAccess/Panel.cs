using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace yidascan.DataAccess {
    public class PanelInfo {
        public PanelInfo() {
            PanelNo = null;
            MaxFloor = ProduceComm.clsSetting.MaxFloor;
            CurrFloor = 1;
            EvenStatus = false;
            OddStatus = false;
            Status = 0;
        }

        public PanelInfo(string pno) {
            PanelNo = pno;
            MaxFloor = ProduceComm.clsSetting.MaxFloor;
            CurrFloor = 1;
            EvenStatus = false;
            OddStatus = false;
            Status = 0;
        }
        public string Remark { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime CreateDate { get; set; }
        public bool EvenStatus { get; set; }
        public bool OddStatus { get; set; }
        public int CurrFloor { get; set; }
        public int Status { get; set; }
        public string PanelNo { get; set; }
        public int SequenceNo { get; set; }
        public int MaxFloor { get; set; }
        public bool HasExceed { get; set; }
        public string ToLocation{get;set;}

        public static string QueryLastPanelNo(string location) {
            var sql = "select top 1 panelno from panel where tolocation=@location order by sequenceno desc";
            var sp = new SqlParameter[]{
                new SqlParameter("@location",location)};
            var dt = DataAccess.CreateDataAccess.sa.Query(sql, sp);

            if (dt != null && dt.Rows.Count > 0) {
                return dt.Rows[0]["panelno"].ToString();
            } else { return string.Empty; }
        }
    }
}
