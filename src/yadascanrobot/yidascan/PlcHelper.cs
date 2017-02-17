using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;

using ProduceComm;
using ProduceComm.OPC;
using yidascan.DataAccess;


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
        const string SLOT_CACHE_ITEM_IN = "";

        // 缓存处来料信号
        public static bool ReadItemInFromCache(OPCClient client) {
            var r = client.ReadInt(SLOT_CACHE_ITEM_IN);
            return r == 1;
        }

        const string SLOT_CACHE_JOB_SIGNAL = "";
        const string SLOT_CACHE_JOB_POS_SAVE = "";
        const string SLOT_CACHE_JOB_POS_GET = "";

        /// <summary>
        /// 写缓存动作
        /// </summary>
        /// <param name="client">opc client.</param>
        /// <param name="job">动作编号。</param>
        /// /// <param name="posSave">动作编号。</param>
        /// /// <param name="posGet">动作编号。</param>
        public static void WriteCacheJob(OPCClient client, CacheJob job, int posSave, int posGet) {
            client.Write(SLOT_CACHE_JOB_SIGNAL, job);
            client.Write(SLOT_CACHE_JOB_POS_SAVE, posSave);
            client.Write(SLOT_CACHE_JOB_POS_GET, posGet);
            Thread.Sleep(DELAY);
            // 复位来料信号。
            client.Write(SLOT_CACHE_ITEM_IN, 0);
        }

        public const string SLOT_LABEL_CATCH_ITEM_IN = "";
        /// <summary>
        /// 读标签采集处来料信号。
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static bool ReadLabelCatch(OPCClient client) {
            var r = client.ReadInt(SLOT_LABEL_CATCH_ITEM_IN);
            return r == 1;
        }

        // 标签采集处直径写地址。
        public const string SLOT_LABEL_CATCH_DIAMETER = "";
        // 标签采集处去向写地址。
        public const string SLOT_LABEL_CATCH_CHANNEL = "";

        public static void WriteLabelCatch(OPCClient client, int diameter, int channel) {
            client.Write(SLOT_LABEL_CATCH_DIAMETER, diameter);
            client.Write(SLOT_LABEL_CATCH_CHANNEL, channel);
            Thread.Sleep(DELAY);
            // 复位标签采集处来料信号。
            client.Write(SLOT_LABEL_CATCH_ITEM_IN, 0);
        }

        public const string SLOT_ITEM_CATCH_1 = "";
        public const string SLOT_ITEM_CATCH_2 = "";
        /// <summary>
        /// 抓料处来料信号。
        /// </summary>
        /// <param name="client"></param>
        /// /// <param name="pos"></param>
        /// <returns></returns>
        public static bool ReadItemCatchSignal(OPCClient client, int pos) {
            var slot = "";
            if (pos == 1) { slot = SLOT_ITEM_CATCH_1; }
            if (pos == 2) { slot = SLOT_ITEM_CATCH_2; }

            if (slot == "") { throw new Exception("error pos."); }

            return 1 == client.ReadInt(slot);
        }

        public static void ResetItemCatchSignal(OPCClient client, int pos) {
            var slot = "";
            if (pos == 1) { slot = SLOT_ITEM_CATCH_1; }
            if (pos == 2) { slot = SLOT_ITEM_CATCH_2; }

            if (slot == "") { throw new Exception("error pos."); }

            client.Write(slot, 0);
        }
    }
}
