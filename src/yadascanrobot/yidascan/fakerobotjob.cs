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
#if DEBUG

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

        public void JobLoop(ref bool isrunning, ListView viewA, ListView viewB) { }

        public void JobLoopPro(ref bool isrunning, TaskQueues taskq, Action onupdate) {
            loghandler.Invoke($"enter loop. isrunning: {isrunning}", LogType.ROBOT_STACK, LogViewType.OnlyForm);

            while (isrunning) {
                loghandler.Invoke("move queue.", LogType.ROBOT_STACK, LogViewType.OnlyForm);

                var ques = new List<Queue<RollPosition>> { taskq.RobotRollAQ, taskq.RobotRollBQ };

                foreach (var qu in ques) {
                    if (qu.Count() > 0) {
                        var item = qu.Peek();
                        if (item != null && JobTask(ref isrunning, item)) {
                            qu.Dequeue();
                        }
                    }
                }
                onupdate();

                Thread.Sleep(500);
            }
        }

        public bool JobTask(ref bool isrun, RollPosition roll) {
            return true;
        }

        public void Dispose() {
            GC.SuppressFinalize(this);
        }

        public void NotifyOpcJobFinished(PanelState pState, string tolocation) {
            // throw new NotImplementedException();
        }
    }
#endif
}
