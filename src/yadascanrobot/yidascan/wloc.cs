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
        private bool isrunning = false;

        public void setRunState(bool isrunning) {
            this.isrunning = isrunning;
            btnReset.Enabled = !isrunning;
        }

        public void setdata(LocationHelper locs_) {
            locs = locs_;
        }

        public LocationHelper getdata() {
            return locs;
        }

        public void ShowMap() {
            var view = listView1;
            view.Items.Clear();

            foreach (var item in locs.LocMap) {
                var viewitem1 = new ListViewItem {
                    // 名义交地
                    Text = item.Key,
                };

                // 优先级
                var p = locs.VirtualLocations.Where(x => x.virtualloc == item.Key)
                    .Select(x => x.priority).First();
                viewitem1.SubItems.Add(LocationHelper.priority_s(p));

                // 实际交地
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
                vi.SubItems.Add(item.panelno);
                vi.Tag = item;

                view.Items.Add(vi);
            }
        }

        private void btnReset_Click(object sender, EventArgs e) {
            if (commonhelper.CommonHelper.Confirm("确定要设置默认板位吗?")) {
                lock (locs) {
                    locs = LocationHelper.LoadRealDefaultPriority();
                    ShowMap();
                    ShowRealLocs();
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e) {
            lock (locs) {
                ShowMap();
                ShowRealLocs();
            }
        }

        private void timer1_Tick(object sender, EventArgs e) {
            try {
                if (isrunning) {
                    lock (locs) {
                        ShowMap();
                        ShowRealLocs();
                    }
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

        private void btnLoad_Click(object sender, EventArgs e) {
            using (var dlg = new OpenFileDialog()) {
                dlg.Filter = "Json Files(*.json) | *.json";
                var rt = dlg.ShowDialog();
                if (rt == DialogResult.Yes || rt == DialogResult.OK) {
                    locs = LocationHelper.LoadConf(dlg.FileName);
                    ShowMap();
                    ShowRealLocs();
                }
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e) {
            var item = listView1.SelectedItems;
            if (item.Count > 0) {
                var selected = item[0];

            }
        }

        private void miEnable_Click(object sender, EventArgs e) {
            var items = listView2.SelectedItems;
            if (items.Count > 0) {
                var item = (RealLoc)items[0].Tag;
                if (item.priority == Priority.DISABLE) {
                    item.priority = Priority.MEDIUM;
                    item.state = LocationState.IDLE;
                }
                ShowRealLocs();
            }
        }

        private void miDisable_Click(object sender, EventArgs e) {
            var items = listView2.SelectedItems;
            if (items.Count > 0) {
                var item = (RealLoc)items[0].Tag;
                if (item.state == LocationState.IDLE) {
                    item.priority = Priority.DISABLE;
                }
                ShowRealLocs();
            }
        }
    }
}
