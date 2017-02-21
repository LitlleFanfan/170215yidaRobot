using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using yidascan.DataAccess;

namespace yidascan {
    public class QueuesView {
        public static Form f;
        public static void Add(ListView lsv, LableCode lc) {
            f.Invoke((EventHandler)(delegate {
                ListViewItem lsvItem = new ListViewItem(string.Format("{0} {1}", lc.LCode, lc.ToLocation));
                lsv.Items.Insert(0, lsvItem);
            }));
        }

        public static void Move(ListView lsv1, ListView lsv2) {
            f.Invoke((EventHandler)(delegate {
                ListViewItem lsvItem = lsv1.Items[lsv1.Items.Count - 1];
                lsv1.Items.Remove(lsvItem);
                lsv2.Items.Insert(0, lsvItem);
            }));
        }

        public static void Insert(ListView lsv, ListViewItem lsvItem) {
            f.Invoke((EventHandler)(delegate {
                lsv.Items.Insert(0, lsvItem);
            }));
        }

        public static void Remove(ListView lsv) {
            f.Invoke((EventHandler)(delegate {
                lsv.Items.RemoveAt(lsv.Items.Count - 1);
            }));
        }
    }
}
