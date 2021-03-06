﻿using ProduceComm;
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
using System.Net.Sockets;

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
            FloorIndex = label.FloorIndex;

            // 布卷按长度调整板上位置， 以确定坐标偏移。
            var adj = AdjustPosByRollLength(x, clsSetting.SplintWidth, label.Length);

            //X = x;
            //Y = y;

            X = x + (side == "B" ? -adj.offsetx : adj.offsetx);
            Y = y + (side == "B" ? -adj.offsety : adj.offsety);

            Z = z;
            Rz = rz;
            //
            if (robotChangeAngle.Contains(RealLocation)) {
                ChangeAngle = x < 0 || y > 0;//3\4\5\9\10\11标签朝外
            } else {
                ChangeAngle = x > 0 || y < 0;
            }

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
        public int FloorIndex;

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
            return $"{LabelCode} {ToLocation}/{RealLocation} {diameter} F{Floor}/{FloorIndex}";
        }

        public string detail() {
            return $"{LabelCode} {ToLocation}/{RealLocation} {diameter} F{Floor}/{FloorIndex} panel/{PanelNo}";
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

        public Action<bool, string> onerror;

        public RobotHelper(IErpApi _erpapi, string ip, string jobName) {
            try {
                erpapi = _erpapi;
                rCtrl = new RobotControl.RobotControl(ip, "11000");
                rCtrl.Connect();
                rCtrl.ServoPower(true);
                JOB_NAME = jobName;
            } catch (Exception ex) {
                clsSetting.loger.Error(ex);
            }
        }

        public void setup(Action<bool, string> errorhandler, Action<string, string, LogViewType> logfunc, IOpcClient c, OPCParam p) {
            onerror = errorhandler;
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
            var counts = 0;
            var wstatus = false;
            var t = TimeCount.TimeIt(() => {
                while (times > 0) {
                    counts++;
                    wstatus = WritePositionPro(rollPos);
                    if (wstatus) { break; }
                    times--;
                    Thread.Sleep(DELAY);
                }
            });
            log($"机器人写坐标{(wstatus ? "成功" : "失败")}耗时{t}毫秒,尝试次数{counts}, {rollPos.LabelCode}, {rollPos.Pos_s()} ChangeAngle {rollPos.ChangeAngle}。",
                LogType.ROBOT_STACK);
            return wstatus;
        }

        private bool WritePositionPro(RollPosition rollPos) {
            const int DELAY = 10;
            // 写变量之前，先设置完成状态为0。
            var writeReady = rCtrl.SetVariables(VariableType.B, 1, 1, "0");

            var a = rCtrl.SetVariables(VariableType.B, 10, 1, rollPos.ChangeAngle ? "1" : "0");
            Thread.Sleep(DELAY);
            var b = rCtrl.SetVariables(VariableType.B, 0, 1, rollPos.BaseIndex.ToString());
            Thread.Sleep(DELAY);
            var c = rCtrl.SetVariables(VariableType.B, 5, 1, "1"); //?            
            Thread.Sleep(DELAY);

            // 原点高位旋转
            var d = rCtrl.SetPostion(PosVarType.Robot,
                rollPos.Origin, 100, PosType.User, 0, rollPos.GetRealLocationNo());
            Thread.Sleep(DELAY);

            //基座
            var e = rCtrl.SetPostion(PosVarType.Base,
                new PostionVar(rollPos.Base2 * 1000, 0, 0, 0, 0),
                0, PosType.Robot, 0, 0);

            Thread.Sleep(DELAY);

            // 目标位置
            var f = rCtrl.SetPostion(PosVarType.Robot,
               rollPos.Target, 101, PosType.User, 0, rollPos.GetRealLocationNo());
            Thread.Sleep(DELAY);

            // 写变量之后，设置完成状态为1.
            var writeOk = rCtrl.SetVariables(VariableType.B, 1, 1, "1");

            return a && b && c && d && e && f && writeReady && writeOk;
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

        public bool TryIsBusy() {
            var times = 5;
            while (times-- > 0) {
                try {
                    return rCtrl.IsBusy();
                } catch (SocketException ex) {
                    log($"来源: {nameof(TryIsBusy)}, {ex}", LogType.ROBOT_STACK, LogViewType.OnlyFile);
                    var b = rCtrl.TryReconnect();
                    if (!b) { onerror?.Invoke(b, "机器人网络故障..."); }
                    log($"来源： {nameof(TryIsBusy)}, 尝试重新建立机器人连接, 连接状态: {b}。", LogType.ROBOT_STACK);
                } catch (Exception ex) {
                    log($"来源: {nameof(TryIsBusy)}, {ex}", LogType.ROBOT_STACK);
                }

                Thread.Sleep(DELAY * 20); // 100ms.
            }
            return false;
        }

        public void NotifyOpcJobFinished(string panelNo, string tolocation, string reallocation, bool panelfull) {
            try {
                var pState = panelfull ? PanelState.Full : PanelState.HalfFull;
                switch (pState) {
                    case PanelState.HalfFull:
                        var lcode = FrmMain.taskQ.UFGetPanelLastRoll(tolocation, panelNo);
                        LableCode.UserSetPanelLastRoll(lcode);//设置板最后一卷布。

                        log($"{reallocation} 半板信号发出,最后一卷布标签{lcode}。slot: {param.BAreaFloorFinish[reallocation]}", LogType.ROBOT_STACK);
                        client.TryWrite(param.BAreaFloorFinish[reallocation], true);
                        break;
                    case PanelState.Full:
                        string msg;
                        ErpHelper.NotifyPanelEnd(erpapi, panelNo, reallocation, out msg);
                        log(msg, LogType.ROBOT_STACK);

                        LableCode.SetPanelFinished(panelNo);

                        // 满板时设置自由板位标志。
                        lock (TaskQueues.LOCK_LOCHELPER) {
                            TaskQueues.lochelper.OnFull(reallocation);
                        }

                        lock (client) {
                            PlcHelper.NotifyFullPanel(client, param, reallocation);
                        }

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
                        lock (client) {
                            client.TryWrite(param.BAreaFloorFinish[roll.RealLocation], true);
                        }
                        log($"{roll.RealLocation}: 半板信号发出。slot: {param.BAreaFloorFinish[roll.RealLocation]}", LogType.ROBOT_STACK);
                        break;
                    case PanelState.Full:
                        string msg;
                        ErpHelper.NotifyPanelEnd(erpapi, roll.PanelNo, roll.RealLocation, out msg);
                        log(msg, LogType.ROBOT_STACK);

                        LableCode.SetPanelFinished(roll.PanelNo);

                        lock (TaskQueues.LOCK_LOCHELPER) {
                            TaskQueues.lochelper.OnFull(roll.RealLocation);
                        }

                        lock (client) {
                            PlcHelper.NotifyFullPanel(client, param, roll.RealLocation);
                        }

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

                var panel = LableCode.GetPanel(roll.PanelNo);
                if (roll.Status == (int)LableState.FloorLastRoll && roll.PnlState != PanelState.Full && roll.Floor == panel.MaxFloor) {
                    log("!---异常板满状态处理---", LogType.ROBOT_STACK);
                    roll.PnlState = PanelState.Full;
                    string msg;
                    ErpHelper.NotifyPanelEnd(erpapi, roll.PanelNo, roll.RealLocation, out msg);
                    client.TryWrite(param.BAreaPanelFinish[roll.RealLocation], true);

                    log($"{roll.RealLocation}: 异常满板信号发出。slot: {param.BAreaPanelFinish[roll.RealLocation]}", LogType.ROBOT_STACK);
                    log(msg, LogType.ROBOT_STACK);

                    LableCode.SetPanelFinished(roll.PanelNo);

                    lock (TaskQueues.LOCK_LOCHELPER) {
                        TaskQueues.lochelper.OnFull(roll.RealLocation);
                    }

                    const int SIGNAL_3 = 3;
                    client.TryWrite(param.BAreaPanelState[roll.RealLocation], SIGNAL_3);
                    log($"{roll.RealLocation}: 异常板状态信号发出，状态值: {SIGNAL_3}。slot: {param.BAreaPanelState[roll.RealLocation]}", LogType.ROBOT_STACK);
                }

            } catch (Exception ex) {
                log($"!来源: {nameof(NotifyOpcJobFinished)}, {ex}", LogType.ROBOT_STACK);
            }
        }

        private void BadShape(RollPosition roll) {
            var layerLabels = LableCode.GetLableCodesOfRecentFloor(roll.ToLocation, roll.PanelNo, roll.Floor);
            if (LayerShape.IsSlope(layerLabels) || LayerShape.IsVshape(layerLabels)) {
                lock (client) {
                    PlcHelper.NotifyBadLayerShape(client, param, roll.RealLocation);
                }
                
                log($"!{roll.RealLocation}/{roll.Floor}形状不规则。板号{roll.PanelNo}", LogType.ROBOT_STACK);
            }
        }

        private void runtask(Queue<RollPosition> que, bool sideA, ref bool isrunning, ListView view) {
            RollPosition roll = null;
            lock (TaskQueues.LOCK_LOCHELPER) {
                if (que.Count > 0) {
                    roll = que.Peek();
                }
            }

            if (roll != null) {
                JobTask(ref isrunning, sideA, que, roll, view);
            }
        }

        public void JobLoop(ref bool isrunning, ListView la, ListView lb) {
            log("robot jobloop启动。", LogType.ROBOT_STACK);
            while (isrunning) {
                var toSidea = true;

                runtask(FrmMain.taskQ.RobotRollAQ, toSidea, ref isrunning, la);
                Thread.Sleep(RobotHelper.DELAY * 20);

                runtask(FrmMain.taskQ.RobotRollBQ, !toSidea, ref isrunning, lb);
                Thread.Sleep(RobotHelper.DELAY * 20);
            }
            log("robot jobloop结束。", LogType.ROBOT_STACK);
        }

        private void DequeueRoll(Queue<RollPosition> robotRollQ, RollPosition roll, ListView lv) {
            try {
                lock (robotRollQ) {
                    if (robotRollQ == null || robotRollQ.Count == 0) { return; }

                    var roll2 = robotRollQ.Peek();

                    if (roll2 != null && roll.LabelCode == roll2.LabelCode) {//如果取出来还是原来那一个，就删一下
                        robotRollQ.Dequeue();
                        log($"号码出机器人队列: {roll.LabelCode}.", LogType.ROBOT_STACK);
                    }

                    if (roll2 == null) {
                        log($"!来源: {nameof(DequeueRoll)}, 机器人队列中无此号码: {roll.LabelCode}", LogType.ROBOT_STACK);
                    }
                }
                FrmMain.showRobotQue(robotRollQ, lv);
            } catch (Exception ex) {
                log($"!来源: {nameof(DequeueRoll)}, {roll.LabelCode}. {ex}", LogType.ROBOT_STACK, LogViewType.OnlyFile);
            }
        }

        public bool JobTask(ref bool isrun, bool isSideA, Queue<RollPosition> robotRollQ, RollPosition roll, ListView lv) {
            // 等待板可放料     
            FrmMain.logOpt.Write($"{roll.RealLocation}等待可放料信号。", LogType.ROBOT_STACK);

            if (!PanelAvailable(roll.RealLocation)) {
                FrmMain.logOpt.Write($"!{roll.RealLocation}未收到可放料信号，请检查板状态和是否有形状不规则报警。", LogType.ROBOT_STACK);
                return false;
            } else {
                FrmMain.logOpt.Write($"{roll.RealLocation}收到可放料信号。", LogType.ROBOT_STACK);
            }

            // 机器人正忙，等待。
            while (isrun) {
                if (TryIsBusy()) {
                    onerror?.Invoke(true, "机器人忙");
                    FrmMain.logOpt.Write($"!机器人正忙", LogType.ROBOT_STACK);
                    Thread.Sleep(OPCClient.DELAY * 10);
                } else { break; }
            }

            onerror?.Invoke(true, "机器人准备好");

            var writeok = TryWritePositionPro(roll);
            if (!writeok) {
                onerror?.Invoke(false, $"发坐标失败");
                log($"!{roll.LabelCode}发送坐标失败。", LogType.ROBOT_STACK);
                return false;
            }

            if (TryRunJob(JOB_NAME)) {
                onerror?.Invoke(true, $"{JOB_NAME}动作发送完成");
                log($"发出机器人示教器动作{JOB_NAME}命令成功, {roll.LabelCode}", LogType.ROBOT_STACK);
                lock (client) {
                    client.TryWrite(param.RobotParam.RobotJobStart, true);
                }
            } else {
                onerror?.Invoke(false, "动作发送失败");
                log($"!机器人示教器动作{JOB_NAME}发送失败, {roll.LabelCode}", LogType.ROBOT_STACK);
                return false;
            }

            Thread.Sleep(RobotHelper.DELAY * 100); // 500ms.

            while (isrun) {
                var side = isSideA ? "A" : "B";
                onerror?.Invoke(true, $"等待抓料信号({side})   ");

                var leaving = false;
                lock (client) {
                    leaving = isSideA ? param.RobotParam.PlcSnA.ReadSN(client) : param.RobotParam.PlcSnB.ReadSN(client);
                }

                if (!leaving) {
                    onerror?.Invoke(true, $"等待抓料信号({side})...");
                }

                if (leaving) {
                    onerror?.Invoke(true, $"{roll.LabelCode}抓起");
                    log($"布卷抓起: {roll.brief()}.", LogType.ROBOT_STACK);

                    DequeueRoll(robotRollQ, roll, lv); // 出队列
                    LableCode.SetOnPanelState(roll.LabelCode); // 写数据库。                    

                    var sideslot = isSideA ? param.RobotParam.RobotStartA : param.RobotParam.RobotStartB;
                    lock (client) {
                        client.TryWrite(sideslot, false);
                        if (isSideA) {
                            param.RobotParam.PlcSnA.WriteSN(client);
                        } else {
                            param.RobotParam.PlcSnB.WriteSN(client);
                        }
                    }
                    break;
                }

                Thread.Sleep(RobotHelper.DELAY * 200); // 1000ms.
            }

            // 等待布卷上垛信号
            while (isrun) {
                if (TryIsRollOnPanel()) {
                    NotifyOpcJobFinished(roll); // 告知OPC
                    onerror?.Invoke(true, $"{roll.LabelCode}上垛");
                    log($"收到布卷{roll.LabelCode}上垛信号，布卷已上垛，实际交地：{roll.RealLocation}。", LogType.ROBOT_STACK);

                    break;
                }
                Thread.Sleep(RobotHelper.DELAY * 20);
            }

            Thread.Sleep(RobotHelper.DELAY * 400);
            return true;
        }

        private bool IsRollOnPanel() {
            const string KEY = "5";
            const string V_ON_PANEL = "0";

            var b5 = rCtrl.GetVariables(VariableType.B, 5, 1,
                (x) => { log(x, LogType.ROBOT_STACK); });

            return (b5 != null && b5.ContainsKey(KEY) && b5[KEY] == V_ON_PANEL);
        }

        private bool TryIsRollOnPanel() {
            var times = 5;
            while (times-- > 0) {
                try {
                    return IsRollOnPanel();
                } catch (SocketException ex) {
                    onerror?.Invoke(false, "机器人连接失败");
                    log($"!来源: {nameof(TryIsRollOnPanel)}, {ex}", LogType.ROBOT_STACK);
                    var b = rCtrl.TryReconnect();
                    onerror?.Invoke(b, b ? "机器人准备好" : "机器人尝试连接失败");
                    log($"!来源： {nameof(TryIsRollOnPanel)}, 尝试重新建立机器人连接, 连接状态: {b}。", LogType.ROBOT_STACK);
                } catch (Exception ex) {
                    log($"!来源: {nameof(TryIsRollOnPanel)}, {ex}", LogType.ROBOT_STACK);
                }

                Thread.Sleep(DELAY * 20); // 100ms.
            }
            return false;
        }

        public bool PanelAvailable(string realloc) {
#if DEBUG
            // 用于测试
            return true;
#endif
            try {
                var s = "";
                var canput = false;
                lock (client) {
                    s = client.TryReadString(param.BAreaPanelState[realloc]);
                    canput = !client.TryReadBool(param.BadShapeLocations[realloc]);
                }

                if (!canput) {
                    onerror?.Invoke(false, $"{realloc}形状坏");
                }

                var layershape = canput ? "正常" : "坏型";
                if (s != "3") {
                    log($"实际交地: {realloc}, 可放料信号板状态: {s}, 层形状状态: {layershape}", LogType.ROBOT_STACK);
                }

                return s == "2" && canput;
            } catch (Exception ex) {
                log($"!来源: {nameof(PanelAvailable)}, 实际交地: {realloc}, 读可放料信号异常: {ex}", LogType.ROBOT_STACK);
                return false;//临时
            }
        }

        public void Dispose() {
            rCtrl.ServoPower(false);
            Thread.Sleep(1000);
            rCtrl.Close();
            GC.SuppressFinalize(this);
        }

        private static int LocOrder(string loc) {
            return 30 + int.Parse(loc.Substring(1));
        }
    }
}

