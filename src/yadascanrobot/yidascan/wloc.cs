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

namespace yidascan {
    public partial class wloc : Form {
        public wloc() {
            InitializeComponent();
        }

        private LocationHelper locs;
        private bool isrunning = false;

        public void setRunState(bool isrunning) {
            this.isrunning = isrunning;
            // 许可/禁止右键菜单。
            contextMenuStrip1.Enabled = !isrunning;
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
                // 给已经对应的实际交地加个星标。
                if (locs.isMapped(item.realloc)) {
                    vi.Text += "*";
                }
                vi.SubItems.Add(LocationHelper.state_s(item.state));
                vi.SubItems.Add(LocationHelper.priority_s(item.priority));
                vi.SubItems.Add(item.panelno);
                vi.Tag = item;

                view.Items.Add(vi);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e) {
            lock (TaskQueues.LOCK_LOCHELPER) {
                ShowMap();
                ShowRealLocs();
            }
        }

        private void timer1_Tick(object sender, EventArgs e) {
            try {
                if (isrunning) {
                    lock (TaskQueues.LOCK_LOCHELPER) {
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

        private void listView1_SelectedIndexChanged(object sender, EventArgs e) {
            var item = listView1.SelectedItems;
            if (item.Count > 0) {
                var selected = item[0];
            }
        }

        private void miEnable_Click(object sender, EventArgs e) {
            var items = listView2.SelectedItems;
            var item = items.Count > 0 ? (RealLoc)items[0].Tag : null;

            if (item == null) {
                MessageBox.Show("没有选中交地。");
                return;
            }

            if (!CommonHelper.Confirm($"要设实际交地{item.realloc}为空闲状态吗？")) {
                return;
            }

            lock (TaskQueues.LOCK_LOCHELPER) {
                if (!locs.isMapped(item.realloc)) {
                    item.priority = Priority.MEDIUM;
                    item.state = LocationState.IDLE;

                    ShowRealLocs();
                } else {
                    CommonHelper.Warn($"交地正在使用中，不能修改状态。");
                }
            }
        }

        private void miDisable_Click(object sender, EventArgs e) {
            var items = listView2.SelectedItems;

            var item = items.Count > 0 ? (RealLoc)items[0].Tag : null;

            if (item == null) {
                MessageBox.Show("没有选中交地。");
                return;
            }

            if (!CommonHelper.Confirm($"要设实际交地{item.realloc}为禁止状态吗？")) {
                return;
            }

            lock (TaskQueues.LOCK_LOCHELPER) {
                if (!locs.isMapped(item.realloc)) {
                    item.priority = Priority.DISABLE;
                    ShowRealLocs();
                } else {
                    MessageBox.Show("交地正在使用中，不能修改状态。");
                }
            }
        }

        private void wloc_Load(object sender, EventArgs e) {
            lock(TaskQueues.LOCK_LOCHELPER) {
                ShowMap();
                ShowRealLocs();
            }
        }

        private void btnForceDisable_Click(object sender, EventArgs e) {
            var items = listView2.SelectedItems;

            var item = items.Count > 0 ? (RealLoc)items[0].Tag : null;

            if (item == null) {
                MessageBox.Show("没有选中交地。");
                return;
            }

            if (!CommonHelper.Confirm($"要强制设置实际交地{item.realloc}为禁止状态吗？")) {
                return;
            }

            lock (TaskQueues.LOCK_LOCHELPER) {
                locs.forceDisable(item.realloc);
            }
            ShowRealLocs();
        }
    }
}


