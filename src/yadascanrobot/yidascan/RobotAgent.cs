// 这个代码是从RobotControl修改来的。
// 通信代码非常差，但是以后再改吧。
// 2016-12-29 laozhang.gz@139.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

// 暂时用一下RobotControl里面的常数定义.
using RobotControl;

namespace Robot {
    public class RobotAgent {
        Socket sck;

        public string IP { get; }
        public string Port { get; }

        public static string STATE_START = "Start";
        public static string STATE_HOLD = "Hold";
        public static string STATE_ON = "on";
        public static string STATE_OK = "OK";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip">以太网通信IP</param>
        public RobotAgent(string ip, string port= "11000") {
            IP = ip;
            Port = port;
        }

        /// <summary>
        /// 建立连接
        /// </summary>
        public void connect() {
            var ipPort = new IPEndPoint(IPAddress.Parse(IP), int.Parse(Port));
            if (sck == null || !sck.Connected) {
                sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            sck.ReceiveTimeout = 50;
            sck.SendTimeout = 50;

            sck.Connect(ipPort);
            var s = ReadSck();
        }

        public bool IsConnected() {
            return sck != null && sck.Connected;
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Close() {
            Send(string.Format("cmd={0};a1=0;a2=0;a3=0;a4=0;a5=0;", Commands.CMD_STOP));
            sck.Shutdown(SocketShutdown.Both);
            sck.Disconnect(true);
            sck.Close();
        }

        private void Send(string cmd) {
            var array = Encoding.ASCII.GetBytes(cmd);
            sck.Send(array, array.Length, SocketFlags.None);
        }

        private string ReadSck() {
            var bytes = new byte[3024];
            try {
                var bytesRead = sck.Receive(bytes);

                if (bytesRead > 0) {
                    return Encoding.Default.GetString(Sub(bytes, 0, bytesRead));
                }
                return string.Empty;
            } catch (Exception ex) {
                return string.Empty;
            }
        }

        private byte[] Sub(byte[] b1, int index, int length) {
            if (b1.Length < index + length + 1) {
                return null;
            }

            // var re = new byte[length];
            //for (int i = 0; i < length; i++) {
            //    re[i] = b1[i + index];
            //}

            // 这个地方有可能溢出。
            var re = b1.Select(x => (byte)(x + index)).ToArray();
            return re;
        }

        private Dictionary<string, string> CurrPosFormat(string re) {
            var sep = new string[] { "\r\n" };
            var t = re.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            return GetVal(t);
        }

        private Dictionary<string, string> GetVal(string[] arg) {
            var currPos = new Dictionary<string, string>();
            if (arg == null || arg.Length == 0) {
                return currPos;
            }

            for (int i = 0; arg.Length > 0 && i < arg.Length; i++) {
                if (arg[i].Contains("Flip")) {
                    currPos.Add("NoFlip", (arg[i] == "No Flip").ToString());
                } else if (arg[i].Contains("Arm")) {
                    currPos.Add("LowArm", (arg[i] == "Low Arm").ToString());
                } else if (arg[i].Contains("Back") || arg[i].Contains("Front")) {
                    currPos.Add("Back", (arg[i] == "Back").ToString());
                } else if (arg[i].Contains("180 deg")) {
                    if (arg[i].Contains("R")) {
                        currPos.Add("Rgt180", (arg[i] == "R >= 180 deg").ToString());
                    } else if (arg[i].Contains("T")) {
                        currPos.Add("Tgt180", (arg[i] == "T >= 180 deg").ToString());
                    }
                } else if (arg[i].Contains("= ")) {
                    var tmp = arg[i].Split(new string[] { "= " }, StringSplitOptions.RemoveEmptyEntries);
                    currPos.Add(tmp[0], tmp[1]);
                } else if (arg[i].Contains("Unit: (X, Y, Z)")) {
                    var tmp = arg[i].Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);
                    currPos.Add(tmp[0], string.Format("{0};{1}", tmp[1], arg[i + 1].Trim()));
                    i = i + 1;
                } else if (arg[i].Contains("Unit: ")) {
                    var tmp = arg[i].Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);
                    currPos.Add(tmp[0], string.Format("{0};{1};{2}", tmp[1], arg[i + 1].Trim(), arg[i + 2].Trim()));
                    i = i + 2;
                } else if (arg[i].Contains(": ")) {
                    var tmp = arg[i].Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);
                    currPos.Add(tmp[0], tmp[1]);
                }
            }
            return currPos;
        }

