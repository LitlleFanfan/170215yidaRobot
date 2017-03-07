using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;
using System.Linq;
using commonhelper;
using yidascan.DataAccess;
using yidascan.DataAccess;
using ProduceComm.OPC;
using ProduceComm;
namespace yidascan {
    public partial class WinDeleteLabel : Form {
        public WinDeleteLabel() {
            InitializeComponent();
        }

        public FrmMain mainwin = null;

        private void button1_Click(object sender, EventArgs e) {
            DeleteLabelByHand(txtLabelCode);
        }

        /// <summary>
        /// 手工删除标签号码。
        /// </summary>
        /// <param name="txtDelLCode">号码</param>
        private void DeleteLabelByHand(TextBox txtDelLCode) {
            if (txtDelLCode.Text.Trim().Length < 12) {
                MessageBox.Show("删除号码长度不正确");
                return;
            }

            var code = txtDelLCode.Text.Trim().Substring(0, 12);

            //提示用户确认。
            var question = string.Format("您确定要删除标签[{0}]吗？", code);
            if (!CommonHelper.Confirm(question)) { return; }

            // 删除号码。
            if (LableCode.Delete(code) || deleteFromTaskq(FrmMain.taskQ, code)) { 
                notifyOpc(code);
                FrmMain.logOpt.Write(string.Format("删除标签{0}成功", code), LogType.NORMAL);
            } else {
                FrmMain.logOpt.Write(string.Format("删除标签{0}失败", code), LogType.NORMAL);
            }

            txtDelLCode.Text = string.Empty;
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            Close();
        }

        private static void notifyOpc(string lcode) {
            var client = new OPCClient();
            client.Open(clsSetting.OPCServerIP);

            var dtopc = OPCParam.Query();
            dtopc.Columns.Remove("Class");
            dtopc.Columns.Add(new DataColumn("Value"));
            client.AddSubscription(dtopc);

            var param = new OPCParam();
            param.Init();

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
    }
}
