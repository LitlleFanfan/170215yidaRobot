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
        private CheckBox[] btns;

        public wdeletebcode() {
            InitializeComponent();
            showTitle();

            btns = new CheckBox[] { ckBeforeCache, ckCache, ckCatchQue, ckLableUp, ckRobotQue, ckWeighQue };
            refreshUI();
            foreach (var btn in btns) {
                btn.CheckedChanged += (s, a) => { refreshUI(); };
            }            
        }

        private void btnDelete_Click(object sender, EventArgs e) {    
            if (tabControl1.SelectedTab == pageSingle) {
                deleteSingle();
            } else if (tabControl1.SelectedTab == pageQueues) {
                deleteQue();
            }
        }

        private void refreshUI() {
            
            foreach (var btn in btns) {
                btn.ForeColor = btn.Checked ? Color.Red : SystemColors.ControlText;
            }
        }

        private void deleteSingle() {
            var code = txtLabelCode.Text.Trim();

            if (code.Length != 12) {
                log($"号码{code}长度应当是12位，实际长度: {code.Length}");
                return;
            }

            Cursor = Cursors.WaitCursor;

            try {
                DeleteCodeFromQueAndDb(code);
            } finally {
                wmain.ShowTaskQ();
                Cursor = Cursors.Default;
            }
        }

        private void deleteQue() {
            var data = new List<string>();
            if (ckWeighQue.Checked) {
                data = data.Union(FrmMain.taskQ.WeighQ.Select(x => x.LCode)).ToList();
            }
            if (ckBeforeCache.Checked) {
                data = data.Union(FrmMain.taskQ.CacheQ.Select(x => x.LCode)).ToList();
            }
            if (ckCache.Checked) {
                data = data.Union(FrmMain.taskQ.CacheSide.Where(x => x.labelcode != null)
                    .Select(x => x.labelcode.LCode)).ToList();
            }
            if (ckLableUp.Checked) {
                data = data.Union(FrmMain.taskQ.LableUpQ.Select(x => x.LCode)).ToList();
            }
            if (ckCatchQue.Checked) {
                data = data.Union(FrmMain.taskQ.CatchAQ.Select(x => x.LCode)).ToList();
                data = data.Union(FrmMain.taskQ.CatchBQ.Select(x => x.LCode)).ToList();
            }
            if (ckRobotQue.Checked) {
                data = data.Union(FrmMain.taskQ.RobotRollAQ.Select(x => x.LabelCode)).ToList();
                data = data.Union(FrmMain.taskQ.RobotRollBQ.Select(x => x.LabelCode)).ToList();
            }

            foreach (var item in data) {
                DeleteCodeFromQueAndDb(item);
                wmain.ShowTaskQ();           
                Application.DoEvents();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            Close();
        }

        #region DELETE_B_CODE
        public void DeleteCodeFromQueAndDb(string code) {
            lock (TaskQueues.LOCK_LOCHELPER) {
                deleteFromTaskq(FrmMain.taskQ, code);
            }
            log($"号码{code}已从队列中删除");

            var label = LableCode.QueryByLCode(code);            
            if (label != null) {
                label.Remark += ";delete";

                // 只有没计算过位置的布卷才可以从数据库中删除。
                if (string.IsNullOrEmpty(label.PanelNo) || label.FloorIndex == 0) {
                    LableCode.Delete(label.LCode);
                    LableCode.SaveToHistory(label);
                    log($"号码{label.LCode}已经从数据库中删除");
                } else {
                    log($"号码{label.LCode}不能从数据库删除，板号: {label.PanelNo}, 层位置: {label.FloorIndex}");
                }

                // tell plc.
                notifyOpc(label.LCode);
            } else {
                var msg = $"数据库中没有这个号码: {code}";
                log(msg);
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

        private static bool deleteFromCache(IEnumerable<CachePos> q, string s) {
            foreach (var item in q) {
                if (item.labelcode != null && item.labelcode.LCode == s) {
                    item.labelcode = null;
                }
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
            var h = deleteFromCache(ques.CacheSide, code);
            return a || b || c || d || e || f || g;
        }

        private static bool IsInCacheQue(CachePos[] cq, string code) {
            return cq.Count(x => x.labelcode != null && x.labelcode.LCode == code) > 0;
        }

        private void log(string s) {
            lbxLog.Items.Insert(0, $"{lbxLog.Items.Count + 1} {s}");
            FrmMain.logOpt.Write("!" + s);
        }
        #endregion

        private void showTitle() {
            var page = tabControl1.SelectedTab.Text;
            Text = $"删除B区号码({page})";
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e) {
            showTitle();
        }
    }
}
