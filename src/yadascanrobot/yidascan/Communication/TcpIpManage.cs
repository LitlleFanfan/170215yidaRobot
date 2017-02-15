using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace ProduceComm {
    public class TcpIpManage : ProduceComm.ICommunication {
        private Socket tc;

        /// <summary>
        /// 读超时时间
        /// </summary>
        private int ReadTimeout = -1;//串口本身的默认值

        /// <summary>
        /// 写超时时间
        /// </summary>
        private int WriteTimeout = -1;//串口本身的默认值

        public TcpIpManage(string ip, int port, int readtimeout = 1000, int writetimeout = 1000) {
            IP = ip;
            Port = port;
            ReadTimeout = readtimeout;
            WriteTimeout = writetimeout;
        }
        string IP;
        int Port;
        /// <summary>
        /// 打开连接
        /// </summary>
        /// <param name="ip">IP</param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool Open() {
            try {
                IPEndPoint iep = new IPEndPoint(IPAddress.Parse(IP), Port);
                tc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                tc.Connect(iep);
                return tc.Connected;
            } catch (Exception ex) {
                clsSetting.loger.Error(string.Format("创建连接 IP: {0}; Port: {1};  {2}", IP, Port, ex));
                return false;
            }
        }

        /// <summary>
        /// 写
        /// </summary>
        /// <param name="sendData">数据</param>
        public void Write(byte[] sendData) {
            tc.SendTimeout = WriteTimeout;
            tc.Send(sendData, sendData.Length, SocketFlags.None);
        }

        /// <summary>
        /// 读
        /// </summary>
        /// <param name="length">长度</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        public byte[] Read(int length, int? timeout = null) {
            byte[] buf = new byte[length];
            tc.ReceiveTimeout = timeout ?? ReadTimeout;
            int n = tc.Receive(buf, 0, length, SocketFlags.Partial);
            byte[] b = buf.Sub(0, n);
            return b;
        }

        public string ReadLine(int? timeout = null) {
            using (var stream = new NetworkStream(tc)) {
                using (var reader = new StreamReader(stream)) {
                    stream.ReadTimeout = timeout ?? ReadTimeout;
                    return reader.ReadLine();
                }
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Close() {
            try {
                tc.Close();
            } catch { }
        }

        public string GetIPOrCom() {
            return IP;
        }

        public int GetPortOrBaudRate() {
            return Port;
        }
    }
}
