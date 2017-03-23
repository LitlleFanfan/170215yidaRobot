using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;
using System.Linq;
using commonhelper;
using yidascan.DataAccess;
using yidascan;
using ProduceComm.OPC;
using ProduceComm;
using commonhelper;
namespace yidascan {
    public partial class WinDeleteLabel : Form {
        public WinDeleteLabel() {
            InitializeComponent();
        }

        public FrmMain mainwin = null;

        private void button1_Click(object sender, EventArgs e) {
            var code = getInputCode();

            //提示用户确认。
            var question = string.Format("您确定要删除标签[{0}]吗？", code);
            if (!CommonHelper.Confirm(question)) {
                return;
            }            

            DeleteLabelByHand(code);
            mainwin.ShowTaskQ();
        }

        /// <summary>
        /// 手工删除标签号码。
        /// </summary>
        /// <param name="code">标签号码</param>
        private void DeleteLabelByHand(string code) {
            // 删除号码。
            var lc = LableCode.QueryByLCode(code);
            if (lc == null) {
                CommonHelper.Warn("不存在这个号码!");
                return;
            }

            var area = lc.ParseLocationArea();
            var bAndOnBoard = (area == "B" && lc.Status >= 3);
            var aCarea = (area == "A" || area == "C");

            if (!bAndOnBoard && !aCarea) {
                CommonHelper.Warn("只能删除B区已上垛的号码, 或者A区、C区的号码!");
                return;
            }

            if (LableCode.Delete(code) || deleteFromTaskq(FrmMain.taskQ, code)) {
                notifyOpc(code);
                lbxLog.Items.Insert(0, $"{lbxLog.Items.Count} {code}已经删除。");
                FrmMain.logOpt.Write(string.Format("删除标签{0}成功", code), LogType.NORMAL);
            } else {
                FrmMain.logOpt.Write(string.Format("删除标签{0}失败", code), LogType.NORMAL);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            Close();
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
            var f = deleteFromque(ques.CatchBQ, code);
            var g = deleteFromque(ques.RobotRollAQ, code);
            var h = deleteFromque(ques.RobotRollBQ, code);
            return a || b || c || d || e || f || g || h;
        }

        private void checkInDb(string code) {
            var lc = LableCode.QueryByLCode(code);
            if (lc != null) {
                lbxLog.Items.Insert(0, $"{lbxLog.Items.Count} 数据库: {lc.LCode} {lc.ToLocation} 板号: {lc.PanelNo}");
            } else {
                lbxLog.Items.Insert(0, $"{lbxLog.Items.Count} {code}数据库中没有此号码。");
            }
        }

        private void checkInqueues(string code, TaskQueues ques) {
            var qlabels = new List<Queue<LableCode>> {
                    ques.CacheQ, ques.LableUpQ, ques.WeighQ, ques.CatchAQ,
                    ques.CatchBQ, ques.CatchBQ};
            var qrolls = new List<Queue<RollPosition>> { ques.RobotRollAQ, ques.RobotRollBQ };

            var rt = new List<LableCode>();
            foreach (var item in qlabels) {
                var tmp = item.Where(x => x.LCode == code).ToArray();
                rt.AddRange(tmp);
            }

            var rr = new List<RollPosition>();
            foreach (var item in qrolls) {
                var tmp = item.Where(x => x.LabelCode == code);
                rr.AddRange(tmp);
            }

            if (rt.Count == 0 && rr.Count == 0) {
                lbxLog.Items.Insert(0, $"{lbxLog.Items.Count} 线上和机器人队列没有此号码: {code}。");
            }

            foreach (var item in rt) {
                lbxLog.Items.Insert(0, $"{lbxLog.Items.Count} 线上: {item.LCode} {item.ToLocation} {item.PanelNo}");
            }

            foreach (var item in rr) {
                lbxLog.Items.Insert(0, $"{lbxLog.Items.Count} 机器人号码队列: {item.LabelCode} {item.ToLocation}");
            }
        }

        private string getInputCode() {
            var s = txtLabelCode.Text.Trim();
            if (s.Length > 12) {
                s = s.Substring(0, 12);
            }
            return s;
        }

        private void btnCheck_Click(object sender, EventArgs e) {
            var code = getInputCode();
            checkInDb(code);
            checkInqueues(code, FrmMain.taskQ);
            lbxLog.Items.Add("");
        }
    }
}
