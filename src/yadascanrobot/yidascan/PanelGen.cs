using System;
using System.Data.SqlClient;
using System.Threading;
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

        public static bool HasPanelNo(string panelno) {
            var sql = "select count(PanelNo) PanelNo from Panel where PanelNo = @panelno";

            var sp = new SqlParameter[]{
                new SqlParameter("@panelno",panelno)};

            var dt = (int)DataAccess.DataAccess.CreateDataAccess.sa.ExecuteScalars(sql, sp);
            return dt > 0;
        }

        private static string InitPanelNo(DateTime dtime) {
            string panelNo;
            lock (foo) {
                // panelNo = LableCode.GetLastPanelNo(string.Format("{0}", dtime.ToString(clsSetting.LABEL_CODE_DATE_FORMAT)));
                panelNo = LableCode.GetLastPanelNo($"{dtime.ToString(clsSetting.LABEL_CODE_DATE_FORMAT)}");
                panelNo = string.IsNullOrEmpty(panelNo)
                    ? string.Format("{0}{1}", dtime.ToString(clsSetting.PANEL_PATTERN), "0000")
                    : (decimal.Parse(panelNo) + 1).ToString();
            }
            return panelNo;
        }

        public static string NewPanelNo() {
            var counter = 10;
            var found = false;
            lock (foo) {
                while (counter-- > 0) {
                    lastPanelNo = (decimal.Parse(lastPanelNo) + 1).ToString();
                    if (!HasPanelNo(lastPanelNo)) {
                        found = true;
                        break;
                    }
                    Thread.Sleep(10);
                }
            }
            if (!found) { throw new Exception($"创建新板号失败。"); }
            return lastPanelNo;
        }
    }

    public static class Area {
        public static string A = "A";
        public static string B = "B";
        public static string C = "C";
    }
}
