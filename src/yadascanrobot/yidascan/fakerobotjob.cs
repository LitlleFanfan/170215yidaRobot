using ProduceComm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using yidascan.DataAccess;

namespace yidascan {
    class FakeRobotJob : IRobotJob, IDisposable {
        Action<string, string, LogViewType> loghandler;
        IOpcClient client;
        string ip;
        string jobName;

        public FakeRobotJob(string ip, string jobName) {
            this.ip = ip;
            this.jobName = jobName;
        }

        public void setup(Action<string, string, LogViewType> loghandler, IOpcClient client, OPCParam param) {
            this.loghandler = loghandler;
            this.client = client;
            loghandler?.Invoke("!fake robot setup.", "fake robot", LogViewType.OnlyForm);
            loghandler?.Invoke($"!ip: {ip}, job name: {jobName}", "fake robot", LogViewType.OnlyFile);
        }

        public bool IsConnected() {
            return true;
        }

        public void JobLoop(ref bool isrunning, ListView viewA, ListView viewB) {
            while (isrunning) {
                if (FrmMain.taskQ.RobotRollAQ.Count > 0) {
                    var roll = FrmMain.taskQ.RobotRollAQ.Peek();
                    if (roll != null) {
                        FrmMain.taskQ.RobotRollAQ.Dequeue();
                        FrmMain.showRobotQue(FrmMain.taskQ.RobotRollAQ, viewA);
                        Thread.Sleep(100);
                    }
                }
                if (FrmMain.taskQ.RobotRollBQ.Count > 0) {
                    var roll = FrmMain.taskQ.RobotRollBQ.Peek();
                    if (roll != null) {
                        FrmMain.taskQ.RobotRollBQ.Dequeue();
                        FrmMain.showRobotQue(FrmMain.taskQ.RobotRollBQ, viewB);
                    }
                }
                Thread.Sleep(RobotHelper.DELAY);
            }
        }

        public void Dispose() {
            GC.SuppressFinalize(this);            
        }
    }
}
