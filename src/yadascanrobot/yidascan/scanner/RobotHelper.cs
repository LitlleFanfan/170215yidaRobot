using ProduceComm;
using ProduceComm.OPC;
using RobotControl;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using yidascan.DataAccess;
using System.Windows.Forms;

namespace yidascan {
    public enum PanelState {
        LessHalf,
        HalfFull,
        Full
    }

    class RobotPosOffset {
        public decimal offsetx { get; set; }
        public decimal offsety { get; set; }

        public RobotPosOffset(decimal ofx, decimal ofy) {
            offsetx = ofx;
            offsety = ofy;
        }
    }

    public class RollPosition {
        public const decimal Z_START = 0;

        public RollPosition() { }

        private static RobotPosOffset AdjustPosByRollLength(decimal x, decimal boardlen, decimal rolllen) {
            // 数据不合理，不做处理。
            if (rolllen > boardlen) {
                return new RobotPosOffset(0, 0);
            }

            // 确定偏移。(150毫米是不居中时，离板边上的距离)
            var offset = ((boardlen - rolllen) / 2) - 150;

            if (x == 0) {
                return new RobotPosOffset(offset, 0);
            } else {
                return new RobotPosOffset(0, offset);
            }
        }

        public RollPosition(LableCode label, string side, PanelState pnlState, decimal x, decimal y, decimal z, decimal rz) {
            this.LabelCode = label.LCode;
            PanelNo = label.PanelNo;
            Floor = label.Floor;
            Status = label.Status;

            // 布卷按长度调整板上位置， 以确定坐标偏移。
            var adj = AdjustPosByRollLength(x, clsSetting.SplintWidth, label.Length);

            //X = x;
            //Y = y;

            X = x + (side == "B" ? -adj.offsetx : adj.offsetx);
            Y = y + (side == "B" ? -adj.offsety : adj.offsety);

            Z = z;
            Rz = rz;
            //
            ChangeAngle = x > 0 || y < 0;
            ChangeAngle = robotChangeAngle.Contains(RealLocation) ? !ChangeAngle : ChangeAngle;//3\4\5\9\10\11标签朝外

            RealLocation = label.RealLocation;
            ToLocation = label.ToLocation;
            diameter = label.Diameter;

            // Index = CalculateBaseIndex(label.ToLocation, x, y);
            Index = CalculateBaseIndex(RealLocation, x, y);

            // LocationNo = int.Parse(label.ToLocation.Substring(1, 2));
            // LocationNo改为getLocationNo.

            // BaseIndex = 4 * (GetLocationNo() - 1) + Index + 1;
            BaseIndex = 4 * (GetRealLocationNo() - 1) + Index + 1;

            Side = side;
            PnlState = pnlState;

            // var tmp = int.Parse(label.ToLocation.Substring(1, 2));
            var tmp = GetRealLocationNo();

            var origin = RobotParam.GetOrigin(tmp);
            var point = RobotParam.GetPoint(tmp, Index);

            Base1 = origin.Base;
            Base2 = point.Base;
            XOffSet = GetXOffSet(origin.Base, point.Base);
            X = X + XOffSet;
            Origin = new PostionVar(XOffSet, 0, 1700, origin.Rx, origin.Ry, origin.Rz + rz);
            Target = new PostionVar(X, Y, Z, origin.Rx, origin.Ry, origin.Rz + rz);
        }

        // 获取交地序号。
        //public int GetLocationNo() {
        //    return int.Parse(ToLocation.Substring(1, 2));
        //}

        public int GetRealLocationNo() {
            return int.Parse(RealLocation.Substring(1, 2));
        }

        private static decimal GetXOffSet(decimal originBase, decimal targetBase) {
            return ((targetBase - originBase) * 2 * -1);
        }

        public static decimal GetToolOffSet(decimal xory) {
            decimal toolLen = 250;
            if (xory >= 0) {
                return xory + toolLen;
            } else {
                return xory - toolLen;
            }
        }


        public static List<string> robotRSidePanel = new List<string> { "B03", "B04", "B05", "B06", "B07", "B08" };
        public static List<string> robotChangeAngle = new List<string> { "B03", "B04", "B05", "B09", "B10", "B11" };
        private static int CalculateBaseIndex(string tolocation, decimal x, decimal y) {
            var baseindex = 0;
            if (x != 0) {
                baseindex = 2;
                if (x > 0) {
                    baseindex += 1;
                }
            } else {
                if (robotRSidePanel.Contains(tolocation) && y > 0) {
                    baseindex += 1;
                } else if (!robotRSidePanel.Contains(tolocation) && y < 0) {
                    baseindex += 1;
                }
            }

            return baseindex;
        }

        public bool IsSameLabel(RollPosition roll) {
            return this.LabelCode == roll.LabelCode;
        }

