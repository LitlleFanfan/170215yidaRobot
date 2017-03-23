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
            var key = shrink(mtxCode.Text);
            if (string.IsNullOrEmpty(key)) { return; }

            var c = LableCode.QueryByLCode(key);
            if (c != null) {
                showlabel(c);
            } else {
                showwarning(key);
            }
        }

        #region PRIVATE
        void showlabel(LableCode c) {
            lbx.Items.Insert(0, $"更新时间: {c.UpdateDate.ToString()}");
            lbx.Items.Insert(0, $"板上坐标: {c.Coordinates}");
            lbx.Items.Insert(0, $"层: {c.Floor} 位置: {c.FloorIndex}");
            lbx.Items.Insert(0, $"板号: {c.PanelNo}");
            lbx.Items.Insert(0, $"长度: {c.Length} 直径: {c.Diameter}");
            lbx.Items.Insert(0, $"号码: {c.LCode}");
            lbx.Items.Insert(0, $"交地: {c.ToLocation}");
            lbx.Items.Insert(0, $"---------------------");
        }

        private void showwarning(string c) {
            lbx.Items.Insert(0, $"号码 {c} 没有找到。");
            lbx.Items.Insert(0, $"---------------------");
        }

        private static string shrink(string k) {
            var s = k.Where(x => !string.IsNullOrWhiteSpace(x.ToString()));
            return string.Join("", s);
        }
        #endregion
    }
}
