[1mdiff --git a/src/yadascanrobot/RobotControl/RobotControl.cs b/src/yadascanrobot/RobotControl/RobotControl.cs[m
[1mindex fce53b9..5da27ae 100644[m
[1m--- a/src/yadascanrobot/RobotControl/RobotControl.cs[m
[1m+++ b/src/yadascanrobot/RobotControl/RobotControl.cs[m
[36m@@ -30,33 +30,23 @@[m [mnamespace RobotControl {[m
         /// [m
         /// </summary>[m
         /// <param name="ip">以太网通信IP</param>[m
[31m-        public RobotControl(string ip) {[m
[32m+[m[32m        public RobotControl(string ip, string port) {[m
             iPAddress = ip;[m
[31m-            portNo = "11000";[m
[31m-        }[m
[31m-[m
[31m-        /// <summary>[m
[31m-        /// [m
[31m-        /// </summary>[m
[31m-        /// <param name="ip">以太网通信IP</param>[m
[31m-        /// <param name="prot">以太网通信端口号</param>[m
[31m-        public RobotControl(string ip, string prot) {[m
[31m-            iPAddress = ip;[m
[31m-            portNo = prot;[m
[32m+[m[32m            portNo = port;[m
         }[m
 [m
         /// <summary>[m
         /// 建立连接[m
         /// </summary>[m
         public void Connect() {[m
[31m-            IPEndPoint ipPort = new IPEndPoint(IPAddress.Parse(iPAddress), int.Parse(portNo));[m
[32m+[m[32m            var ipPort = new IPEndPoint(IPAddress.Parse(iPAddress), int.Parse(portNo));[m
             if (sck == null || !sck.Connected) {[m
                 sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);[m
             }[m
             sck.ReceiveTimeout = 50;[m
             sck.SendTimeout = 50;[m
             sck.Connect(ipPort);[m
[31m-            string s = ReadSck();[m
[32m+[m[32m            var s = ReadSck();[m
             connected = true;// s == "Connected to TCP server...\r\n";[m
         }[m
 [m
[36m@@ -75,18 +65,41 @@[m [mnamespace RobotControl {[m
             sck.Close();[m
         }[m
 [m
[32m+[m[32m        public bool TryReconnect() {[m
[32m+[m[32m            var times = 10;[m
[32m+[m[32m            while (times-- > 0) {[m
[32m+[m[32m                if (!Reconnect()) {[m
[32m+[m[32m                    Thread.Sleep(200);[m
[32m+[m[32m                } else {[m
[32m+[m[32m                    return true;[m
[32m+[m[32m                }[m
[32m+[m[32m            }[m
[32m+[m[32m            return false;[m
[32m+[m[32m        }[m
[32m+[m
[32m+[m[32m        private bool Reconnect() {[m
[32m+[m[32m            try {[m
[32m+[m[32m                Close();[m
[32m+[m[32m                Thread.Sleep(200);[m
[32m+[m[32m                Connect();[m
[32m+[m[32m                return true;[m
[32m+[m[32m            } catch (SocketException ex) {[m
[32m+[m[32m                return false;[m
[32m+[m[32m            }[m
[32m+[m[32m        }[m
[32m+[m
         private void Send(string cmd) {[m
[31m-            byte[] array = System.Text.Encoding.ASCII.GetBytes(cmd);[m
[32m+[m[32m            var array = Encoding.ASCII.GetBytes(cmd);[m
             sck.Send(array, array.Length, SocketFlags.None);[m
         }[m
 [m
         private string ReadSck() {[m
[31m-            byte[] bytes = new byte[3024];[m
[32m+[m[32m            var bytes = new byte[3024];[m
             try {[m
[31m-                int bytesRead = sck.Receive(bytes);[m
[32m+[m[32m                var bytesRead = sck.Receive(bytes);[m
 [m
                 if (bytesRead > 0) {[m
[31m-                    return System.Text.Encoding.Default.GetString(Sub(bytes, 0, bytesRead));[m
[32m+[m[32m                    return Encoding.Default.GetString(Sub(bytes, 0, bytesRead));[m
                 }[m
                 return string.Empty;[m
             } catch (Exception ex) {[m
[36m@@ -94,10 +107,24 @@[m [mnamespace RobotControl {[m
             }[m
         }[m
 [m
[32m+[m[32m        private string ReadAll() {[m
[32m+[m[32m            using (var stream = new NetworkStream(sck))[m
[32m+[m[32m            using (var reader = new System.IO.StreamReader(stream)) {[m
[32m+[m[32m                return reader.ReadToEnd();[m
[32m+[m[32m            }[m
[32m+[m[32m        }[m
[32m+[m
[32m+[m[32m        private string ReadLine() {[m
[32m+[m[32m            using (var stream = new NetworkStream(sck))[m
[32m+[m[32m            using (var reader = new System.IO.StreamReader(stream)) {[m
[32m+[m[32m                return reader.ReadLine();[m
[32m+[m[32m            }[m
[32m+[m[32m        }[m
[32m+[m
         private byte[] Sub(byte[] b1, int index, int length) {[m
             if (b1.Length < index + length + 1)[m
                 return null;[m
[31m-            byte[] re = new byte[length];[m
[32m+[m[32m            var re = new byte[length];[m
             for (int i = 0; i < length; i++) {[m
                 re[i] = b1[i + index];[m
             }[m
[36m@@ -105,13 +132,13 @@[m [mnamespace RobotControl {[m
         }[m
 [m
         private Dictionary<string, string> CurrPosFormat(string re) {[m
[31m-            string[] t = re.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);[m
[32m+[m[32m            var t = re.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);[m
 [m
             return GetVal(t);[m
         }[m
 [m
         private Dictionary<string, string> GetVal(string[] arg) {[m
[31m-            Dictionary<string, string> currPos = new Dictionary<string, string>();[m
[32m+[m[32m            var currPos = new Dictionary<string, string>();[m
             if (arg == null || arg.Length == 0)[m
                 return currPos;[m
 [m
[36m@@ -148,10 +175,10 @@[m [mnamespace RobotControl {[m
         }[m
 [m
         public List<string> GetRobotResult() {[m
[31m-            List<string> re = new List<string>();[m
[31m-            DateTime d1 = DateTime.Now;[m
[32m+[m[32m            var re = new List<string>();[m
[32m+[m[32m            var d1 = DateTime.Now;[m
             while (connected && new TimeSpan(DateTime.Now.Ticks - d1.Ticks).Milliseconds < 10) {[m
[31m-                string s = ReadSck();[m
[32m+[m[32m                var s = ReadSck();[m
                 if (!string.IsNullOrEmpty(s)) {[m
                     re.Add(s);[m
                 }[m
[36m@@ -163,25 +190,9 @@[m [mnamespace RobotControl {[m
         /// 获取机器人当前所在坐标[m
         /// </summary>[m
         /// <returns></returns>[m
[31m-        public PostionVar GetCurrPos2() {[m
[31m-            Dictionary<string, string> currPos = GetCurrPos();[m
[31m-[m
[31m-            PostionVar pv = new PostionVar(decimal.Parse(currPos["(X)"]),[m
[31m-                decimal.Parse(currPos["(Y)"]),[m
[31m-                decimal.Parse(currPos["(Z)"]),[m
[31m-                decimal.Parse(currPos["(Rx)"]),[m
[31m-                decimal.Parse(currPos["(Ry)"]),[m
[31m-                decimal.Parse(currPos["(Rz)"]), 0, false);[m
[31m-            return pv;[m
[31m-        }[m
[31m-[m
[31m-        /// <summary>[m
[31m-        /// 获取机器人当前所在坐标[m
[31m-        /// </summary>[m
[31m-        /// <returns></returns>[m
         public Dictionary<string, string> GetCurrPos() {[m
             Send("cmd=15;a1=0;a2=0;a3=0;a4=0;a5=0;");[m
[31m-            string re = string.Empty;[m
[32m+[m[32m            var re = string.Empty;[m
             do {[m
                 re = ReadSck();[m
             } while (string.IsNullOrEmpty(re) || re == "OK\r\n");[m
[36m@@ -189,20 +200,16 @@[m [mnamespace RobotControl {[m
             return CurrPosFormat(re);[m
         }[m
 [m
[31m-        /// <summary>[m
[31m-        /// 获取机器人当前所在坐标[m
[31m-        /// </summary>[m
[31m-        /// <returns></returns>[m
         public PostionVar GetCurrPosEx2(PosVarType ptype) {[m
             Send(string.Format("cmd={0};a1=0;a2={1};a3=0;a4=0;a5=0;", Commands.CMD_MpGetCarPosEx, ptype == PosVarType.Robot ? 1 : 0));[m
[31m-            string re = string.Empty;[m
[32m+[m[32m            var re = string.Empty;[m
             do {[m
                 re = ReadSck();[m
             } while (string.IsNullOrEmpty(re) || re == "OK\r\n");[m
 [m
[31m-            Dictionary<string, string> currPos = CurrPosFormat(re);[m
[32m+[m[32m            var currPos = CurrPosFormat(re);[m
 [m
[31m-            PostionVar pv = new PostionVar(decimal.Parse(currPos["(X)"]),[m
[32m+[m[32m            var pv = new PostionVar(decimal.Parse(currPos["(X)"]),[m
                 decimal.Parse(currPos["(Y)"]),[m
                 decimal.Parse(currPos["(Z)"]),[m
                 decimal.Parse(currPos["(Rx)"]),[m
[36m@@ -210,13 +217,10 @@[m [mnamespace RobotControl {[m
                 decimal.Parse(currPos["(Rz)"]), 0, false);[m
             return pv;[m
         }[m
[31m-        /// <summary>[m
[31m-        /// 获取机器人当前所在坐标[m
[31m-        /// </summary>[m
[31m-        /// <returns></returns>[m
[32m+[m
         public Dictionary<string, string> GetCurrPosEx(PosVarType ptype) {[m
             Send(string.Format("cmd={0};a1=0;a2={1};a3=0;a4=0;a5=0;", Commands.CMD_MpGetCarPosEx, ptype == PosVarType.Robot ? 1 : 0));[m
[31m-            string re = string.Empty;[m
[32m+[m[32m            var re = string.Empty;[m
             do {[m
                 re = ReadSck();[m
             } while (string.IsNullOrEmpty(re) || re == "OK\r\n");[m
[36m@@ -224,11 +228,6 @@[m [mnamespace RobotControl {[m
             return CurrPosFormat(re);[m
         }[m
 [m
[31m-        /// <summary>[m
[31m-        /// 写位置变量[m
[31m-        /// </summary>[m
[31m-        /// <param name="pv">位置</param>[m
[31m-        /// <returns></returns>[m
         public bool SetPostion(PosVarType ptype, PostionVar pv, int pvIndex, PosType pt, int toolNo, int userFrameNo) {[m
             int nConfig;[m
 [m
[36m@@ -245,7 +244,7 @@[m [mnamespace RobotControl {[m
                 Commands.CMD_MpPutPosVarData, (int)ptype, pvIndex, nConfig,[m
                 pv.sOrX, pv.lOrY, pv.uOrZ, pv.bOrRx, pv.rOrRy, pv.tOrRz, 0, 0);[m
             Send(cmd);[m
[31m-            string s = ReadSck();[m
[32m+[m[32m            var s = ReadSck();[m
             return s.Contains("OK");[m
         }[m
 [m
[36m@@ -258,10 +257,10 @@[m [mnamespace RobotControl {[m
         /// <param name="val">值</param>[m
         /// <returns></returns>[m
         public bool SetVariables(VariableType vt, int varIndex, int qtyToSet, string val) {[m
[31m-            string cmd = string.Format("cmd={0};a1={1};a2={2};a3={3};a4={4};a5=0;",[m
[32m+[m[32m            var cmd = string.Format("cmd={0};a1={1};a2={2};a3={3};a4={4};a5=0;",[m
                 Commands.CMD_MpPutVarData, (int)vt, varIndex, qtyToSet, val);[m
             Send(cmd);[m
[31m-            string s = ReadSck();[m
[32m+[m[32m            var s = ReadSck();[m
             ReadSck();[m
             return s.Contains("OK");[m
         }[m
[36m@@ -279,29 +278,30 @@[m [mnamespace RobotControl {[m
 [m
             Send(cmd);[m
 [m
[31m-            var re = GetRobotResult();[m
[32m+[m[32m            var re = ReadAll().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);[m
             var ret = new Dictionary<string, string>();[m
 [m
[31m-            foreach (string reitem in re) {[m
[31m-                if (reitem.Contains("OK")) {[m
[32m+[m[32m            foreach (string item in re) {[m
[32m+[m[32m                if (item.Contains("OK")) {[m
                     continue;[m
                 }[m
 [m
[31m-                var tmp = reitem.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);[m
[31m-                if (cmdID == Commands.CMD_MpGetVarData) {[m
[31m-                    foreach (var str in tmp) {[m
[31m-                        var stmp = str.Split(new string[] { "= " }, StringSplitOptions.RemoveEmptyEntries);[m
[32m+[m[32m                try {[m
[32m+[m[32m                    if (cmdID == Commands.CMD_MpGetVarData) {[m
[32m+[m[32m                        var foo = item.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);[m
 [m
[31m-                        if (stmp.Count() < 2) { continue; }[m
[32m+[m[32m                        if (foo.Count() < 2) { continue; }[m
 [m
[31m-                        if (ret.ContainsKey(stmp[0])) {[m
[31m-                            ret[stmp[0]] = stmp[1];[m
[32m+[m[32m                        if (ret.ContainsKey(foo[0])) {[m
[32m+[m[32m                            ret[foo[0]] = foo[1];[m
                         } else {[m
[31m-                            ret.Add(stmp[0], stmp[1]);[m
[32m+[m[32m                            ret.Add(foo[0], foo[1]);[m
                         }[m
[32m+[m[32m                    } else {[m
[32m+[m[32m                        ret = GetVal(tmp);[m
                     }[m
[31m-                } else {[m
[31m-                    ret = GetVal(tmp);[m
[32m+[m[32m                } catch {[m
[32m+[m[32m                    return new Dictionary<string, string>();[m
                 }[m
             }[m
 [m
[1mdiff --git a/src/yadascanrobot/yidascan/SelfTest.cs b/src/yadascanrobot/yidascan/SelfTest.cs[m
[1mindex 8d70317..fc88410 100644[m
[1m--- a/src/yadascanrobot/yidascan/SelfTest.cs[m
[1m+++ b/src/yadascanrobot/yidascan/SelfTest.cs[m
[36m@@ -47,7 +47,7 @@[m [mnamespace yidascan {[m
             // var port = 11000;[m
             RobotControl.RobotControl robot;[m
             try {[m
[31m-                robot = new RobotControl.RobotControl(ip);[m
[32m+[m[32m                robot = new RobotControl.RobotControl(ip, "11000");[m
 [m
                 robot.Connect();[m
                 var status = robot.GetPlayStatus();[m
[1mdiff --git a/src/yadascanrobot/yidascan/scanner/RobotHelper.cs b/src/yadascanrobot/yidascan/scanner/RobotHelper.cs[m
[1mindex 6774012..d9cf809 100644[m
[1m--- a/src/yadascanrobot/yidascan/scanner/RobotHelper.cs[m
[1m+++ b/src/yadascanrobot/yidascan/scanner/RobotHelper.cs[m
[36m@@ -206,7 +206,7 @@[m [mnamespace yidascan {[m
         public RobotHelper(IErpApi _erpapi, string ip, string jobName) {[m
             try {[m
                 erpapi = _erpapi;[m
[31m-                rCtrl = new RobotControl.RobotControl(ip);[m
[32m+[m[32m                rCtrl = new RobotControl.RobotControl(ip, "11000");[m
                 rCtrl.Connect();[m
                 rCtrl.ServoPower(true);[m
                 JOB_NAME = jobName;[m
[36m@@ -297,19 +297,15 @@[m [mnamespace yidascan {[m
             }, times, 80);[m
         }[m
 [m
[31m-        public bool IsBusy() {[m
[32m+[m[32m        private bool IsBusy() {[m
             // 读机器人的状态可能有错。[m
[31m-            try {[m
[31m-                var status = rCtrl.GetPlayStatus();[m
[31m-[m
[31m-                if (status == null || status.Count == 0) {[m
[31m-                    return true;[m
[31m-                } else {[m
[31m-                    return (status["Start"] || status["Hold"]);[m
[31m-                }[m
[31m-            } catch (Exception ex) {[m
[31m-                log($"来源: {nameof(IsBusy)}, {ex}", LogType.ROBOT_STACK, LogViewType.OnlyFile);[m
[32m+[m[32m            var status = rCtrl.GetPlayStatus();[m
[32m+[m[32m            const string start = "Start";[m
[32m+[m[32m            const string hold = "Hold";[m
[32m+[m[32m            if (status == null || !status.ContainsKey(start) || !status.ContainsKey(hold)) {[m
                 return true;[m
[32m+[m[32m            } else {[m
[32m+[m[32m                return (status[start] || status[hold]);[m
             }[m
         }[m
 [m
[36m@@ -317,20 +313,18 @@[m [mnamespace yidascan {[m
             var times = 5;[m
             while (times-- > 0) {[m
                 try {[m
[31m-                    var status = rCtrl.GetPlayStatus();[m
[31m-[m
[31m-                    if (status == null || status.Count == 0) {[m
[31m-                        c