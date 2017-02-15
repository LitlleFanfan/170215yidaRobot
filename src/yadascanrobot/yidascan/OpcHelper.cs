using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProduceComm.OPC;
using yidascan.DataAccess;

namespace yidascan {    
    public static class OpcHelper {
        static OPCClient client = null;
        static OPCParam param = null;
        
        /// <summary>
        /// 读布尔值。
        /// </summary>
        /// <param name="slot">地址</param>
        /// <returns>非bool值时，弹出异常。</returns>
        public static bool ReadBool(string slot) {
            return (bool)client.Read(slot);
        }

        public static string ReadString(string slot) {
            return (string)client.Read(slot);
        }

        public static decimal ReadDecimal(string slot) {
            return (decimal)client.Read(slot);
        }

        public static  int ReadInt(string slot) {
            return (int)client.Read(slot);            
        }

        public static string ReadACAreaLabel(LCodeSignal signal) {
            const int MAX_LEN = 6;
            string r1 = ReadString(signal.LCode1);
            string r2 = ReadString(signal.LCode2);
            return r1.PadLeft(MAX_LEN, '0') + r2.PadLeft(MAX_LEN, '0');            
        }

        public static int ReadWeighingSignal(string slot) {
            return ReadInt(param.ScanParam.GetWeigh);
        }

        public static void WriteWeighingSignal(bool b) {
            var val = b ? "0" : "2";
            client.Write(param.ScanParam.GetWeigh, val);
        }

        public static void WriteACAreaCompletionSignal(LCodeSignal signal) {
            client.Write(signal.Signal, 0);
        }

        public static string ReadCacheLabel() {
            const int MAX_LEN = 6;
            string r1 = ReadString(param.CacheParam.BeforCacheLable1);
            string r2 = ReadString(param.CacheParam.BeforCacheLable2);
            return r1.PadLeft(MAX_LEN, '0') + r2.PadLeft(MAX_LEN, '0');
        }

        /// <summary>
        /// 读布卷到达缓冲信号。
        /// </summary>
        /// <returns></returns>
        public static bool ReadBeforeCacheSignal() {
            return ReadBool(param.CacheParam.BeforCacheStatus);            
        }

        public static void ResetBeforeCacheSignal() {
            client.Write(param.CacheParam.BeforCacheStatus, false);
        }

        /// <summary>
        /// 读扫描状态信号。
        /// </summary>
        /// <returns></returns>
        public static bool ReadScanState() {
            return ReadBool(param.ScanParam.ScanState);            
        }

        /// <summary>
        /// 标签扫描处理完成以后，发消息告知OPC。
        /// </summary>
        /// <param name="area">交地所在的区代码："A", "B" or "C"</param>
        /// <param name="numb">交地序号</param>
        /// <param name="label1">标签前半部分</param>
        /// <param name="label2">标签后半部分</param>
        /// <param name="camera">相机号</param>
        public static void WriteScanOK(string area, string numb, string label1, string label2, string camera) {
            // 交地
            client.Write(param.ScanParam.ToLocationArea, area);
            client.Write(param.ScanParam.ToLocationNo, numb);
            // 标签条码
            client.Write(param.ScanParam.ScanLable1, label1);
            client.Write(param.ScanParam.ScanLable2, label2);
            // 相机
            client.Write(param.ScanParam.CameraNo, camera);
            // 完成信号。
            WriteScanOK();
        }

        /// <summary>
        /// 布卷条码扫描完成，令托架放下，传送带动。
        /// </summary>
        public static void WriteScanOK() {
            client.Write(param.ScanParam.ScanState, true);
        }
    }
}
