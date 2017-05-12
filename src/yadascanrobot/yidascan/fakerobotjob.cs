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
    /// <summary>
    /// 仅用于测试。
    /// </summary>
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
            loghandler?.Invoke("!fake robot setup.", LogType.ROBOT_STACK, LogViewType.OnlyForm);
            loghandler?.Invoke($"!ip: {ip}, job name: {jobName}", "fake robot", LogViewType.OnlyFile);
        }

        public bool IsConnected() {
            return true;
        }

        public void JobLoop(ref bool isrunning, ListView la, ListView lb) {
            while (isrunning) {
                if (FrmMain.taskQ.RobotRollAQ.Count > 0) {
                    var roll = FrmMain.taskQ.RobotRollAQ.Peek();
                    if (roll != null) {
                        JobTask(ref isrunning, true, FrmMain.taskQ.RobotRollAQ, roll, la);
                    }
                }
                if (FrmMain.taskQ.RobotRollBQ.Count > 0) {
                    var roll = FrmMain.taskQ.RobotRollBQ.Peek();
                    if (roll != null) {
                        JobTask(ref isrunning, false, FrmMain.taskQ.RobotRollBQ, roll, lb);
                    }
                }
                Thread.Sleep(RobotHelper.DELAY * 40);
            }
        }

        public void JobLoopPro(ref bool isrunning, TaskQueues taskq, Action onupdate) { }

        public bool JobTask(ref bool isrun, bool isSideA, Queue<RollPosition> robotRollQ, RollPosition roll, ListView lv) {
            LableCode.SetOnPanelState(roll.LabelCode);
            
            // 告知OPC
            NotifyOpcJobFinished(roll);
            TaskQueues.lochelper.OnFull(roll.RealLocation);
            DequeueRoll(robotRollQ, roll, lv);
            
            return true;
        }

        public void Dispose() {
            GC.SuppressFinalize(this);
        }

        public void NotifyOpcJobFinished(string panelNo, string tolocation, string reallocation) {
            // throw new NotImplementedException();
        }

        public void NotifyOpcJobFinished(RollPosition roll) {
            // throw new NotImplementedException();
        }

        public bool PanelAvailable(string tolocation) { return true; }

        private void DequeueRoll(Queue<RollPosition> robotRollQ, RollPosition roll, ListView lv) {
            try {
                if (robotRollQ == null) { return; }

                var roll2 = robotRollQ.Peek();
                if (roll2 != null && roll.LabelCode == roll2.LabelCode) {//如果取出来还是原来那一个，就删一下
                    robotRollQ.Dequeue();
                    FrmMain.showRobotQue(robotRollQ, lv);
                }
            } catch (Exception ex) {
                var msg = $"robot Dequeue roll: {roll.LabelCode}. {ex}";
                throw new Exception(msg);
            }
        }
    }
}
