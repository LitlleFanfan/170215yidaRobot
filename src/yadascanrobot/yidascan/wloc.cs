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

        public void setdata(LocationHelper locs_) {
            locs = locs_;
        }

        public void ShowMap() {
            var view = listView1;
            view.Items.Clear();

            foreach (var item in locs.LocMap) {
                if (string.IsNullOrEmpty(item.Value)) {
                    var viewitem1 = new ListViewItem {
                        Text = item.Key,
                    };
                    viewitem1.SubItems.Add("-");

                    view.Items.Add(viewitem1);
                    continue;
                }

                var real = locs.RealLocations.Single(x => x.realloc == item.Value);

                var viewitem = new ListViewItem {
                    Text = item.Key,
                };
                viewitem.SubItems.Add(real.realloc);

                view.Items.Add(viewitem);
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
            if (commonhelper.CommonHelper.Confirm("确定要复位吗?")) {
                locs.Resetall();
                ShowMap();
                ShowRealLocs();
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e) {
            if (listView1.SelectedItems.Count > 0) {
                var item = listView1.SelectedItems[0];
                txVirtual.Text = item.Text;
            }
        }

        private void btnOnFull_Click(object sender, EventArgs e) {
            locs.OnFull(txOnFull.Text);
            ShowMap();
            ShowRealLocs();
        }

        private void btnOnReady_Click(object sender, EventArgs e) {
            locs.OnReady(txOnFull.Text);
            ShowMap();
            ShowRealLocs();
        }

        private void btnGet_Click(object sender, EventArgs e) {
            locs.Convert(txVirtual.Text);
            ShowMap();
            ShowRealLocs();
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e) {
            var items = listView2.SelectedItems;
            if (items.Count > 0) {
                txOnFull.Text = items[0].Text;
            }
        }
    }
}
