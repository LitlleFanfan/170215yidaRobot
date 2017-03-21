using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ProduceComm;

namespace yidascan {
    public partial class FrmSet : Form {
        public FrmSet() {
            InitializeComponent();

            IniCombobox(cmbCom);
            IniCombobox(cmbCom2);
        }

        public static CommunicationCfg pcfgScan1 = new CommunicationCfg("SCAN1");
        public static CommunicationCfg pcfgScan2 = new CommunicationCfg("SCAN2");

        private static void IniCombobox(ComboBox cmb) {
            cmb.Items.Clear();
            var num2 = System.IO.Ports.SerialPort.GetPortNames().Length - 1;
            for (int i = 0; i <= num2; i++) {
                cmb.Items.Add(System.IO.Ports.SerialPort.GetPortNames().GetValue(i));
            }
        }

        private void btnSave_Click(object sender, EventArgs e) {
            pcfgScan1.ComPort = cmbCom.Text;
            pcfgScan1.BaudRate = cmbBaudRate.Text;
            pcfgScan1.IPAddr = txtIPAddr.Text;
            pcfgScan1.IPPort = txtIPPort.Text;
            pcfgScan1.CommunicationType = rbtType1.Checked ? CommunicationType.SerialPort.ToString() : CommunicationType.Network.ToString();

            pcfgScan2.ComPort = cmbCom2.Text;
            pcfgScan2.BaudRate = cmbBaudRate2.Text;
            pcfgScan2.IPAddr = txtIPAddr2.Text;
            pcfgScan2.IPPort = txtIPPort2.Text;
            pcfgScan2.CommunicationType = rbtType12.Checked ? CommunicationType.SerialPort.ToString() : CommunicationType.Network.ToString();

            clsSetting.OPCServerIP = txtOPCIP.Text;

            clsSetting.GetLocation = txtGetLocation.Text;
            clsSetting.ToWeight = txtToWeight.Text;
            clsSetting.PanelFinish = txtPanelFinish.Text;

            //clsSetting.ShelfWidth = nudShelfWidth.Value;
            //clsSetting.ShelfTallFirst = nudTallFirst.Value;
            //clsSetting.ShelfTallSecond = nudTallSecond.Value;
            //clsSetting.ShelfTallThird = nudTallThird.Value;
            //clsSetting.ShelfTallFourth = nudTallFourth.Value;
            clsSetting.RollSep = nudObligateLen.Value;

            clsSetting.SplintLength = nudSplintLength.Value;
            clsSetting.SplintWidth = nudSplintWidth.Value;
            clsSetting.SplintHeight = nudSplintHeight.Value;

            clsSetting.OddTurn = chbOddTurn.Checked;
            clsSetting.EdgeSpace = nudEdgeObligate.Value;
            clsSetting.CacheIgnoredDiff = nudCacheIgnoredDiff.Value;
            clsSetting.InitHeigh = nudInitHeigh.Value;
            clsSetting.MaxFloor = (int)nudMaxFloor.Value;

            clsSetting.RobotIP = txtRobotIP.Text;
            clsSetting.JobName = txtJobName.Text;

            try {
                clsSetting.DiffSlope = (int)nudDiffSlope.Value;
                clsSetting.DiffVshape = (int)nudDiffVshape.Value;
            } catch (Exception ex) {
                MessageBox.Show($"保存失败: {ex.ToString()}");
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void CommunicationTypeChange(bool i = true) {
            if (i) {
                pnl1.Enabled = rbtType1.Checked;
                pnl2.Enabled = rbtType2.Checked;
            } else {
                pnl12.Enabled = rbtType12.Checked;
                pnl22.Enabled = rbtType22.Checked;
            }
        }

        private void rbtType1Plc_CheckedChanged(object sender, EventArgs e) {
            CommunicationTypeChange(false);
        }

        private void rbtType2Plc_CheckedChanged(object sender, EventArgs e) {
            CommunicationTypeChange(false);
        }

        private void rbtType1_CheckedChanged(object sender, EventArgs e) {
            CommunicationTypeChange();
        }

        private void rbtType2_CheckedChanged(object sender, EventArgs e) {
            CommunicationTypeChange();
        }

        private void InitRbt(bool netework, bool i = true) {
            if (i) {
                rbtType1.Checked = !netework;
                rbtType2.Checked = netework;
            } else {
                rbtType12.Checked = !netework;
                rbtType22.Checked = netework;
            }
        }

        private void FrmSet_Load(object sender, EventArgs e) {
            cmbCom.Text = pcfgScan1.ComPort;
            cmbBaudRate.Text = pcfgScan1.BaudRate;
            txtIPAddr.Text = pcfgScan1.IPAddr;
            txtIPPort.Text = pcfgScan1.IPPort;
            InitRbt(pcfgScan1.CommunicationType == CommunicationType.Network.ToString());

            cmbCom2.Text = pcfgScan2.ComPort;
            cmbBaudRate2.Text = pcfgScan2.BaudRate;
            txtIPAddr2.Text = pcfgScan2.IPAddr;
            txtIPPort2.Text = pcfgScan2.IPPort;
            InitRbt(pcfgScan2.CommunicationType == CommunicationType.Network.ToString(), false);

            txtOPCIP.Text = clsSetting.OPCServerIP;

            txtGetLocation.Text = clsSetting.GetLocation;
            txtToWeight.Text = clsSetting.ToWeight;
            txtPanelFinish.Text = clsSetting.PanelFinish;

            //nudShelfWidth.Value = clsSetting.ShelfWidth;
            //nudTallFirst.Value = clsSetting.ShelfTallFirst;
            //nudTallSecond.Value = clsSetting.ShelfTallSecond;
            //nudTallThird.Value = clsSetting.ShelfTallThird;
            //nudTallFourth.Value = clsSetting.ShelfTallFourth;
            nudObligateLen.Value = clsSetting.RollSep;

            nudSplintLength.Value = clsSetting.SplintLength;
            nudSplintWidth.Value = clsSetting.SplintWidth;
            nudSplintHeight.Value = clsSetting.SplintHeight;

            chbOddTurn.Checked = clsSetting.OddTurn;
            nudEdgeObligate.Value = clsSetting.EdgeSpace;
            nudCacheIgnoredDiff.Value = clsSetting.CacheIgnoredDiff;
            nudInitHeigh.Value = clsSetting.InitHeigh;
            nudMaxFloor.Value = clsSetting.MaxFloor;

            txtRobotIP.Text = clsSetting.RobotIP;
            txtJobName.Text = clsSetting.JobName;

            nudDiffSlope.Value = clsSetting.DiffSlope;
            nudDiffVshape.Value = clsSetting.DiffVshape;
        }
    }
}
