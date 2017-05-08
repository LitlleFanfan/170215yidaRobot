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

namespace yidascan {
    public partial class wloc : Form {
        public wloc() {
            InitializeComponent();
        }

        private LocationHelper locs;
        public bool keep_refreshing = false;

        public void setdata(LocationHelper locs_) {
            locs = locs_;
        }

        public void ShowMap() {
            var view = listView1;
            view.Items.Clear();

            foreach (var item in locs.LocMap) {
                var viewitem1 = new ListViewItem {
                    Text = item.Key,
                };
                if (string.IsNullOrEmpty(item.Value)) {
                    viewitem1.SubItems.Add("-");
                } else {
                    viewitem1.SubItems.Add(item.Value);
                }
                view.Items.Add(viewitem1);
            }
        }

        public void ShowRealLocs() {
            var view = listView2;
            view.Items.Clear();

            foreach (var item in locs.RealLocations) {
                var vi = new ListViewItem {
                    Text = item.realloc,
                };
                vi.SubItems.Add(LocationHelper.state_s(item.state));
                vi.SubItems.Add(LocationHelper.priority_s(item.priority));
                view.Items.Add(vi);
            }
        }

        private void btnReset_Click(object sender, EventArgs e) {
            if (commonhelper.CommonHelper.Confirm("确定要设置默认板位吗?")) {
                // locs.Resetall();
                locs.SetRealDefaultPriority();
                ShowMap();
                ShowRealLocs();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e) {
            ShowMap();
            ShowRealLocs();
        }

        private void timer1_Tick(object sender, EventArgs e) {
            try {
                if (keep_refreshing) {
                    ShowMap();
                    ShowRealLocs();
                    stMessage.Text = $"刷新时间: {DateTime.Now}";
                }
            } catch {
                stMessage.Text = "刷新失败";
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            timer1.Enabled = false;
            Close();
        }

        private void btnDefaultReal_Click(object sender, EventArgs e) {
            locs.Resetall();
            ShowMap();
            ShowRealLocs();
        }
    }
}
