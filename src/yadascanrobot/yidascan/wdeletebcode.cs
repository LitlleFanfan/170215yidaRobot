using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using commonhelper;
using yidascan.DataAccess;
using ProduceComm;
using ProduceComm.OPC;

namespace yidascan {
    public partial class wdeletebcode : Form {
        public FrmMain wmain;

        public wdeletebcode() {
            InitializeComponent();
        }

        private void btnDelete_Click(object sender, EventArgs e) {    
            var code = txtLabelCode.Text.Trim();

            if (code.Length != 12) {
                log($"号码{code}长度应当是12位，实际长度: {code.Length}");
                return;
            }

            if (IsInCacheQue(FrmMain.taskQ.CacheSide, code)) {
                log($"号码{code}在缓存位，不能删除");
                return;
            }

            Cursor = Cursors.WaitCursor;

            var rt = DeleteCodeFromQueAndDb(code);
            wmain.ShowTaskQ();

            if (rt) {
                var msg = $"{code}已经删除";
                FrmMain.logOpt.Write(msg, LogType.NORMAL);
            } else {
                var msg = $"数据库中没有这个号码: {code}";
                log(msg);
                FrmMain.logOpt.Write(msg, LogType.NORMAL);
            }

            Cursor = Cursors.Default;
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            Close();
        }

        #region DELETE_B_CODE
        public bool DeleteCodeFromQueAndDb(string code) {
            var label = LableCode.QueryByLCode(code);            
            if (label != null) {
                label.Remark += ";delete";
                // delete
                deleteFromTaskq(FrmMain.taskQ, label.LCode);
                log($"号码{code}已从队列中删除");

                // 只有没计算过位置的布卷才可以从数据库中删除。
                if (!string.IsNullOrEmpty(label.PanelNo) && label.FloorIndex != 0) {
                    LableCode.Delete(label.LCode);
                    LableCode.SaveToHistory(label);
                    log($"号码{label.LCode}已经从数据库中删除");
                }

                // tell plc.
                notifyOpc(label.LCode);
                
                return true;
            } else {               
                return false;
            }
        }

        private static bool deleteFromque(Queue<LableCode> q, string s) {
            var lst = q.ToList();
            var lcode = lst.FirstOrDefault(x => x.LCode == s);

            if (lcode == null) {
                return false;
            }

            lst.Remove(lcode);

            q.Clear();
            foreach (var item in lst) {
                q.Enqueue(item);
            }

            return true;
        }

        private static void notifyOpc(string lcode) {
            var param = new OPCParam();
            IOpcClient client;

#if DEBUG
            client = new FakeOpcClient(param);
#else
            client = new OPCClient();
#endif            
            client.Open(clsSetting.OPCServerIP);
            param.DeleteLCode = new LCodeSignal(client, "DeleteLCode");

            PlcHelper.NotifyLabelCodeDeleted(client, param, lcode);
        }

        private static bool deleteFromque(Queue<RollPosition> q, string s) {
            // 两个函数几乎完全相同!!!
            var lst = q.ToList();
            var lcode = lst.FirstOrDefault(x => x.LabelCode == s);

            if (lcode == null) {
                return false;
            }

            lst.Remove(lcode);

            q.Clear();
            foreach (var item in lst) {
                q.Enqueue(item);
            }

            return true;
        }

        private static bool deleteFromTaskq(TaskQueues ques, string code) {
            var a = deleteFromque(ques.CacheQ, code);
            var b = deleteFromque(ques.LableUpQ, code);
            var c = deleteFromque(ques.WeighQ, code);
            var d = deleteFromque(ques.CatchAQ, code);
            var e = deleteFromque(ques.CatchBQ, code);
            var f = deleteFromque(ques.RobotRollAQ, code);
            var g = deleteFromque(ques.RobotRollBQ, code);
            return a || b || c || d || e || f || g;
        }

        private static bool IsInCacheQue(CachePos[] cq, string code) {
            return cq.Count(x => x.labelcode != null && x.labelcode.LCode == code) > 0;
        }

        private void log(string s) {
            lbxLog.Items.Insert(0, $"{lbxLog.Items.Count + 1} {s}");
        }
        #endregion
    }
}
