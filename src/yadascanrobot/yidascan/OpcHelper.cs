using System;
using yidascan.DataAccess;

namespace yidascan {
    /// <summary>
    /// 这个要合并修改。
    /// </summary>
    [Obsolete("待重构, 2017-02-17")]
    public static class OpcHelper {
        public static string ReadACAreaLabel(IOpcClient client, LCodeSignal signal) {
            const int MAX_LEN = 6;
            var r1 = client.TryReadString(signal.LCode1);
            var r2 = client.TryReadString(signal.LCode2);
            return r1.PadLeft(MAX_LEN, '0') + r2.PadLeft(MAX_LEN, '0');
        }

        public static int ReadWeighingSignal(IOpcClient client, OPCParam param) {
            return client.TryReadInt(param.WeighParam.GetWeigh);
        }

        public static void WriteWeighingSignal(IOpcClient client, OPCParam param, bool b) {
            var val = b ? "0" : "2";
            client.TryWrite(param.WeighParam.GetWeigh, val);
        }

        public static void WriteACAreaCompletionSignal(IOpcClient client, LCodeSignal signal) {
            client.TryWrite(signal.Signal, 0);
        }

        /// <summary>
        /// 读布卷到达缓冲信号。
        /// </summary>
        /// <param name="client">opc client</param>
        /// <param name="param">opc param</param>
        /// <returns></returns>
        public static bool ReadBeforeCacheSignal(IOpcClient client, OPCParam param) {
            return client.TryReadBool(param.CacheParam.BeforCacheStatus);
        }

        public static void ResetBeforeCacheSignal(IOpcClient client, OPCParam param) {
            client.TryWrite(param.CacheParam.BeforCacheStatus, false);
        }

        /// <summary>
        /// 读扫描状态信号。
        /// </summary>
        /// <param name="client">opc client.</param>
        /// <param name="param">opc param.</param>
        /// <returns></returns>
        public static bool ReadScanState(IOpcClient client, OPCParam param) {
            return client.TryReadBool(param.ScanParam.ScanState);
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
            client.TryWrite(param.ScanParam.ToLocationArea, area);
            client.TryWrite(param.ScanParam.ToLocationNo, numb);
            // 标签条码
            client.TryWrite(param.ScanParam.ScanLable1, label1);
            client.TryWrite(param.ScanParam.ScanLable2, label2);
            // 相机
            client.TryWrite(param.ScanParam.PushAside, camera);
            // 完成信号。
            WriteScanOK(client, param);
        }

        /// <summary>
        /// 布卷条码扫描完成，令托架放下，传送带动。
        /// </summary>
        /// <param name="client">opc client instance.</param>
        /// <param name="param">opc param</param>
        public static void WriteScanOK(IOpcClient client, OPCParam param) {
            client.TryWrite(param.ScanParam.ScanState, true);
        }
    }
}
