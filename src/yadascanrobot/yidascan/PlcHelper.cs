using System;
using System.Threading;
using yidascan.DataAccess;

namespace yidascan {
    public enum RollCatchChannel {
        /// <summary>
        /// 通道1
        /// </summary>
        channel_1 = 1,

        /// <summary>
        /// 通道2
        /// </summary>
        channel_2 = 2,
    }

    public static class PlcHelper {
        private const int DELAY = 10;

        /// <summary>
        /// 缓存处来料信号
        /// </summary>
        /// <param name="client"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool ReadCacheSignal(IOpcClient client, OPCParam param) {
            var r = param.CacheParam.PlcSn.ReadSN(client);
            return r;
        }
        public static string ReadCacheLabble(IOpcClient client, OPCParam param) {
            return client.TryReadString(param.CacheParam.BeforCacheLable1).PadLeft(6, '0') +
                client.TryReadString(param.CacheParam.BeforCacheLable2).PadLeft(6, '0');
        }

        /// <summary>
        /// 写缓存动作
        /// </summary>
        /// <param name="client">opc client.</param>
        /// <param name="job">动作编号。</param>
        /// <param name="posSave">动作编号。</param>
        /// <param name="posGet">动作编号。</param>
        /// <param name="param"></param>
        /// <param name="lcode"></param>
        public static void WriteCacheJob(IOpcClient client, OPCParam param, CacheState job, int posSave, int posGet, string lcode) {
            client.TryWrite(param.CacheParam.BeforCacheLable1, lcode.Substring(0, 6));
            client.TryWrite(param.CacheParam.BeforCacheLable2, lcode.Substring(6, 6));
            client.TryWrite(param.CacheParam.CacheStatus, job);
            client.TryWrite(param.CacheParam.CachePoint, posSave);
            client.TryWrite(param.CacheParam.GetPoint, posGet);
            Thread.Sleep(DELAY);
            // 复位来料信号。
            param.CacheParam.PlcSn.WriteSN(client);
            client.TryWrite(param.CacheParam.BeforCacheStatus, 0);
        }

        /// <summary>
        /// 标签采集处，写布卷直径和去向。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="diameter">直径</param>
        /// <param name="channel">去向, 1 or 2.</param>
        /// <param name="param"></param>
        public static void WriteLabelUpData(IOpcClient client, OPCParam param, decimal diameter, RollCatchChannel channel) {
            // diameter单位是毫米。
            client.TryWrite(param.LableUpParam.Diameter, diameter);
            client.TryWrite(param.LableUpParam.Goto, channel);
            Thread.Sleep(DELAY);
            // 复位标签采集处来料信号。
            client.TryWrite(param.LableUpParam.Signal, 0);
            param.LableUpParam.PlcSn.WriteSN(client);
        }

        /// <summary>
        /// 抓料处来料信号。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="pos">抓料处位置编号。</param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool ReadItemCatchSignal(IOpcClient client, OPCParam param, int pos) {
            var slot = "";
            if (pos == 1) { slot = param.RobotCarryParam.RobotCarryA; }
            if (pos == 2) { slot = param.RobotCarryParam.RobotCarryB; }

            if (slot == "") { throw new Exception("error pos."); }

