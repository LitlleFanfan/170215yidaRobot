﻿using System;
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

        public static void StopJobTask() {
            isrun = false;
            Thread.Sleep(1000);
        }

        private static void pushInQueue(string fullcode, string side) {
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
