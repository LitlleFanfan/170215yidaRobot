using System;
using System.Data.SqlClient;
using yidascan.DataAccess;
using ProduceComm;

namespace yidascan {
    /// <summary>
    /// 调用方法：
    /// 在LABELCodeBll.cs文件的以下函数中，
    /// public void GetPanelNo(LableCode lc, DateTime dtime, int shiftNo)
    /// 函数签名改为 GetPanelNo(LableCode lc)
    /// 将语句
    /// string panelNo = NewPanelNo(dtime, shiftNo);
    /// 替换为
    /// string panelNo = PanelGen.NewPanelNo();
    /// </summary>
    public static class PanelGen {
        private static string foo = "lock for panel.";
        private static string lastPanelNo;

        /// <summary>
        /// 在生产任务开始时初始化。
        /// PanelGen.Init(dtpDate.Value);
        /// </summary>
        /// <param name="dtime"></param>       
        public static void Init(DateTime dtime) {
            lastPanelNo = InitPanelNo(dtime);
        }

        private static string GetLastPanelNo(string shiftNo) {
            var sql = "select top 1 PanelNo from LableCode where PanelNo like @shiftNo+'%' group by PanelNo order by PanelNo desc";

            var sp = new SqlParameter[]{
                new SqlParameter("@shiftNo",shiftNo)};

            var dt = DataAccess.DataAccess.CreateDataAccess.sa.Query(sql, sp);
            if (dt == null || dt.Rows.Count < 1) {
                return string.Empty;
            }
            return dt.Rows[0][0].ToString();
        }

        private static string InitPanelNo(DateTime dtime) {
            string panelNo;
            lock (foo) {
                panelNo = LableCode.GetLastPanelNo(string.Format("{0}", dtime.ToString(clsSetting.LABEL_CODE_DATE_FORMAT)));
                panelNo = string.IsNullOrEmpty(panelNo)
                    ? string.Format("{0}{1}", dtime.ToString(clsSetting.LABEL_CODE_DATE_FORMAT), "0000000000")
                    : (decimal.Parse(panelNo) + 1).ToString();
            }
            return panelNo;
        }

        public static string NewPanelNo() {
            lock (foo) {
                lastPanelNo = (decimal.Parse(lastPanelNo) + 1).ToString();
                return lastPanelNo;
            }
        }
    }
}