            return client.TryReadBool(slot);
        }

        /// <summary>
        /// 复位抓料处信号。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="pos"></param>
        /// <param name="param"></param>
        public static void ResetItemCatchSignal(IOpcClient client, OPCParam param, int pos) {
            var slot = "";
            if (pos == 1) { slot = param.RobotCarryParam.RobotCarryA; }
            if (pos == 2) { slot = param.RobotCarryParam.RobotCarryB; }

            if (slot == "") { throw new Exception("error pos."); }

            client.TryWrite(slot, 0);
        }

        /// <summary>
        /// 代替主窗口的同名函数。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="param"></param>
        /// <param name="errorCode">错误码</param>
        public static void ERPAlarmNo(IOpcClient client, OPCParam param, int errorCode) {
            client.TryWrite(param.None.ERPAlarm, errorCode);
        }

        /// <summary>
        /// 删除号码以后，告知opc.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="param"></param>
        /// <param name="lcode">12位长的号码</param>
        public static void NotifyLabelCodeDeleted(IOpcClient client, OPCParam param, string lcode) {
            client.TryWrite(param.DeleteLCode.LCode1, lcode.Substring(0, 6));
            client.TryWrite(param.DeleteLCode.LCode2, lcode.Substring(6, 6));
            client.TryWrite(param.DeleteLCode.Signal, true);
        }

        /// <summary>
        /// 从plc读有板信号
        /// </summary>
        /// <param name="client"></param>
        /// <param name="param"></param>
        /// <param name="tolocation">交地</param>
        /// <returns></returns>
        public static bool IsPanelAvailable(IOpcClient client, OPCParam param, string tolocation) {
            const string PANEL_OK = "2";
            var s = client.TryReadString(param.BAreaPanelState[tolocation]);
            return s == PANEL_OK;
        }

        /// <summary>
        /// 读取完整标签号码。
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static string ReadCompleteLable(IOpcClient client, LCodeSignal slot) {
            const int MAX_LEN = 6;
            var lable1 = client.TryReadString(slot.LCode1);
            var lable2 = client.TryReadString(slot.LCode2);
            return lable1.PadLeft(MAX_LEN, '0') + lable2.PadLeft(MAX_LEN, '0');
        }

        public static void PushAside(IOpcClient client, OPCParam param) {
            const int PUSH_ASIDE = 1;
            client.TryWrite(param.ScanParam.PushAside, PUSH_ASIDE);
        }

        public static bool IsPushedAside(IOpcClient client, OPCParam param, Action onResetError = null) {
            var slot = param.ScanParam.PlcPushAside;
            if (client.TryReadBool(slot)) {
                var resetok = client.TryWrite(slot, 0);                
                if(!resetok) {
                    onResetError?.Invoke(); // 复位失败
                }
                return true;
            } else {
                return false;
            }
        }

        public static bool WriteScanResult(IOpcClient client, OPCParam param, string area, string locationNo, string codepart1, string codepart2) {
            client.TryWrite(param.ScanParam.ToLocationArea, area);
            client.TryWrite(param.ScanParam.ToLocationNo, locationNo);
            // write label.
            client.TryWrite(param.ScanParam.ScanLable1, codepart1);
            client.TryWrite(param.ScanParam.ScanLable2, codepart2);
            // write camera no. and set state true.
            return client.TryWrite(param.ScanParam.ScanState, true);
        }

        public static void NotifyBadLayerShape(IOpcClient client, OPCParam param, string tolocation) {
            const int BAD_SHAPE = 1;
            client.TryWrite(param.BadShapeLocations[tolocation], BAD_SHAPE);
        }

        public static void NotifyFullPanel(IOpcClient client, OPCParam param, string relloc) {
            client.TryWrite(param.BAreaPanelFinish[relloc], true);
            FrmMain.logOpt.Write($"{relloc}: 满板信号发出。slot: {param.BAreaPanelFinish[relloc]}", LogType.ROBOT_STACK);
            const int SIGNAL_3 = 3;
            client.TryWrite(param.BAreaPanelState[relloc], SIGNAL_3);
            FrmMain.logOpt.Write($"{relloc}: 板状态信号发出，状态值: {SIGNAL_3}。slot: {param.BAreaPanelState[relloc]}", LogType.ROBOT_STACK);
        }

        public static void HeartBeat(IOpcClient client, string slot, int count) {
            client.TryWrite(slot, count);
        }

        public static int HeartBeatBack(IOpcClient client, string slot) {
            return client.TryReadInt(slot);
        }

        public static string GetLabelCodeWhenWeigh(IOpcClient client, OPCParam param) {
            var code1 = client.TryReadString(param.WeighParam.LabelPart1).PadLeft(6, '0');
            var code2 = client.TryReadString(param.WeighParam.LabelPart2).PadLeft(6, '0');
            return code1 + code2;
        }
    }
}
