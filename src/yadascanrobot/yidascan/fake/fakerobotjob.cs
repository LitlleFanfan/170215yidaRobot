using ProduceComm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using yidascan.DataAccess;
using yidascan.Properties;

namespace yidascan {
    /// <summary>
    /// 仅用于测试。
    /// </summary>
    class FakeRobotJob : IRobotJob, IDisposable {
        Action<string, string, LogViewType> loghandler;
        IOpcClient client;
        private OPCParam param;
        string ip;
        string jobName;
        private string JOB_NAME = "";

        public FakeRobotJob(string ip, string jobName) {
            this.ip = ip;
            this.jobName = jobName;
        }

        public void setup(Action<string, string, LogViewType> loghandler, IOpcClient client, OPCParam param) {
            this.loghandler = loghandler;
            this.client = client;
            this.param = param;
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

        private bool IsBusy() {
            return false;
        }

        private void log(string s, string group, LogViewType ltype = LogViewType.Both) {
            FrmMain.logOpt.Write(s, LogType.ROBOT_STACK);
        }

        private bool WritePositionPro(RollPosition rollPos) { return true; }

        public bool TryWritePositionPro(RollPosition rollPos, int times = 5) {
            const int DELAY = 100;
            int counts = 0;
            bool wstatus = false;
            var t = TimeCount.TimeIt(() => {
                while (times > 0) {
                    counts++;
                    wstatus = WritePositionPro(rollPos);
                    if (wstatus) { break; }
                    times--;
                    Thread.Sleep(DELAY);
                }
            });
            log($"机器人写坐标{(wstatus ? "成功" : "失败")}耗时{t}毫秒,尝试次数{counts}, {rollPos.LabelCode}, {rollPos.Pos_s()}。",
                LogType.ROBOT_STACK);
            return wstatus;
        }

        public bool IsRollOnPanel() { return true; }

        public bool TryRunJob(string jobName, int times = 10) { return true; }

        public void JobLoopPro(ref bool isrunning, TaskQueues taskq, Action onupdate) { }

        public bool JobTask(ref bool isrun, bool isSideA, Queue<RollPosition> robotRollQ, RollPosition roll, ListView lv) {
            // 等待板可放料
            // 等待板可放料
            if (PanelAvailable(roll.RealLocation)) {
                FrmMain.logOpt.Write($"{roll.RealLocation} PushInQueue收到可放料信号", LogType.ROBOT_STACK);
            } else {
                FrmMain.logOpt.Write($"! {roll.RealLocation} PushInQueue未收到可放料信号，请检查板状态和是否有形状不规则报警。", LogType.ROBOT_STACK);
                return false;
            }

            // 机器人正忙，等待。
            while (isrun) {
                if (IsBusy()) {
                    FrmMain.logOpt.Write($"!机器人正忙", LogType.ROBOT_STACK);
#if DEBUG
                    Thread.Sleep(FakeOpcClient.DELAY * 10);
#endif
                } else { break; }
            }

            if (!TryWritePositionPro(roll)) {
                return false;
            }

            if (TryRunJob(JOB_NAME)) {
                log($"发出机器人示教器动作{JOB_NAME}命令成功。", LogType.ROBOT_STACK);
                client.Write(param.RobotParam.RobotJobStart, true);
            } else {
                log($"!机器人示教器动作{JOB_NAME}发送失败。", LogType.ROBOT_STACK);
                return false;
            }
            Thread.Sleep(RobotHelper.DELAY * 200);
            log($"check roll is leaving from PLC: {roll.LabelCode}.", LogType.ROBOT_STACK, LogViewType.OnlyFile);
            //删除对列布卷
            var startTime = System.DateTime.Now;
            var now = System.DateTime.Now;
            var time = now - startTime;
            while (isrun) {
                var leaving = client.ReadBool(isSideA ? param.RobotParam.RobotStartA : param.RobotParam.RobotStartB);
                if (leaving) {
                    log($"roll is leaving: {roll.LabelCode}.", LogType.ROBOT_STACK, LogViewType.OnlyFile);
                    DequeueRoll(robotRollQ, roll, lv);
                    client.Write(isSideA ? param.RobotParam.RobotStartA : param.RobotParam.RobotStartB, false);
                    break;
                }
                //now = System.DateTime.Now;
                //time = now - startTime;
                //if (time.Milliseconds > RobotHelper.DELAY * 600) {//等leaving信号超时，等3秒
                //    break;
                //}
                Thread.Sleep(RobotHelper.DELAY);
            }

            //var sleeptime = RobotHelper.DELAY * 1000 - time.Milliseconds;
            //if (sleeptime > 0) {
            //    Thread.Sleep(sleeptime);
            //}

            // 等待布卷上垛信号
            while (isrun) {
                if (IsRollOnPanel()) {
                    // 写数据库。
                    LableCode.SetOnPanelState(roll.LabelCode);
                    // 告知OPC
                    NotifyOpcJobFinished(roll);
                    log("布卷已上垛。", LogType.ROBOT_STACK, LogViewType.Both);

                    break;
                }
                Thread.Sleep(RobotHelper.DELAY * 20);
            }

            // Thread.Sleep(RobotHelper.DELAY * 500);

            // 等待机器人结束码垛。
            while (isrun && IsBusy()) {
                Thread.Sleep(RobotHelper.DELAY * 20);
            }
            DequeueRoll(robotRollQ, roll, lv);
            if (!isrun) {//解决压布，布卷未上垛问题
                         // 写数据库。
                LableCode.SetOnPanelState(roll.LabelCode);
                // 告知OPC
                NotifyOpcJobFinished(roll);
                log($"{roll.LabelCode}布卷已上垛。", LogType.ROBOT_STACK, LogViewType.Both);
            }
            log($"robot job done: {roll.LabelCode}.", LogType.ROBOT_STACK);
            return true;
        }

        public void Dispose() {
            GC.SuppressFinalize(this);
        }

        public void NotifyOpcJobFinished(string panelNo, string tolocation, string reallocation) {
            try {
                var pState = LableCode.IsAllRollOnPanel(panelNo) ? PanelState.Full : PanelState.HalfFull;
                FrmMain.SetReallocationState(reallocation, pState);
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

                        lock (TaskQueues.LOCK_LOCHELPER) {
                            TaskQueues.lochelper.OnFull(reallocation);
                        }
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
                FrmMain.SetReallocationState(roll.RealLocation, roll.PnlState);
                switch (roll.PnlState) {
                    case PanelState.HalfFull:
                        FrmMain.logOpt.Write($"{roll.RealLocation}: 半满板信号发出。slot: ", LogType.ROBOT_STACK);
                        break;
                    case PanelState.Full:
                        FrmMain.logOpt.Write($"{roll.RealLocation}: 满板信号发出。slot: ", LogType.ROBOT_STACK);

                        LableCode.SetPanelFinished(roll.PanelNo);

                        lock (TaskQueues.LOCK_LOCHELPER) {
                            TaskQueues.lochelper.OnFull(roll.RealLocation);
                        }
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
            var layerLabels = LableCode.GetLableCodesOfRecentFloor(roll.ToLocation, roll.PanelNo, roll.Floor);
            if (LayerShape.IsSlope(layerLabels) || LayerShape.IsVshape(layerLabels)) {
                FrmMain.logOpt.Write($"!{roll.RealLocation} 第{roll.Floor}层 形状不规则。板号{roll.PanelNo}", LogType.ROBOT_STACK);
            }
        }

        public bool PanelAvailable(string tolocation) { return true; }

        private void DequeueRoll(Queue<RollPosition> robotRollQ, RollPosition roll, ListView lv) {
            // try {
                lock (robotRollQ) {
                    if (robotRollQ == null || robotRollQ.Count == 0) { return; }
                    
                    var roll2 = robotRollQ.Peek();
                    if (roll2 != null && roll.LabelCode == roll2.LabelCode) {//如果取出来还是原来那一个，就删一下
                        robotRollQ.Dequeue();
                        FrmMain.showRobotQue(robotRollQ, lv);
                    }
                }
            //} catch (Exception ex) {
            //    var msg = $"!{nameof(DequeueRoll)}: {roll.LabelCode}. {ex}";
            //    log(msg, LogType.ROBOT_STACK);
            //   //  throw new Exception(msg);
            //}
        }
    }
}
