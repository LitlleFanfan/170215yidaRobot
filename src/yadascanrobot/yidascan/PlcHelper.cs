using System;

using System.Threading;
namespace yidascan {
    /// <summary>
    /// 缓存区动作代码
    /// </summary>
    public enum CacheJob {
        CACHE_JOB_GO = 1, // 直接走。
        CACHE_JOB_SAVE = 2, // 直接存。
        CACHE_JOB_GET_THEN_SAVE = 3, // 先取后存。
        CACHE_JOB_GO_THEN_GET = 4, // 先走后取。
        CACHE_JOB_GET_THEN_GO = 5, // 先取后走。
        CACHE_JOB_SAVE_THEN_GET = 6, // 显存后取。
    }
    
    public static class PlcHelper {
        private const int DELAY = 10;
        
        /// <summary>
        /// 缓存处来料信号
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static bool ReadItemInFromCache(IOpcClient client) {
            var r = client.ReadInt(PlcSlot.CACHE_ITEM_IN);
            return r == 1;
        }
        
        /// <summary>
        /// 写缓存动作
        /// </summary>
        /// <param name="client">opc client.</param>
        /// <param name="job">动作编号。</param>
        /// /// <param name="posSave">动作编号。</param>
        /// /// <param name="posGet">动作编号。</param>
        public static void WriteCacheJob(IOpcClient client, CacheJob job, int posSave, int posGet) {
            client.Write(PlcSlot.CACHE_JOB_SIGNAL, job);
            client.Write(PlcSlot.CACHE_JOB_POS_SAVE, posSave);
            client.Write(PlcSlot.CACHE_JOB_POS_GET, posGet);
            Thread.Sleep(DELAY);
            // 复位来料信号。
            client.Write(PlcSlot.CACHE_ITEM_IN, 0);
        }
        
        /// <summary>
        /// 读标签采集处来料信号。
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static bool ReadLabelCatch(IOpcClient client) {
            var r = client.ReadInt(PlcSlot.LABEL_UP_CATCH_ITEM_IN);
            return r == 1;
        }
       
        /// <summary>
        /// 标签采集处，写布卷直径和去向。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="diameter">直径</param>
        /// <param name="channel">去向, 1 or 2.</param>
        public static void WriteLabelCatch(IOpcClient client, int diameter, int channel) {
            // diameter单位是毫米。
            client.Write(PlcSlot.LABEL_UP_CATCH_DIAMETER, diameter);
            client.Write(PlcSlot.LABEL_UP_CATCH_CHANNEL, channel);
            Thread.Sleep(DELAY);
            // 复位标签采集处来料信号。
            client.Write(PlcSlot.LABEL_UP_CATCH_ITEM_IN, 0);
        }
               
        /// <summary>
        /// 抓料处来料信号。
        /// </summary>
        /// <param name="client"></param>
        /// /// <param name="pos">抓料处位置编号。</param>
        /// <returns></returns>
        public static bool ReadItemCatchSignal(IOpcClient client, int pos) {
            var slot = "";
            if (pos == 1) { slot = PlcSlot.ROLL_CATCH_1; }
            if (pos == 2) { slot = PlcSlot.ROLL_CATCH_2; }

            if (slot == "") { throw new Exception("error pos."); }

            return 1 == client.ReadInt(slot);
        }

        /// <summary>
        /// 复位抓料处信号。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="pos"></param>
        public static void ResetItemCatchSignal(IOpcClient client, int pos) {
            var slot = "";
            if (pos == 1) { slot = PlcSlot.ROLL_CATCH_1; }
            if (pos == 2) { slot = PlcSlot.ROLL_CATCH_2; }

            if (slot == "") { throw new Exception("error pos."); }

            client.Write(slot, 0);
        }

        public static void PushAsideClothRoll(IOpcClient client) {
            client.Write(PlcSlot.PUSH_ASIDE_SIGNAL, 1);
        }

        public static void subscribe(IOpcClient client) {
            client.AddSubscription(PlcSlot.CACHE_ITEM_IN);
            client.AddSubscription(PlcSlot.CACHE_JOB_POS_GET);
            client.AddSubscription(PlcSlot.CACHE_JOB_POS_SAVE);
            client.AddSubscription(PlcSlot.CACHE_JOB_SIGNAL);
            client.AddSubscription(PlcSlot.LABEL_UP_CATCH_CHANNEL);
            client.AddSubscription(PlcSlot.LABEL_UP_CATCH_DIAMETER);
            client.AddSubscription(PlcSlot.LABEL_UP_CATCH_ITEM_IN);
            client.AddSubscription(PlcSlot.PUSH_ASIDE_SIGNAL);
            client.AddSubscription(PlcSlot.ROLL_CATCH_1);
            client.AddSubscription(PlcSlot.ROLL_CATCH_2);
        }
    }
}