        /// <summary>
        /// 返回坐标的字符串表示。
        /// </summary>
        /// <returns></returns>
        public string Pos_s() {
            return $"x: {X}, y: {Y}, z: {Z}, rz: {Rz}";
        }

        public string LabelCode;

        public string PanelNo;
        public int Floor;
        public int Status;

        // public int LocationNo;
        // 改为getLocationNo();

        public int Index;
        public int BaseIndex;
        public bool ChangeAngle;

        public PostionVar Target;
        public PostionVar Origin;

        public decimal X;
        public decimal Y;
        public decimal Z;
        public decimal Rz;
        public decimal XOffSet;
        public decimal Base1;
        public decimal Base2;

        public string RealLocation { get; set; }
        public string ToLocation { get; set; }
        public decimal diameter { get; set; }
        // A侧或B侧
        public string Side { get; set; }
        public PanelState PnlState { get; set; }

        public string brief() {
            return $"{LabelCode} {ToLocation}/{RealLocation} {diameter}";
        }
    }

    public class RobotHelper : IRobotJob, IDisposable {
        private string JOB_NAME = "";

        RobotControl.RobotControl rCtrl;
        IErpApi erpapi;

        public const int DELAY = 5;

        public Action<string, string, LogViewType> _log;

        private IOpcClient client;
        private OPCParam param;

        public RobotHelper(IErpApi _erpapi, string ip, string jobName) {
            try {
                erpapi = _erpapi;
                rCtrl = new RobotControl.RobotControl(ip);
                rCtrl.Connect();
                rCtrl.ServoPower(true);
                JOB_NAME = jobName;
            } catch (Exception ex) {
                clsSetting.loger.Error(ex);
            }
        }

        public void setup(Action<string, string, LogViewType> logfunc, IOpcClient c, OPCParam p) {
            _log = logfunc;
            client = c;
            param = p;
        }

        private void log(string msg, string group, LogViewType ltype = LogViewType.Both) {
            if (_log == null) { return; }
            _log(msg, group, ltype);
        }

        public bool IsConnected() {
            return rCtrl.IsConnected();
        }

        /// <summary>
        /// 尝试多次机器人写坐标，直到成功或用完次数。
        /// </summary>
        /// <param name="rollPos"></param>
        /// <param name="times"></param>
        /// <returns></returns>
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

        private bool WritePositionPro(RollPosition rollPos) {
            const int DELAY = 10;
            var a = rCtrl.SetVariables(RobotControl.VariableType.B, 10, 1, rollPos.ChangeAngle ? "1" : "0");
            Thread.Sleep(DELAY);
            var b = rCtrl.SetVariables(RobotControl.VariableType.B, 0, 1, rollPos.BaseIndex.ToString());
            Thread.Sleep(DELAY);
            var c = rCtrl.SetVariables(RobotControl.VariableType.B, 5, 1, "1");
            Thread.Sleep(DELAY);

            // 原点高位旋转
            var d = rCtrl.SetPostion(RobotControl.PosVarType.Robot,
                rollPos.Origin, 100, RobotControl.PosType.User, 0, rollPos.GetRealLocationNo());
            Thread.Sleep(DELAY);

            //基座
            var e = rCtrl.SetPostion(RobotControl.PosVarType.Base,
                new RobotControl.PostionVar(rollPos.Base2 * 1000, 0, 0, 0, 0),
                0, RobotControl.PosType.Robot, 0, 0);

            Thread.Sleep(DELAY);

            // 目标位置
            var f = rCtrl.SetPostion(RobotControl.PosVarType.Robot,
               rollPos.Target, 101, RobotControl.PosType.User, 0, rollPos.GetRealLocationNo());
            Thread.Sleep(DELAY);

            return a && b && c && d && e && f;
        }

        /// <summary>
        /// 发出执行示教程序的指令
        /// 如果失败，则重复执行
        /// </summary>
        /// <param name="jobName">示教程序名， 默认10次</param>
        /// <param name="times">尝试次数</param>
        /// <returns></returns>
        public bool TryRunJob(string jobName, int times = 10) {
            // 新写法, 需要测试。2017-02-20
            return Helper.retry(() => {
                return rCtrl.StartJob(jobName);
            }, times, 80);
        }

        public bool IsBusy() {
            // 读机器人的状态可能有错。
            try {
                var status = rCtrl.GetPlayStatus();
                log($"robot status: {JsonConvert.SerializeObject(status)}", LogType.ROBOT_STACK, LogViewType.OnlyFile);

                if (status == null || status.Count == 0) {
                    return true;
                } else {
                    return (status["Start"] || status["Hold"]);
                }
            } catch (Exception ex) {
                log($"来源: {nameof(IsBusy)}, {ex}", LogType.ROBOT_STACK, LogViewType.OnlyFile);
                return true;
            }
        }

