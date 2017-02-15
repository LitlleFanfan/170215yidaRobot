using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProduceComm
{
    /// <summary>
    /// 串口管理
    /// </summary>
    public class SerialPortManage : ProduceComm.ICommunication
    {
        /// <summary>
        /// 串口
        /// </summary>
        private SerialPort serialPort = new SerialPort();

        /// <summary>
        /// 读超时时间
        /// </summary>
        private int ReadTimeout = -1;//串口本身的默认值

        /// <summary>
        /// 写超时时间
        /// </summary>
        private int WriteTimeout = -1;//串口本身的默认值

        /// <summary>
        /// 构造方法
        /// </summary>
        public SerialPortManage(string portName, int baudRate)
        {
                 PortName=portName;
                 BaudRate=baudRate;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="readtimeout">读超时时间</param>
        /// <param name="writetimeout">写超时时间</param>
        public SerialPortManage(int readtimeout, int writetimeout)
        {
            ReadTimeout = readtimeout;
            WriteTimeout = writetimeout;
        }
        string PortName;
        int BaudRate;
        /// <summary>
        /// 打开
        /// </summary>
        /// <param name="portName">端口</param>
        /// <param name="baudRate">波特率</param>
        /// <param name="dataBits">数据位</param>
        /// <param name="stopBits">停止位</param>
        /// <param name="parity">奇偶校验位</param>
        /// <returns></returns>
        public bool Open()
        {
            try
            {
                this.serialPort.DataBits = 8;
                this.serialPort.StopBits = StopBits.One;
                this.serialPort.BaudRate = BaudRate;
                this.serialPort.Parity = Parity.None;
                this.serialPort.PortName = PortName;
                this.serialPort.DtrEnable = true;
                this.serialPort.RtsEnable = true;
                this.serialPort.Open();
                return this.serialPort.IsOpen;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 打开
        /// </summary>
        /// <param name="portName">端口</param>
        /// <param name="baudRate">波特率</param>
        /// <param name="dataBits">数据位</param>
        /// <param name="stopBits">停止位</param>
        /// <param name="parity">奇偶校验位</param>
        /// <returns></returns>
        public bool Open(string portName, int baudRate, int dataBits = 8,
            StopBits stopBits = StopBits.One, Parity parity = Parity.None)
        {
            try
            {
                this.serialPort.DataBits = dataBits;
                this.serialPort.StopBits = stopBits;
                this.serialPort.BaudRate = baudRate;
                this.serialPort.Parity = parity;
                this.serialPort.PortName = portName;
                this.serialPort.DtrEnable = true;
                this.serialPort.RtsEnable = true;
                this.serialPort.Open();
                return this.serialPort.IsOpen;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 写
        /// </summary>
        /// <param name="sendData">数据</param>
        public void Write(byte[] sendData)
        {
            serialPort.WriteTimeout = WriteTimeout;
            serialPort.Write(sendData, 0, sendData.Length);
        }

        /// <summary>
        /// 读
        /// </summary>
        /// <param name="length">长度</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        public byte[] Read(int length, int? timeout = null)
        {
            byte[] buf = new byte[length];
            serialPort.ReadTimeout = timeout ?? ReadTimeout;
            int n = serialPort.Read(buf, 0, length);
            byte[] b = buf.Sub(0, n);
            return b;
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            try
            {
                serialPort.Close();
            }
            catch { }
        }

        public string GetIPOrCom()
        {
            return PortName;
        }

        public int GetPortOrBaudRate()
        {
            return BaudRate;
        }

        public string ReadLine(int? timeout = default(int?)) {
            return string.Empty;
        }
    }
}
