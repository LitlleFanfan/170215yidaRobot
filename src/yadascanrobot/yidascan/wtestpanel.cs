using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace yidascan {
    public partial class wtestpanel : Form {
        public wtestpanel() {
            InitializeComponent();

            refreshSignals();
        }

        private void btnStartAllSignals_Click(object sender, EventArgs e) {
#if DEBUG
            SignalGen.startall();
            refreshSignals();
            // logOpt.Write("!模拟信号发生启动.");
#endif
        }

        private void btnSignalWeigh_Click(object sender, EventArgs e) {
#if DEBUG   
            SignalGen.toggle(SignalGen.timerWeigh);
#endif
        }

        private void btnSignalCache_Click(object sender, EventArgs e) {
#if DEBUG
            SignalGen.toggle(SignalGen.timerCache);
#endif
        }

        private void btnSignalLableUp_Click(object sender, EventArgs e) {
#if DEBUG
            SignalGen.toggle(SignalGen.timerLabelUp);
#endif
        }

        private void btnSignalCatchA_Click(object sender, EventArgs e) {
#if DEBUG
            SignalGen.toggle(SignalGen.timerItemCatchA);
#endif
        }

        private void button1_Click(object sender, EventArgs e) {
#if DEBUG
            SignalGen.toggle(SignalGen.timerItemCatchB);
#endif
        }

        private void refreshSignals() {
#if DEBUG
            lbWeigth.Text = SignalGen.timerWeigh.Enabled ? "on" : "off";
            lbCache.Text = SignalGen.timerCache.Enabled ? "on" : "off";
            lbCatchA.Text = SignalGen.timerItemCatchA.Enabled ? "on" : "off";
            lbCatchB.Text = SignalGen.timerItemCatchB.Enabled ? "on" : "off";
            lbLableUp.Text = SignalGen.timerLabelUp.Enabled ? "on" : "off";
#endif
        }

        private void timer1_Tick(object sender, EventArgs e) {
            refreshSignals();
        }

        private void btnStopAll_Click(object sender, EventArgs e) {
#if DEBUG
            SignalGen.stoptall();
#endif
            refreshSignals();
        }
    }
}