        public List<string> GetRobotResult() {
            var re = new List<string>();
            // ???读10ms.
            var d1 = DateTime.Now;
            while (new TimeSpan(DateTime.Now.Ticks - d1.Ticks).Milliseconds < 10) {
                var s = ReadSck();
                if (!string.IsNullOrEmpty(s)) {
                    re.Add(s);
                }
            }
            return re;
        }

        /// <summary>
        /// 获取机器人当前所在坐标
        /// </summary>
        /// <returns></returns>
        //public PostionVar GetCurrPos2() {
        //    var currPos = GetCurrPos();

            //var pv = new PostionVar(decimal.Parse(currPos["(X)"]),
            //    decimal.Parse(currPos["(Y)"]),
            //    decimal.Parse(currPos["(Z)"]),
            //    decimal.Parse(currPos["(Rx)"]),
            //    decimal.Parse(currPos["(Ry)"]),
            //    decimal.Parse(currPos["(Rz)"]), 0, false);
            //return pv;
        // }

        /// <summary>
        /// 获取机器人当前所在坐标
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetCurrPos() {
            Send("cmd=15;a1=0;a2=0;a3=0;a4=0;a5=0;");
            var re = string.Empty;
            do {
                re = ReadSck();
            } while (string.IsNullOrEmpty(re) || re == "OK\r\n");

            return CurrPosFormat(re);
        }

        /// <summary>
        /// 获取机器人当前所在坐标
        /// </summary>
        /// <param name="ptype">位置变量类型</param>
        /// <returns></returns>
        //public PostionVar GetCurrPosEx2(PosVarType ptype) {
        //    Send(string.Format("cmd={0};a1=0;a2={1};a3=0;a4=0;a5=0;", Commands.CMD_MpGetCarPosEx, ptype == PosVarType.Robot ? 1 : 0));
        //    var re = string.Empty;
        //    do {
        //        re = ReadSck();
        //    } while (string.IsNullOrEmpty(re) || re == "OK\r\n");

        //    var currPos = CurrPosFormat(re);

        //    var pv = new PostionVar(decimal.Parse(currPos["(X)"]),
        //        decimal.Parse(currPos["(Y)"]),
        //        decimal.Parse(currPos["(Z)"]),
        //        decimal.Parse(currPos["(Rx)"]),
        //        decimal.Parse(currPos["(Ry)"]),
        //        decimal.Parse(currPos["(Rz)"]), 0, false);
        //    return pv;
        //}
        /// <summary>
        /// 获取机器人当前所在坐标
        /// </summary>
        /// <param name="ptype">位置变量类型</param>
        /// <returns></returns>
        public Dictionary<string, string> GetCurrPosEx(PosVarType ptype) {
            Send(string.Format("cmd={0};a1=0;a2={1};a3=0;a4=0;a5=0;", Commands.CMD_MpGetCarPosEx, ptype == PosVarType.Robot ? 1 : 0));
            var re = string.Empty;
            do {
                re = ReadSck();
            } while (string.IsNullOrEmpty(re) || re == "OK\r\n");

            return CurrPosFormat(re);
        }

        /// <summary>
        /// 写位置变量
        /// </summary>
        /// <param name="pv">位置</param>
        /// <param name="ptype">todo: describe ptype parameter on SetPostion</param>
        /// <param name="pvIndex">todo: describe pvIndex parameter on SetPostion</param>
        /// <param name="pt">todo: describe pt parameter on SetPostion</param>
        /// <param name="toolNo">todo: describe toolNo parameter on SetPostion</param>
        /// <param name="userFrameNo">todo: describe userFrameNo parameter on SetPostion</param>
        /// <returns></returns>
        public bool SetPostion(PosVarType ptype, PostionVar pv, int pvIndex, PosType pt, int toolNo, int userFrameNo) {
            int nConfig;

            nConfig = (int)pt;
            nConfig |= (pv.NoFlip ? 1 : 0) << 8;
            nConfig |= (pv.LowArm ? 1 : 0) << 9;
            nConfig |= (pv.Back ? 1 : 0) << 10;
            nConfig |= (pv.RGt180 ? 1 : 0) << 11;
            nConfig |= (pv.TGt180 ? 1 : 0) << 12;
            nConfig |= toolNo << 16;
            nConfig |= userFrameNo << 22;

            var cmd = string.Format("cmd={0};usType={1};usIndex={2};nConfig={3};ulValue1={4};ulValue2={5};ulValue3={6};ulValue4={7};ulValue5={8};ulValue6={9};ulValue7={10};ulValue8={11};",
                Commands.CMD_MpPutPosVarData, (int)ptype, pvIndex, nConfig,
                pv.sOrX, pv.lOrY, pv.uOrZ, pv.bOrRx, pv.rOrRy, pv.tOrRz, 0, 0);
            Send(cmd);
            var s = ReadSck();
            return s.Contains("OK");
        }

