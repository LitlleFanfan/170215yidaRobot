using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProduceComm;
using ProduceComm.OPC;
using yidascan.DataAccess;

namespace yidascan {
    class RobotJobTask {
        private static OPCClient client;
        private static OPCParam param;

        private static Action<string, string, LogViewType> _log;

        private static bool isrun;
        private static int zstart = 0;

        public static void setup(OPCClient c, OPCParam p, Action<string, string, LogViewType> logfunc) {
            client = c;
            param = p;
            _log = logfunc;
        }

        private static void log(string msg, string group, LogViewType showtype = LogViewType.Both) {
            if (_log == null) { return; }
            _log(msg, group, showtype);
        }

        /// <summary>
        /// slot is param.RobotCarryA or param.RobotCarryB
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="jobname">"A" or "B"</param>
        private static void startJobTask(LCodeSignal slot, string jobname) {
            isrun = true;
            while (isrun) {
                // 两个任务共用一个client，故要加锁。
                lock (client) {
                    if (client.ReadBool(slot.Signal)) {
                        // 加入机器人布卷队列。
                        var code1 = client.Read(slot.LCode1).ToString();
                        var code2 = client.Read(slot.LCode2).ToString();
                        var fullcode = LableCode.MakeCode(code1, code2);

                        pushInQueue(fullcode, jobname);

                        client.Write(slot.Signal, false);
                    }
                }
                Thread.Sleep(5000);
            }
        }

        public static void StartJobTaskA() {
            Task.Run(() => {
                startJobTask(param.RobotCarryA, "A");
            });
        }

        public static void StartJobTaskB() {
            Task.Run(() => {
                startJobTask(param.RobotCarryB, "B");
            });
        }

        public static void StopJobTask() {
            isrun = false;
            Thread.Sleep(1000);
        }

        private static void pushInQueue(string fullcode, string side) {
            var label = LableCode.QueryByLCode(fullcode);
            if (label == null) {
                log($"!{side} {fullcode}找不到", LogType.ROLL_QUEUE);
                return;
            }
            if (label.Status >= (int)LableState.OnPanel) {
                log($"!{side} {label.LCode}已在板上,未加入队列,交地{label.ToLocation}.", LogType.ROLL_QUEUE);
                return;
            }
            if (label.CoordinatesIsEmpty()) {
                log("!{side} {label.LCode}未算位置，未加入队列,交地{label.ToLocation}.", LogType.ROLL_QUEUE);
                return;
            }

            try {
                log($"PushInQueue等可放料信号, 交地: {label.ToLocation}", LogType.ROBOT_STACK);
                while (isrun) {
                    if (PanelAvailable(label.ToLocation)) {
                        log($"PushInQueue收到可放料信号, 交地: {label.ToLocation}", LogType.ROBOT_STACK);
                        break;
                    }
                    Thread.Sleep(OPCClient.DELAY * 200);
                }

            } catch (Exception ex) {
                log($"!PushInQueue等可放料信号异常: { ex.ToString()}", LogType.ROBOT_STACK);
            }

            var pinfo = LableCode.GetPanel(label.PanelNo);
            var state = GetPanelState(label, pinfo);
            var statename = Enum.GetName(typeof(PanelState), state);
            log($"{label.LCode} {label.ToLocation} {statename}", LogType.ROLL_QUEUE, LogViewType.OnlyFile);

            var x = label.Cx;
            var y = label.Cy;
            var z = label.Cz + zstart;
            var rz = label.Crz;

            if (rz == 0) {
                y = RollPosition.GetToolOffSet(y);
            } else {
                x = RollPosition.GetToolOffSet(x);
            }

            if (x + y > 0) {
                if (rz == 0) {
                    rz = -180;
                }
            } else {
                if (rz != 0) {
                    rz = rz * -1;
                }
            }
            if (RollPosition.robotRSidePanel.Contains(label.ToLocation)) {
                rz = rz + 180;
            }
            var roll = new RollPosition(fullcode, side, label.ToLocation, state, x, y, z, rz);
            var success = RobotHelper.robotJobs.AddRoll(roll);

            var msg = success
                ? string.Format("布卷:{0}。", roll.LabelCode)
                : string.Format("重复:{0}", roll.LabelCode);

            log(string.Format((success ? "" : "!") + "{0} {1} {2}", side, msg, label.ToLocation), LogType.ROLL_QUEUE);
        }

        /// <summary>
        /// 去OPC的可放料信号。
        /// </summary>
        /// <param name="tolocation"></param>
        /// <returns></returns>
        private static bool PanelAvailable(string tolocation) {
            try {
                lock (client) {
                    var s = client.ReadString(param.BAreaPanelState[tolocation]);
                    return s == "2";
                }
            } catch (Exception ex) {
                log($"读交地状态信号异常 tolocation: {tolocation} opc:{JsonConvert.SerializeObject(param.BAreaFloorFinish)} err:{ex}", LogType.ROBOT_STACK);
                return false;//临时
            }
        }

        private static PanelState GetPanelState(LableCode label, PanelInfo pinfo) {
            var state = PanelState.LessHalf;
            if (label.Floor >= pinfo.MaxFloor - 1) {
                state = PanelState.HalfFull;
            }
            if (pinfo.Status == 5 && LableCode.IsPanelLastRoll(pinfo.PanelNo, label.LCode)) {
                state = PanelState.Full;
            }
            return state;
        }

    }
}
