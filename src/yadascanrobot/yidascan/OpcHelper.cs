using System;
using yidascan.DataAccess;

namespace yidascan {
    /// <summary>
    /// 这个要合并修改。
    /// </summary>
    [Obsolete("待重构, 2013-02-17")]
    public static class OpcHelper {
        public static string ReadACAreaLabel(IOpcClient client, LCodeSignal signal) {
            const int MAX_LEN = 6;
            var r1 = client.ReadString(signal.LCode1);
            var r2 = client.ReadString(signal.LCode2);
            return r1.PadLeft(MAX_LEN, '0') + r2.PadLeft(MAX_LEN, '0');            
        }

        public static int ReadWeighingSignal(IOpcClient client, OPCParam param) {
            return client.ReadInt(param.ScanParam.GetWeigh);
        }

        public static void WriteWeighingSignal(IOpcClient client, OPCParam param, bool b) {
            var val = b ? "0" : "2";
            client.Write(param.ScanParam.GetWeigh, val);
        }

        public static void WriteACAreaCompletionSignal(IOpcClient client, LCodeSignal signal) {
            client.Write(signal.Signal, 0);
        }

        public static string ReadCacheLabel(IOpcClient client, OPCParam param) {
            const int MAX_LEN = 6;
            var r1 = client.ReadString(param.CacheParam.BeforCacheLable1);
            var r2 = client.ReadString(param.CacheParam.BeforCacheLable2);
            return r1.PadLeft(MAX_LEN, '0') + r2.PadLeft(MAX_LEN, '0');
        }

        /// <summary>
        /// 读布卷到达缓冲信号。
        /// </summary>
        /// <param name="client">opc client</param>
        /// <param name="param">opc param</param>
        /// <returns></returns>
        public static bool ReadBeforeCacheSignal(IOpcClient client, OPCParam param) {
            return client.ReadBool(param.CacheParam.BeforCacheStatus);            
        }

        public static void ResetBeforeCacheSignal(IOpcClient client, OPCParam param) {
            client.Write(param.CacheParam.BeforCacheStatus, false);
        }

        /// <summary>
        /// 读扫描状态信号。
        /// </summary>
        /// <param name="client">opc client.</param>
        /// <param name="param">opc param.</param>
        /// <returns></returns>
        public static bool ReadScanState(IOpcClient client, OPCParam param) {
            return client.ReadBool(param.ScanParam.ScanState);            
        }

        /// <summary>
        /// 标签扫描处理完成以后，发消息告知OPC。
        /// </summary>
        /// <param name="area">交地所在的区代码："A", "B" or "C"</param>
        /// <param name="numb">交地序号</param>
        /// <param name="label1">标签前半部分</param>
        /// <param name="label2">标签后半部分</param>
        /// <param name="camera">相机号</param>
        /// <param name="client">opc client</param>
        /// <param name="param">opc param</param>
        public static void WriteScanOK(IOpcClient client, 
            OPCParam param, 
            string area, 
            string numb, 
            string label1, 
            string label2, 
            string camera) {
            // 交地
            client.Write(param.ScanParam.ToLocationArea, area);
            client.Write(param.ScanParam.ToLocationNo, numb);
            // 标签条码
            client.Write(param.ScanParam.ScanLable1, label1);
            client.Write(param.ScanParam.ScanLable2, label2);
            // 相机
            client.Write(param.ScanParam.CameraNo, camera);
            // 完成信号。
            WriteScanOK(client, param);
        }

        /// <summary>
        /// 布卷条码扫描完成，令托架放下，传送带动。
        /// </summary>
        /// <param name="client">opc client instance.</param>
        /// <param name="param">opc param</param>
        public static void WriteScanOK(IOpcClient client, OPCParam param) {
            client.Write(param.ScanParam.ScanState, true);
        }
    }
}
