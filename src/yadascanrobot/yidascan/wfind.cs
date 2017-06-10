using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

using yidascan.DataAccess;

namespace yidascan {
    public partial class wfind : Form {
        public wfind() {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, EventArgs e) {
            mtxCode.SelectAll();
            var key = shrink(mtxCode.Text);
            if (string.IsNullOrEmpty(key)) { return; }

            Cursor = Cursors.WaitCursor;
            try {
                if (!ckHistory.Checked) {
                    var c = LableCode.QueryByLCode(key);
                    if (c != null) {
                        showlabel(c);
                    } else {
                        showwarning(key, "当前运行数据");
                    }
                    lbx.Items.Insert(0, $"<当前运行数据>查询结果:");
                    lbx.Items.Insert(0, $"---------------------");
                }

                if (ckHistory.Checked) {
                    var c = LableCode.QueryByLCodeFromHis(key);
                    if (c != null && c.Count() != 0) {
                        foreach (var obj in c) {
                            showlabel(obj);
                        }
                    } else {
                        showwarning(key, "历史数据");
                    }

                    var count = c == null ? 0 : c.Count;
                    lbx.Items.Insert(0, $"<历史数据>查询结果({count}):");
                    lbx.Items.Insert(0, $"---------------------");
                }
            } finally { Cursor = Cursors.Default; }
        }

        #region PRIVATE
        void showlabel(LableCode c) {
            lbx.Items.Insert(0, $"扫描时间: {c.CreateDate.ToString()}");
            lbx.Items.Insert(0, $"板上坐标: {c.Coordinates}");
            switch (c.Status) {
                case 5:
                    lbx.Items.Insert(0, $"状态: 已上垛，且板已完成。");
                    break;
                case 3:
                    lbx.Items.Insert(0, $"状态: 已上垛。");
                    break;
                case 2:
                    lbx.Items.Insert(0, $"状态: 未上垛，是层最后一卷。");
                    break;
                case 0:
                default:
                    lbx.Items.Insert(0, $"状态: 未上垛。");
                    break;
            }
            lbx.Items.Insert(0, $"层: {c.Floor} 层序号: {c.FloorIndex}");
            lbx.Items.Insert(0, $"板号: {c.PanelNo}");
            lbx.Items.Insert(0, $"长度: {c.Length}mm 直径: {c.Diameter}mm");
            lbx.Items.Insert(0, $"号码: {c.LCode}");
            lbx.Items.Insert(0, $"名义交地: {c.ToLocation}  实际交地: {c.RealLocation}");
            lbx.Items.Insert(0, $"---------------------");
        }

        private void showwarning(string c, string dataarea) {
            lbx.Items.Insert(0, $"<{dataarea}>中，号码 {c} 没有找到。");
            lbx.Items.Insert(0, $"---------------------");
        }

        private static string shrink(string k) {
            var s = k.Where(x => !string.IsNullOrWhiteSpace(x.ToString()));
            return string.Join("", s);
        }
        #endregion

        private void mtxCode_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == '\r') {
                mtxCode.Text = mtxCode.Text.Substring(0, 12);
                btnSearch_Click(sender, e);
            }
        }

        private void btnClear_Click(object sender, EventArgs e) {
            lbx.Items.Clear();
        }
    }
}