        public void NotifyOpcJobFinished(string panelNo, string tolocation, string reallocation) {
            try {
                var pState = LableCode.IsAllRollOnPanel(panelNo) ? PanelState.Full : PanelState.HalfFull;
                switch (pState) {
                    case PanelState.HalfFull:
                        client.Write(param.BAreaFloorFinish[reallocation], true);

                        var lcode = FrmMain.taskQ.UFGetPanelLastRoll(tolocation, panelNo);
                        LableCode.UserSetPanelLastRoll(lcode);//设置板最后一卷布。
                        log($"{reallocation} 半板信号发出,最后一卷布标签{lcode}。slot: {param.BAreaFloorFinish[reallocation]}", LogType.ROBOT_STACK);
                        break;
                    case PanelState.Full:
                        string msg;
                        ErpHelper.NotifyPanelEnd(erpapi, panelNo, out msg);
                        client.Write(param.BAreaPanelFinish[reallocation], true);
                        log($"{reallocation}: 满板信号发出。slot: {param.BAreaPanelFinish[reallocation]}", LogType.ROBOT_STACK);
                        log(msg, LogType.ROBOT_STACK);

                        // 满板时设置自由板位标志。
                        TaskQueues.lochelper.OnFull(reallocation);

                        const int SIGNAL_3 = 3;
                        client.Write(param.BAreaPanelState[reallocation], SIGNAL_3);
                        log($"{reallocation}: 板状态信号发出，状态值: {SIGNAL_3}。slot: {param.BAreaPanelState[reallocation]}", LogType.ROBOT_STACK);
                        break;
                    case PanelState.LessHalf:
                        break;
                    default:
                        log($"!板状态不明，不发信号, {pState}", LogType.ROBOT_STACK);
                        break;
                }
            } catch (Exception ex) {
                log($"!来源: {nameof(NotifyOpcJobFinished)}, {ex}", LogType.ROBOT_STACK);
            }
        }

        public void NotifyOpcJobFinished(RollPosition roll) {
            try {
                switch (roll.PnlState) {
                    case PanelState.HalfFull:
                        client.Write(param.BAreaFloorFinish[roll.RealLocation], true);
                        log($"{roll.RealLocation}: 半板信号发出。slot: {param.BAreaFloorFinish[roll.RealLocation]}", LogType.ROBOT_STACK);
                        break;
                    case PanelState.Full:
                        string msg;
                        ErpHelper.NotifyPanelEnd(erpapi, roll.PanelNo, out msg);
                        client.Write(param.BAreaPanelFinish[roll.RealLocation], true);
                        log($"{roll.RealLocation}: 满板信号发出。slot: {param.BAreaPanelFinish[roll.RealLocation]}", LogType.ROBOT_STACK);
                        log(msg, LogType.ROBOT_STACK);
                        LableCode.SetPanelFinished(roll.PanelNo);

                        TaskQueues.lochelper.OnFull(roll.RealLocation);

                        const int SIGNAL_3 = 3;
                        client.Write(param.BAreaPanelState[roll.RealLocation], SIGNAL_3);
                        log($"{roll.RealLocation}: 板状态信号发出，状态值: {SIGNAL_3}。slot: {param.BAreaPanelState[roll.RealLocation]}", LogType.ROBOT_STACK);
                        break;
                    case PanelState.LessHalf:
                        break;
                    default:
                        log($"!板状态不明，不发信号, {roll.PnlState}", LogType.ROBOT_STACK);
                        break;
                }
                if (roll.Status == (int)LableState.FloorLastRoll && roll.PnlState != PanelState.Full) {
                    BadShape(roll);
                }
            } catch (Exception ex) {
                log($"!来源: {nameof(NotifyOpcJobFinished)}, {ex}", LogType.ROBOT_STACK);
            }
        }

        private void BadShape(RollPosition roll) {
            var layerLabels = LableCode.GetLableCodesOfRecentFloor(roll.RealLocation, roll.PanelNo, roll.Floor);
            if (LayerShape.IsSlope(layerLabels) || LayerShape.IsVshape(layerLabels)) {
                PlcHelper.NotifyBadLayerShape(client, param, roll.RealLocation);
                log($"!{roll.RealLocation} 第{roll.Floor}层 形状不规则。板号{roll.PanelNo}", LogType.ROBOT_STACK);
            }
        }

        public void JobLoopPro(ref bool isrunning, TaskQueues taskq, Action onupdate) {
            _log.Invoke($"enter loop. isrunning: {isrunning}", LogType.ROBOT_STACK, LogViewType.OnlyForm);

            while (isrunning) {
                _log.Invoke("move queue.", LogType.ROBOT_STACK, LogViewType.OnlyForm);

                var ques = new List<Queue<RollPosition>> { taskq.RobotRollAQ, taskq.RobotRollBQ };

                foreach (var qu in ques) {
                    if (qu.Count() > 0) {
                        var item = qu.Peek();
                        //if (item != null && JobTask(ref isrunning, item)) {
                        //    qu.Dequeue();
                        //}
                    }
                }
                onupdate();

                Thread.Sleep(500);
            }
        }

