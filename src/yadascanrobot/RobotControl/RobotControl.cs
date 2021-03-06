﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;

namespace RobotControl {
    public class RobotControl {
        public static readonly string START = "Start";
        public static readonly string HOLD = "Hold";

        Socket sck;
        // private StreamReader reader;

        bool connected = false;

        public bool Connected {
            get { return connected; }
        }

        string iPAddress;

        public string IP {
            get { return iPAddress; }
        }
        string portNo;

        public string PortNo {
            get { return portNo; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip">以太网通信IP</param>
        public RobotControl(string ip, string port) {
            iPAddress = ip;
            portNo = port;
        }

        /// <summary>
        /// 建立连接
        /// </summary>
        public void Connect() {
            var ipPort = new IPEndPoint(IPAddress.Parse(iPAddress), int.Parse(portNo));
            if (sck == null || !sck.Connected) {
                sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            sck.ReceiveTimeout = 100;
            sck.SendTimeout = 100;
            sck.Connect(ipPort);
            var s = ReadSck();

            connected = true;// s == "Connected to TCP server...\r\n";
            // var stream = new NetworkStream(sck);
            // reader = new StreamReader(stream);
        }

        public bool IsConnected() {
            return sck != null && sck.Connected;
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Close() {
            try {
                connected = false;
                Send(string.Format("cmd={0};a1=0;a2=0;a3=0;a4=0;a5=0;", Commands.CMD_STOP));
                sck.Shutdown(SocketShutdown.Both);
                // reader.Close();
                sck.Close();
            } catch { }
        }

        public bool TryReconnect() {
            var times = 10;
            while (times-- > 0) {
                if (!Reconnect()) {
                    Thread.Sleep(200);
                } else {
                    return true;
                }
            }
            return false;
        }

        private bool Reconnect() {
            try {
                Close();
                Thread.Sleep(200);
                Connect();
                return true;
            } catch (SocketException ex) {
                return false;
            }
        }

        private void Send(string cmd) {
            var array = Encoding.ASCII.GetBytes(cmd);
            sck.Send(array, array.Length, SocketFlags.None);
        }

        private string ReadSck() {
            const int MAX = 3024;
            var bytes = new byte[MAX];
            try {
                var bytesRead = sck.Receive(bytes);

                if (bytesRead > 0) {
                    return Encoding.Default.GetString(Slice(bytes, 0, bytesRead));
                }
                return string.Empty;
            } catch (Exception ex) {
                return string.Empty;
            }
        }

        private string ReadAll() {
            return ReadSck();
            //if (!reader.EndOfStream) {
            //    return reader.ReadToEnd();
            //} else { return string.Empty; }
        }

        //private string ReadLine() {
        //    return reader.ReadLine();
        //}

        private static byte[] Slice(byte[] b1, int index, int length) {
            if (b1.Length < index + length + 1)
                return null;
            var re = new byte[length];
            for (int i = 0; i < length; i++) {
                re[i] = b1[i + index];
            }
            return re;
        }

        private static Dictionary<string, string> CurrPosFormat(string re) {
            var t = re.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            return GetVal(t);
        }

        private static Dictionary<string, string> GetVal(string[] arg) {
            var currPos = new Dictionary<string, string>();
            if (arg == null || arg.Length == 0)
                return currPos;

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
            var d1 = DateTime.Now;
            while (connected && new TimeSpan(DateTime.Now.Ticks - d1.Ticks).Milliseconds < 10) {
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
        public Dictionary<string, string> GetCurrPos() {
            Send("cmd=15;a1=0;a2=0;a3=0;a4=0;a5=0;");
            var re = string.Empty;
            do {
                re = ReadSck();
            } while (string.IsNullOrEmpty(re) || re == "OK\r\n");

            return CurrPosFormat(re);
        }

        public PostionVar GetCurrPosEx2(PosVarType ptype) {
            Send(string.Format("cmd={0};a1=0;a2={1};a3=0;a4=0;a5=0;", Commands.CMD_MpGetCarPosEx, ptype == PosVarType.Robot ? 1 : 0));
            var re = string.Empty;
            do {
                re = ReadSck();
            } while (string.IsNullOrEmpty(re) || re == "OK\r\n");

            var currPos = CurrPosFormat(re);

            var pv = new PostionVar(decimal.Parse(currPos["(X)"]),
                decimal.Parse(currPos["(Y)"]),
                decimal.Parse(currPos["(Z)"]),
                decimal.Parse(currPos["(Rx)"]),
                decimal.Parse(currPos["(Ry)"]),
                decimal.Parse(currPos["(Rz)"]), 0, false);
            return pv;
        }

        public Dictionary<string, string> GetCurrPosEx(PosVarType ptype) {
            Send(string.Format("cmd={0};a1=0;a2={1};a3=0;a4=0;a5=0;", Commands.CMD_MpGetCarPosEx, ptype == PosVarType.Robot ? 1 : 0));
            var re = string.Empty;
            do {
                re = ReadSck();
            } while (string.IsNullOrEmpty(re) || re == "OK\r\n");

            return CurrPosFormat(re);
        }

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
        /// <param name="log">日志函数</param>
        /// <returns></returns>
        public Dictionary<string, string> GetVariablesPro(VariableType vt, int varIndex, int qtyToSet, Action<string> log) {
            // 未测试通过。2017.07.29
            var cmdID = (int)vt > 4 ? Commands.CMD_MpGetPosVarData : Commands.CMD_MpGetVarData;
            var cmd = $"cmd={cmdID};a1={(int)vt};a2={varIndex};a3={qtyToSet};a4=0;a5=0;";

            Send(cmd);

            var lines = ReadAll().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            
            var ret = new Dictionary<string, string>();

            foreach (string item in lines) {
                if (item.Contains("OK")) { continue; }

                try {
                    if (cmdID == Commands.CMD_MpGetVarData) {
                        var foo = item.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);

                        if (foo.Count() < 2) { continue; }

                        if (ret.ContainsKey(foo[0])) {
                            ret[foo[0]] = foo[1];
                        } else {
                            ret.Add(foo[0], foo[1]);
                        }
                    } else {
                        ret = GetVal(lines);
                    }
                } catch {
                    return new Dictionary<string, string>();
                }
            }

            return ret;
        }

        public Dictionary<string, string> GetVariables(VariableType vt, int varIndex, int qtyToSet, Action<string> log) {
            // 未测试通过。2017.07.29
            var cmdID = (int)vt > 4 ? Commands.CMD_MpGetPosVarData : Commands.CMD_MpGetVarData;
            var cmd = $"cmd={cmdID};a1={(int)vt};a2={varIndex};a3={qtyToSet};a4=0;a5=0;";

            Send(cmd);

            var lines = GetRobotResult();

            var ret = new Dictionary<string, string>();

            foreach (string item in lines) {
                if (item.Contains("OK")) { continue; }

                try {
                    if (cmdID == Commands.CMD_MpGetVarData) {
                        var foo = item.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);

                        foo = foo.Select(x => x.Trim()).ToArray();

                        if (foo.Count() < 2) { continue; }

                        if (ret.ContainsKey(foo[0])) {
                            ret[foo[0]] = foo[1];
                        } else {
                            ret.Add(foo[0], foo[1]);
                        }
                    } else {
                        ret = GetVal(lines.ToArray());
                    }
                } catch {
                    return new Dictionary<string, string>();
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
            // Send(string.Format("cmd={0};a1=0;a2=0;a3=0;a4=0;a5=0;", Commands.CMD_MpGetPlayStatus));
            Send($"cmd={Commands.CMD_MpGetPlayStatus};a1=0;a2=0;a3=0;a4=0;a5=0;");
            var re = GetRobotResult();
            var ret = new Dictionary<string, bool>();
            if (re.Count > 0) {
                foreach (string reitem in re) {
                    var tmp = reitem.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string str in tmp) {
                        if (str.Contains(START) || str.Contains(HOLD)) {
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
            Send(string.Format("cmd={0};a1=0;a2={1};", Commands.CMD_MpStartJob, jobName));
            return ReadSck().Contains("OK");
        }

        /// <summary>
        /// 伺服准备
        /// </summary>
        /// <param name="on">开：true；关：false；</param>
        /// <returns></returns>
        public bool ServoPower(bool on) {
            Send(string.Format("cmd={0};a1={1};a2=0;a3=0;a4=0;a5=0;", Commands.CMD_MpSetServoPower, on ? 1 : 0));
            return ReadSck().Contains("OK");
        }

        /// <summary>
        /// 清除报警
        /// </summary>
        public void ClearAlarm() {
            Send(string.Format("cmd={0};a1=0;a2=0;a3=0;a4=0;a5=0;", Commands.CMD_MpResetAlarm));
        }

        /// <summary>
        /// 清除错误
        /// </summary>
        public void ClearError() {
            Send(string.Format("cmd={0};a1=0;a2=0;a3=0;a4=0;a5=0;", Commands.CMD_MpCancelError));
        }

        /// <summary>
        /// 暂停
        /// </summary>
        /// <param name="on">开：true；关：false；</param>
        public void Holding(bool on) {
            Send(string.Format("cmd={0};a1={1};a2=0;a3=0;a4=0;a5=0;", Commands.CMD_MpHold, on ? 0 : 1));
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

        public bool IsBusy() {
            var status = GetPlayStatus();
            if (status == null || !status.ContainsKey(START) || !status.ContainsKey(HOLD)) {
                return true;
            } else {
                return (status[START] || status[HOLD]);
            }
        }
    }
}
