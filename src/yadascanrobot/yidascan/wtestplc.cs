using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using yidascan.DataAccess;

namespace yidascan {
    public partial class wtestplc : Form {
        public wtestplc() {
            InitializeComponent();
            initOpcClient();

            for (var i = 1; i <= 6; i++) {
                cbxJob.Items.Add(i.ToString());
            }
        }

        private IOpcClient client;
        public string opc_server_ip = "127.0.0.1";

        public void initOpcClient() {
            var dtopc = OPCParam.Query();
            dtopc.Columns.Remove("Class");
            dtopc.Columns.Add(new DataColumn("Value"));

            if (client.Open(opc_server_ip)) {
                client.AddSubscription(dtopc);
            } else {
                MessageBox.Show("访问OPC server失败。");
            }
        }

        private void WriteLog(string s) {
            var d = DateTime.Now.ToString();
            var msg = $"{d} {s}";
            listBox1.Items.Insert(0, msg);
        }

        private void btnReadCacheSignal_Click(object sender, EventArgs e) {
            var r = PlcHelper.ReadItemInFromCache(client); // 读来料处信号
            WriteLog($"缓存来料信号: {r}");
        }

        private void btnWriteSignal_Click(object sender, EventArgs e) {
            if (cbxJob.Text == "") {
                throw new Exception("没有选动作。");
                }
            var job = Int32.Parse(cbxJob.Text);
            var possave = (int)ntxtCachePosSave.Value;
            var posget = (int)ntxtCachePosGet.Value;
            PlcHelper.WriteCacheJob(client, (CacheJob)job, possave, posget);
            WriteLog($"写缓存区动作: {job}");
        }

        private void button3_Click(object sender, EventArgs e) {
            // 测试标签采集处来料信号。
            var r = PlcHelper.ReadLabelCatch(client); // 读来料处信号
            WriteLog($"标签采集处信号: {r}");
        }

        private void button4_Click(object sender, EventArgs e) {
            // 写标签采集处来料信号。
            var d = Int32.Parse(txDiameter.Text);
            var c = (int)ntxtChannel.Value;
            PlcHelper.WriteLabelCatch(client, d, c);
        }

        private void button5_Click(object sender, EventArgs e) {
            // 读抓料处信号。
            var pos = (int)ntxtPos.Value;
            var r = PlcHelper.ReadItemCatchSignal(client, pos);
            WriteLog($"{pos}, {r}");
        }

        private void button6_Click(object sender, EventArgs e) {
            // 复位抓料处信号。
            var pos = (int)ntxtPos.Value;
            PlcHelper.ResetItemCatchSignal(client, pos);
        }
    }
}
