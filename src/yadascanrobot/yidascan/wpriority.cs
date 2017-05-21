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

            loadDefaultConf();
            showVirtualLocations();
        }

        public LocationHelper locs;

        public void showVirtualLocations() {
            listView1.Items.Clear();
            foreach (var item in locs.VirtualLocations) {
                var vi = new ListViewItem {
                    Text = item.virtualloc
                };
                vi.SubItems.Add(LocationHelper.priority_s(item.priority));
                vi.Tag = item;
                listView1.Items.Add(vi);
            }
        }

        private void loadDefaultConf() {
            const string JSONFILE = "location_default.json";
            var exe = Assembly.GetExecutingAssembly().Location;
            var exepath = Path.GetDirectoryName(exe);
            var fn = Path.Combine(exepath, JSONFILE);
            locs = LocationHelper.LoadConf(fn);
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

                var item = (VirtualLoc)listView1.SelectedItems[0].Tag;
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
                lbLocation.Text = item.virtualloc;
            }
        }

        private void btnSet_Click(object sender, EventArgs e) {
            var selected = (VirtualLoc)listView1.SelectedItems[0].Tag;
            if (rbHigh.Checked) {
                selected.priority = Priority.HIGH;
            } else if (rbMedium.Checked) {
                selected.priority = Priority.MEDIUM;
            } else if (rbLow.Checked) {
                selected.priority = Priority.LOW;
            }
            
            showVirtualLocations();
        }

        private void btnSave_Click(object sender, EventArgs e) {
            const string JSONFILE = "location_default.json";
            var exe = Assembly.GetExecutingAssembly().Location;
            var exepath = Path.GetDirectoryName(exe);
            var fn = Path.Combine(exepath, JSONFILE);
            locs.SaveConf(fn);

            var td = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            stMsg.Text = $"{ JSONFILE}保存完成, {td}";
        }
    }
}