        /// <summary>
        /// 写变量
        /// </summary>
        /// <param name="vt">变量类型</param>
        /// <param name="varIndex">起始变量索引</param>
        /// <param name="qtyToSet">写变量个数</param>
        /// <param name="val">值</param>
        /// <returns></returns>
        public bool SetVariables(VariableType vt, int varIndex, int qtyToSet, string val) {
            var cmd = string.Format("cmd={0};a1={1};a2={2};a3={3};a4={4};a5=0;",
                Commands.CMD_MpPutVarData, (int)vt, varIndex, qtyToSet, val);
            Send(cmd);
            var s = ReadSck();
            ReadSck();
            return s.Contains("OK");
        }

        /// <summary>
        /// 读变量值
        /// </summary>
        /// <param name="vt">变量类型</param>
        /// <param name="varIndex">起始变量索引</param>
        /// <param name="qtyToSet">读变量个数</param>
        /// <returns></returns>
        public Dictionary<string, string> GetVariables(VariableType vt, int varIndex, int qtyToSet) {
            var cmdID = (int)vt > 4 ? Commands.CMD_MpGetPosVarData : Commands.CMD_MpGetVarData;
            var cmd = string.Format("cmd={0};a1={1};a2={2};a3={3};a4=0;a5=0;",
                cmdID, (int)vt, varIndex, qtyToSet);
            Send(cmd);

            var re = GetRobotResult();
            var ret = new Dictionary<string, string>();
            if (re.Count > 0) {
                foreach (string reitem in re) {
                    if (reitem.Contains("OK")) {
                        continue;
                    }
                    var tmp = reitem.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    if (cmdID == Commands.CMD_MpGetVarData) {
                        foreach (string str in tmp) {
                            var stmp = str.Split(new string[] { "= " }, StringSplitOptions.RemoveEmptyEntries);
                            if (ret.ContainsKey(stmp[0])) {
                                ret[stmp[0]] = stmp[1];
                            } else {
                                ret.Add(stmp[0], stmp[1]);
                            }
                        }
                    } else {
                        ret = GetVal(tmp);
                    }
                }
            }
            return ret;
        }

        [Obsolete("not test yet.")]
        public Dictionary<string, string> GetVariablesPro(VariableType vt, int varIndex, int qtyToSet) {
            var cmdID = (int)vt > 4 ? Commands.CMD_MpGetPosVarData : Commands.CMD_MpGetVarData;
            var cmd = string.Format("cmd={0};a1={1};a2={2};a3={3};a4=0;a5=0;",
                cmdID, (int)vt, varIndex, qtyToSet);
            Send(cmd);

            var re = GetRobotResult();
            var ret = new Dictionary<string, string>();

            if (re.Count > 0) {
                foreach (string reitem in re) {
                    if (reitem.Contains("OK")) { continue; }
                    var tmp = reitem.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    if (cmdID == Commands.CMD_MpGetVarData) {
                        foreach (string str in tmp) {
                            var stmp = str.Split(new string[] { "= " }, StringSplitOptions.RemoveEmptyEntries);
                            if (ret.ContainsKey(stmp[0])) {
                                ret[stmp[0]] = stmp[1];
                            } else {
                                ret.Add(stmp[0], stmp[1]);
                            }
                        }
                    } else {
                        ret = GetVal(tmp);
                    }
                }
            } // end of if.
            return ret;
        }


