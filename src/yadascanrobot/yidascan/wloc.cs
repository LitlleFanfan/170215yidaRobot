using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace yidascan {
    public partial class wloc : Form {
        public wloc() {
            InitializeComponent();
        }

        private LocationHelper locs;

        public void setdata(LocationHelper locs_) {
            locs = locs_;
        }

        public void ShowItems() {
            var view = listView1;
            view.Items.Clear();

            foreach (var item in locs.LocMap) {
                if (string.IsNullOrEmpty(item.Value)) {
                    var viewitem1 = new ListViewItem {
                        Text = item.Key,
                    };
                    viewitem1.SubItems.Add("-");
                    viewitem1.SubItems.Add("-");

                    view.Items.Add(viewitem1);
                    continue;
                }

                var real = locs.RealLocations.Single(x => x.readloc == item.Value);

                var viewitem = new ListViewItem {
                    Text = item.Key,
                };
                viewitem.SubItems.Add(real.readloc);
                viewitem.SubItems.Add(LocationHelper.LocationState_s(real.state));

                view.Items.Add(viewitem);
            }
        }

        private void btnReset_Click(object sender, EventArgs e) {
            locs.Resetall();
            ShowItems();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e) {
            if (listView1.SelectedItems.Count > 0) {
                var item = listView1.SelectedItems[0];
                txVirtual.Text = item.Text;
                txReal.Text = item.SubItems[1].Text;
            }
        }

        private void btnSet_Click(object sender, EventArgs e) {
            locs.Map(txVirtual.Text, txReal.Text);
            ShowItems();
        }

        private void btnUnset_Click(object sender, EventArgs e) {
            locs.Map(txVirtual.Text, txReal.Text);
            ShowItems();
        }
    }
}
