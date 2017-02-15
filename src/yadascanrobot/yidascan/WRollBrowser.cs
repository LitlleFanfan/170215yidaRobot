using System;
using System.Collections.Generic;
using System.Data;
using commonhelper;
using System.Windows.Forms;
using yidascan.DataAccess;
using System.Data.SqlClient;

namespace yidascan {
    public partial class WRollBrowser : Form {
        public WRollBrowser() {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e) {
            if (CommonHelper.Confirm("确定要退出吗？")) {
                Close();
            }
        }

        private void btnLoadData_Click(object sender, EventArgs e) {
            try {
                loadPanels();

                lbxPanel.Items.Clear();
                lbPanelTitle.Text = $"码垛状态{lbxPanel.Items.Count}";

                lbxCache.Items.Clear();
                lbCacheTitle.Text = $"缓存状态{lbxCache.Items.Count}";

                lbPanelno.Text = "";
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString());
            }
        }
        
        private void lbxPanelList_SelectedValueChanged(object sender, EventArgs e) {
            lbxCache.Items.Clear();
            lbxPanel.Items.Clear();
            stmsg.Text = "";

            lbPanelno.Text = "";

            var loc = lbxLocation.SelectedItem;
            if (loc == null) {
                return;
            }

            lbPanelno.Text = $"交地: {loc}";

            try {
                var pno = PanelInfo.QueryLastPanelNo(loc.ToString());
                stmsg.Text = pno;

                lbPanelno.Text = $"交地: {loc} 板号: {pno}"; ;

                var lst = QueryLabesByPanelNo(pno);

                if (lst == null) {
                    stmsg.Text = "labels: null.";
                    return;
                } else {
                    stmsg.Text = $"labels result: {lst.Count}";
                }

                foreach (var item in lst) {
                    if (item.isOnPanel) {
                        lbxPanel.Items.Add(item.label);
                    } else {
                        lbxCache.Items.Add(item.label);
                    }
                }
                lbCacheTitle.Text = $"缓存状态({lbxCache.Items.Count})";
                lbPanelTitle.Text = $"码垛状态({lbxPanel.Items.Count})";
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString());
            }
        }

        private void WRollBrowser_Load(object sender, EventArgs e) {
            btnLoadData.PerformClick();
            stmsg.Text = "选择交地，查看状态。";
        }

        private void loadPanels() {
            lbxLocation.Items.Clear();

            var lst = LableCode.QueryAreaBLocations();
            lbxLocation.Items.AddRange(lst.ToArray());
        }

        private static List<PanelBrief> QueryLabesByPanelNo(string panelno) {
            var sql = "select lcode, isnull(coordinates,'') coordinates from lablecode where panelno=@panelno order by sequenceno";
            var sp = new SqlParameter[]{
                new SqlParameter("@panelno",panelno)};
            var dt = DataAccess.DataAccess.CreateDataAccess.sa.Query(sql, sp);

            var lst = new List<PanelBrief>();

            if (dt != null && dt.Rows.Count > 0) {
                foreach (DataRow row in dt.Rows) {
                    var foo = new PanelBrief(row["lcode"].ToString(),
                                             row["coordinates"].ToString() != "");
                    lst.Add(foo);
                }
            };

            return lst;
        }
    }

    class PanelBrief {
        public string label { get; set; }
        public bool isOnPanel { get; set; }

        public PanelBrief(string s, bool b) {
            label = s;
            isOnPanel = b;
        }
    }
}
