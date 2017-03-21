﻿using System;
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
        /// <returns></returns>
        public static bool ReadCacheSignal(IOpcClient client) {
            return client.ReadBool(PlcSlot.CACHE_SIGNAL);
        }

        /// <summary>
        /// 写缓存动作
        /// </summary>
        /// <param name="client">opc client.</param>
        /// <param name="job">动作编号。</param>
        /// /// <param name="posSave">动作编号。</param>
        /// /// <param name="posGet">动作编号。</param>
        public static void WriteCacheJob(IOpcClient client, CacheState job, int posSave, int posGet) {
            client.Write(PlcSlot.CACHE_JOB_SIGNAL, job);
            client.Write(PlcSlot.CACHE_JOB_POS_SAVE, posSave);
            client.Write(PlcSlot.CACHE_JOB_POS_GET, posGet);
            Thread.Sleep(DELAY);
            // 复位来料信号。
            client.Write(PlcSlot.CACHE_SIGNAL, 0);
        }

        /// <summary>
        /// 读标签采集处来料信号。
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static bool ReadLabelUpSignal(IOpcClient client) {
            return client.ReadBool(PlcSlot.LABEL_UP_SIGNAL);
        }

        /// <summary>
        /// 标签采集处，写布卷直径和去向。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="diameter">直径</param>
        /// <param name="channel">去向, 1 or 2.</param>
        public static void WriteLabelUpData(IOpcClient client, decimal diameter, RollCatchChannel channel) {
            // diameter单位是毫米。
            client.Write(PlcSlot.LABEL_UP_CATCH_DIAMETER, diameter);
            client.Write(PlcSlot.LABEL_UP_CATCH_CHANNEL, channel);
            Thread.Sleep(DELAY);
            // 复位标签采集处来料信号。
            client.Write(PlcSlot.LABEL_UP_SIGNAL, 0);
        }

        /// <summary>
        /// 抓料处来料信号。
        /// </summary>
        /// <param name="client"></param>
        /// /// <param name="pos">抓料处位置编号。</param>
        /// <returns></returns>
        public static bool ReadItemCatchSignal(IOpcClient client, int pos) {
            var slot = "";
            if (pos == 1) { slot = PlcSlot.ITEM_CATCH_A; }
            if (pos == 2) { slot = PlcSlot.ITEM_CATCH_B; }

            if (slot == "") { throw new Exception("error pos."); }

            return client.ReadBool(slot);
        }

        /// <summary>
        /// 复位抓料处信号。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="pos"></param>
        public static void ResetItemCatchSignal(IOpcClient client, int pos) {
            var slot = "";
            if (pos == 1) { slot = PlcSlot.ITEM_CATCH_A; }
            if (pos == 2) { slot = PlcSlot.ITEM_CATCH_B; }

            if (slot == "") { throw new Exception("error pos."); }

            client.Write(slot, 0);
        }

        public static void subscribe(IOpcClient client) {
            client.AddSubscription(PlcSlot.CACHE_SIGNAL);
            client.AddSubscription(PlcSlot.CACHE_JOB_POS_GET);
            client.AddSubscription(PlcSlot.CACHE_JOB_POS_SAVE);
            client.AddSubscription(PlcSlot.CACHE_JOB_SIGNAL);
            client.AddSubscription(PlcSlot.LABEL_UP_CATCH_CHANNEL);
            client.AddSubscription(PlcSlot.LABEL_UP_CATCH_DIAMETER);
            client.AddSubscription(PlcSlot.LABEL_UP_SIGNAL);
            client.AddSubscription(PlcSlot.ITEM_CATCH_A);
            client.AddSubscription(PlcSlot.ITEM_CATCH_B);

            foreach (var item in PlcSlot.BadShapeLocations) {
                client.AddSubscription(item.Value);
            }

            //client.AddSubscription("MicroWin.S7-1200.NewItem16");
            //client.AddSubscription("MicroWin.S7-1200.NewItem18");
            //client.AddSubscription("MicroWin.S7-1200.NewItem19");
            //client.AddSubscription("MicroWin.S7-1200.NewItem20");
        }

        /// <summary>
        /// 代替主窗口的同名函数。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="param"></param>
        /// <param name="errorcode">错误码</param>
        public static void ERPAlarmNo(IOpcClient client, OPCParam param, int errorcode) {
            client.Write(param.None.ERPAlarm, errorcode);
        }

        /// <summary>
        /// 删除号码以后，告知opc.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="param"></param>
        /// <param name="lcode">12位长的号码</param>
        public static void NotifyLabelCodeDeleted(IOpcClient client, OPCParam param, string lcode) {
            client.Write(param.DeleteLCode.LCode1, lcode.Substring(0, 6));
            client.Write(param.DeleteLCode.LCode2, lcode.Substring(6, 6));
            client.Write(param.DeleteLCode.Signal, true);
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
            var s = client.ReadString(param.BAreaPanelState[tolocation]);
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
            var lable1 = client.ReadString(slot.LCode1);
            var lable2 = client.ReadString(slot.LCode2);
            return lable1.PadLeft(MAX_LEN, '0') + lable2.PadLeft(MAX_LEN, '0');
        }

        public static void PushAside(IOpcClient client, OPCParam param) {
            const int PUSH_ASIDE = 1;
            client.Write(param.ScanParam.PushAside, PUSH_ASIDE);
        }

        public static void NotifyBadLayerShape(IOpcClient client, string tolocation) {
            const int BAD_SHAPE = 1;
            client.Write(PlcSlot.BadShapeLocations[tolocation], BAD_SHAPE);
        }
    }
}
