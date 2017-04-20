using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Data;
using ProduceComm;
using ProduceComm.OPC;
using yidascan.DataAccess;

namespace yidascan {
    class SelfTest {
        private IOpcClient client;
        private OPCParam param;

        public Action<string> log;
        public CommunicationCfg cameracfg;

        public SelfTest(CommunicationCfg cfg, Action<string> loghandler) {
            cameracfg = cfg;
            log = loghandler;
        }

        public void initopc() {
            // init opc client.
            client = new OPCClient();

            var dtopc = OPCParam.Query();
            dtopc.Columns.Remove("Class");
            dtopc.Columns.Add(new DataColumn("Value"));

            // init camera config.
            client.AddSubscription(dtopc);

            param = new OPCParam();
        }

        public void close() {
            if (client != null) {
                client.Close();
            }
        }

        private bool testRobot() {
            var ip = clsSetting.RobotIP;
            // var port = 11000;
            RobotControl.RobotControl robot;
            try {
                robot = new RobotControl.RobotControl(ip);

                robot.Connect();
                var status = robot.GetPlayStatus();

                log?.Invoke("机器人状态: ");
                foreach (var k in status) {
                    log?.Invoke($"{k.Key}: {k.Value}");
                }
                log?.Invoke("------------------");

                robot.Close();
                return true;
            } catch (Exception ex) {
                log?.Invoke($"!机器人访问异常: {ex}");
                return false;
            } 
        }

        public void run() {
            // test scan signal.
            var r = testScanner();
            var target = "相机";
            if (!r) { log?.Invoke($"!{target}自检失败"); }

            r = testErp();
            target = "Erp通信";
            if (!r) { log?.Invoke($"!{target}自检失败"); }

            r = testSignals();
            target = "opc信号读写";
            if (!r) { log?.Invoke($"!{target}自检失败"); }


            r = testRobot();
            target = "机器人通信";
            if (!r) { log?.Invoke($"!{target}自检失败"); }
        }

        private void testTcpConnect(string ip, int port) {
            using (var sock = new TcpClient(ip, port)) {
                log?.Invoke($"tcp连接状态: {sock.Connected}");
                sock.Close();
            }
        }

        public bool testScanner() {
            var ip = cameracfg.IPAddr;
            var port = cameracfg.IPPort;
            try {
                testTcpConnect(ip, Int32.Parse(port));
                return true;
            } catch (Exception ex) {
                log?.Invoke($"!tcp连接异常： {ex.ToString()}");
                return false;
            }
        }

        public bool testErp() {
            log?.Invoke("!no erp test.");
            return true;
        }

        public bool testSignals() {
            try {
                initopc();
                log?.Invoke("no test.");
                return true;
            } catch (Exception ex) {
                log?.Invoke($"!opc访问失败: {ex.ToString()}");
                return false;
            }
        }
    }
}