        [Obsolete("use JobLoopPro instead.")]
        public void JobLoop(ref bool isrun, ListView la, ListView lb) {
            while (isrun) {
                if (FrmMain.taskQ.RobotRollAQ.Count > 0) {
                    var roll = FrmMain.taskQ.RobotRollAQ.Peek();
                    if (roll != null) {
                        JobTask(ref isrun, true, FrmMain.taskQ.RobotRollAQ, roll, la);
                    }
                }
                if (FrmMain.taskQ.RobotRollBQ.Count > 0) {
                    var roll = FrmMain.taskQ.RobotRollBQ.Peek();
                    if (roll != null) {
                        JobTask(ref isrun, false, FrmMain.taskQ.RobotRollBQ, roll, lb);
                    }
                }
                Thread.Sleep(RobotHelper.DELAY * 40);
            }
        }

        private void DequeueRoll(Queue<RollPosition> robotRollQ, RollPosition roll, ListView lv) {
            try {
                if (robotRollQ == null) { return; }

                var roll2 = robotRollQ.Peek();
                if (roll2 != null && roll.LabelCode == roll2.LabelCode) {//如果取出来还是原来那一个，就删一下
                    robotRollQ.Dequeue();
                    FrmMain.showRobotQue(robotRollQ, lv);
                    log($"robot Dequeue roll: {roll.LabelCode}.", LogType.ROBOT_STACK, LogViewType.OnlyFile);
                }
            } catch (Exception ex) {
                log($"来源: {nameof(DequeueRoll)}, robot Dequeue roll: {roll.LabelCode}. {ex}", LogType.ROBOT_STACK, LogViewType.OnlyFile);
            }
        }

        public bool JobTask(ref bool isrun, bool isSideA, Queue<RollPosition> robotRollQ, RollPosition roll, ListView lv) {
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
                    Thread.Sleep(OPCClient.DELAY * 10);
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
                now = System.DateTime.Now;
                time = now - startTime;
                if (time.Milliseconds > RobotHelper.DELAY * 600) {//等leaving信号超时，等3秒
                    break;
                }
                Thread.Sleep(RobotHelper.DELAY);
            }

            var sleeptime = RobotHelper.DELAY * 1000 - time.Milliseconds;
            if (sleeptime > 0) {
                Thread.Sleep(sleeptime);
            }

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

            Thread.Sleep(RobotHelper.DELAY * 500);

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
                log("布卷已上垛。", LogType.ROBOT_STACK, LogViewType.Both);
            }
            log($"robot job done: {roll.LabelCode}.", LogType.ROBOT_STACK);
            return true;
        }

        private bool IsRollOnPanel() {
            const string KEY = "5";
            const string V_ON_PANEL = "0";
            try {
                var b5 = rCtrl.GetVariables(VariableType.B, 5, 1);
                return (b5 != null && b5.ContainsKey(KEY) && b5[KEY] == V_ON_PANEL);
            } catch (Exception ex) {
                log($"!来源: nameof(IsRollOnPanel): {ex}", LogType.ROBOT_STACK, LogViewType.OnlyFile);
                return false;
            }
        }

        public bool PanelAvailable(string tolocation) {
#if DEBUG
            // 用于测试
            return true;
#endif
            try {
                var s = client.ReadString(param.BAreaPanelState[tolocation]);
                var canput = !client.ReadBool(param.BadShapeLocations[tolocation]);
                log($"! {tolocation} 可放料信号板状态{s} 未报警{canput}", LogType.ROBOT_STACK, LogViewType.OnlyFile);
                return s == "2" && canput;
            } catch (Exception ex) {
                log($"!来源: {nameof(PanelAvailable)}, 读可放料信号异常 tolocation: {tolocation} opc:{param.BadShapeLocations[tolocation]} err:{ex}", LogType.ROBOT_STACK);
                return false;//临时
            }
        }

        [Obsolete]
        public Dictionary<string, string> AlarmTask() {
            try {
                var s = rCtrl.GetAlarmStatus();
                if (s != null && s.Count != 0) {
                    if (s["Error"] || s["Alarm"]) {
                        return rCtrl.GetAlarmCode();
                    }
                }
            } catch (Exception ex) {
                log($"!{ex}", LogType.ROBOT_STACK);
            }
            return new Dictionary<string, string>();
        }

        public void Dispose() {
            rCtrl.ServoPower(false);
            Thread.Sleep(1000);
            rCtrl.Close();
            GC.SuppressFinalize(this);
        }
    }
}
