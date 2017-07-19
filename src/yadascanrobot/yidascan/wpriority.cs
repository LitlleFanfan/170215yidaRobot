using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace yidascan {
    public partial class wpriority : Form {
        public wpriority() {
            InitializeComponent();

            showRealLocations();
        }
        
        public LocationHelper locs;

        public void showRealLocations() {
            listView1.Items.Clear();
            foreach (var item in locs.RealLocations) {
                var vi = new ListViewItem {
                    Text = item.realloc
                };
                vi.SubItems.Add(LocationHelper.priority_s(item.priority));
                vi.Tag = item;
                listView1.Items.Add(vi);
            }
        }

        private void resetChecks() {
            rbHigh.Checked = false;
            rbMedium.Checked = false;
            rbLow.Checked = false;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e) {
            var c = listView1.SelectedItems.Count;
            if (c > 0) {
                resetChecks();

                var item = (RealLoc)listView1.SelectedItems[0].Tag;
                switch (item.priority) {
                    case Priority.HIGH:
                        rbHigh.Checked = true;
                        break;
                    case Priority.MEDIUM:
                        rbMedium.Checked = true;
                        break;
                    case Priority.LOW:
                        rbLow.Checked = true;
                        break;
                }
                lbLocation.Text = item.realloc;
            }
        }

        private void btnSet_Click(object sender, EventArgs e) {
            var selected = (RealLoc)listView1.SelectedItems[0].Tag;
            if (rbHigh.Checked) {
                selected.priority = Priority.HIGH;
            } else if (rbMedium.Checked) {
                selected.priority = Priority.MEDIUM;
            } else if (rbLow.Checked) {
                selected.priority = Priority.LOW;
            }
            
            showRealLocations();
        }

        private void btnExit_Click(object sender, EventArgs e) {
            Close();
        }
    }
}
