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
                       // 等待板可放料
            if (PanelAvailable(roll.RealLocation)) {
                FrmMain.logOpt.Write($"{roll.RealLocation} PushInQueue收到可放料信号", LogType.ROBOT_STACK);
            } else {
                FrmMain.logOpt.Write($"! {roll.RealLocation} PushInQueue未收到可放料信号，请检查板状态和是否有形状不规则报警。", LogType.ROBOT_STACK);
                return false;
            }
            LableCode.SetOnPanelState(roll.LabelCode);

            // 告知OPC
            NotifyOpcJobFinished(roll);
            DequeueRoll(robotRollQ, roll, lv);

            FrmMain.logOpt.Write($"robot job done: {roll.LabelCode}.", LogType.ROBOT_STACK);
            return true;
        }

        public void Dispose() {
            GC.SuppressFinalize(this);
        }

        public void NotifyOpcJobFinished(string panelNo, string tolocation, string reallocation) {
            try {
                var pState = LableCode.IsAllRollOnPanel(panelNo) ? PanelState.Full : PanelState.HalfFull;
                switch (pState) {
                    case PanelState.HalfFull:
                        var lcode = FrmMain.taskQ.UFGetPanelLastRoll(tolocation, panelNo);
                        LableCode.UserSetPanelLastRoll(lcode);//设置板最后一卷布。
                        FrmMain.logOpt.Write($"{reallocation} 半板信号发出,最后一卷布标签{lcode}。slot: ", LogType.ROBOT_STACK);
                        break;
                    case PanelState.Full:
                        FrmMain.logOpt.Write($"{reallocation}: 满板信号发出。slot: ", LogType.ROBOT_STACK);

                        LableCode.SetPanelFinished(panelNo);

                        // 满板时设置自由板位标志。
                        TaskQueues.lochelper.OnFull(reallocation);
                        break;
                    case PanelState.LessHalf:
                        break;
                    default:
                        FrmMain.logOpt.Write($"!板状态不明，不发信号, {pState}", LogType.ROBOT_STACK);
                        break;
                }
            } catch (Exception ex) {
                FrmMain.logOpt.Write($"!来源: {nameof(NotifyOpcJobFinished)}, {ex}", LogType.ROBOT_STACK);
            }
        }

        public void NotifyOpcJobFinished(RollPosition roll) {
            try {
                switch (roll.PnlState) {
                    case PanelState.HalfFull:
                        FrmMain.logOpt.Write($"{roll.RealLocation}: 半满板信号发出。slot: ", LogType.ROBOT_STACK);
                        break;
                    case PanelState.Full:
                       FrmMain.logOpt.Write($"{roll.RealLocation}: 满板信号发出。slot: ", LogType.ROBOT_STACK);
                        
                        LableCode.SetPanelFinished(roll.PanelNo);

                        TaskQueues.lochelper.OnFull(roll.RealLocation);
                        break;
                    case PanelState.LessHalf:
                        break;
                    default:
                        FrmMain.logOpt.Write($"!板状态不明，不发信号, {roll.PnlState}", LogType.ROBOT_STACK);
                        break;
                }
                if (roll.Status == (int)LableState.FloorLastRoll && roll.PnlState != PanelState.Full) {
                    BadShape(roll);
                }
            } catch (Exception ex) {
                FrmMain.logOpt.Write($"!来源: {nameof(NotifyOpcJobFinished)}, {ex}", LogType.ROBOT_STACK);
            }
        }

        private void BadShape(RollPosition roll) {
            var layerLabels = LableCode.GetLableCodesOfRecentFloor(roll.RealLocation, roll.PanelNo, roll.Floor);
            if (LayerShape.IsSlope(layerLabels) || LayerShape.IsVshape(layerLabels)) {
                FrmMain.logOpt.Write($"!{roll.RealLocation} 第{roll.Floor}层 形状不规则。板号{roll.PanelNo}", LogType.ROBOT_STACK);
            }
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