        /// <summary>
        /// 获得机器人运行状态
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, bool> GetPlayStatus() {
            Send(string.Format("cmd={0};a1=0;a2=0;a3=0;a4=0;a5=0;", Commands.CMD_MpGetPlayStatus));
            var re = GetRobotResult();
            var ret = new Dictionary<string, bool>();
            if (re.Count > 0) {
                foreach (string reitem in re) {
                    var tmp = reitem.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string str in tmp) {
                        if (str.Contains(STATE_START) || str.Contains(STATE_HOLD)) {
                            var stmp = str.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                            if (ret.ContainsKey(stmp[0])) {
                                ret[stmp[0]] = stmp[1].ToLower() == "true" || stmp[1].ToLower() == "on";
                            } else {
                                ret.Add(stmp[0], stmp[1].ToLower() == "true" || stmp[1].ToLower() == "on");
                            }
                        }
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// 获得机器人报警状态
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, bool> GetAlarmStatus() {
            Send(string.Format("cmd={0};a1=0;a2=0;a3=0;a4=0;a5=0;", Commands.CMD_MpGetAlarmStatus));
            var re = GetRobotResult();
            var ret = new Dictionary<string, bool>();
            if (re.Count > 0) {
                foreach (string reitem in re) {
                    var tmp = reitem.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string str in tmp) {
                        var stmp = str.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        if (ret.ContainsKey(stmp[0])) {
                            ret[stmp[0]] = stmp[0].ToLower() != "on";
                        } else {
                            ret.Add(stmp[1], stmp[0].ToLower() != "on");
                        }
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// 获得机器人报警信息
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetAlarmCode() {
            Send(string.Format("cmd={0};a1=0;a2=0;a3=0;a4=0;a5=0;", Commands.CMD_MpGetAlarmCode));
            var re = GetRobotResult();
            var ret = new Dictionary<string, string>();
            if (re.Count > 0) {
                foreach (string reitem in re) {
                    var tmp = reitem.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string str in tmp) {
                        var stmp = str.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                        if (ret.ContainsKey(stmp[0])) {
                            ret[stmp[0]] = stmp[1];
                        } else {
                            ret.Add(stmp[0], stmp[1]);
                        }
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// 启动程序
        /// </summary>
        /// <param name="jobName">程序名称</param>
        /// <returns></returns>
        public bool StartJob(string jobName) {
            Send($"cmd={Commands.CMD_MpStartJob};a1=0;a2={jobName};");
            return ReadSck().Contains("OK");
        }

        /// <summary>
        /// 伺服准备
        /// </summary>
        /// <param name="on">开：true；关：false；</param>
        /// <returns></returns>
        public bool ServoPower(bool on) {
            // Send(string.Format("cmd={0};a1={1};a2=0;a3=0;a4=0;a5=0;", Commands.CMD_MpSetServoPower, on ? 1 : 0));
            var ison = on ? 1 : 0;
            Send($"cmd={Commands.CMD_MpSetServoPower};a1={ison};a2=0;a3=0;a4=0;a5=0;");
            return ReadSck().Contains("OK");
        }

        /// <summary>
        /// 清除报警
        /// </summary>
        public void ClearAlarm() {
            // Send(string.Format("cmd={0};a1=0;a2=0;a3=0;a4=0;a5=0;", Commands.CMD_MpResetAlarm));
            Send($"cmd={Commands.CMD_MpResetAlarm};a1=0;a2=0;a3=0;a4=0;a5=0;");
        }

        /// <summary>
        /// 清除错误
        /// </summary>
        public void ClearError() {
            // Send(string.Format("cmd={0};a1=0;a2=0;a3=0;a4=0;a5=0;", Commands.CMD_MpCancelError));
            Send($"cmd={Commands.CMD_MpCancelError};a1=0;a2=0;a3=0;a4=0;a5=0;");
        }

        /// <summary>
        /// 暂停
        /// </summary>
        /// <param name="on">开：true；关：false；</param>
        public void Hold(bool on) {
            // Send(string.Format("cmd={0};a1={1};a2=0;a3=0;a4=0;a5=0;", Commands.CMD_MpHold, on ? 0 : 1));
            var ison = on ? 0 : 1;
            Send($"cmd={Commands.CMD_MpHold};a1={ison};a2=0;a3=0;a4=0;a5=0;");
        }

        /// <summary>
        /// 发送机器人命令
        /// </summary>
        /// <param name="cmd">命令</param>
        /// <returns></returns>
        public bool SendCmd(string cmd) {
            Send(cmd);
            return ReadSck().Contains("OK");
        }
    }
}
