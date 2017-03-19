using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using yidascan.DataAccess;

namespace yidascan {
    public static class PlcSlot {
        /// <summary>
        /// 缓存处来料信号地址。
        /// </summary>
        public static string CACHE_SIGNAL = "MicroWin.S7-1200-3.NewItem108";

        /// <summary>
        /// 缓存区动作编号地址。
        /// </summary>
        public static string CACHE_JOB_SIGNAL = "MicroWin.S7-1200-3.NewItem50";

        /// <summary>
        /// 缓存区动作地址，存布卷位置。
        /// </summary>
        public static string CACHE_JOB_POS_SAVE = "MicroWin.S7-1200-3.NewItem46";

        /// <summary>
        /// 缓存区动作地址，取布卷位置。
        /// </summary>
        public static string CACHE_JOB_POS_GET = "MicroWin.S7-1200-3.NewItem47";

        /// <summary>
        /// 标签朝上采集处来料信号地址。
        /// </summary>
        public static string LABEL_UP_SIGNAL = "MicroWin.S7-1200-3.NewItem113";

        /// <summary>
        /// 标签朝上采集处直径写地址。
        /// </summary>
        public static string LABEL_UP_CATCH_DIAMETER = "MicroWin.S7-1200-3.NewItem51";

        /// <summary>
        /// 标签朝上采集处去向写地址。
        /// </summary>
        public static string LABEL_UP_CATCH_CHANNEL = "MicroWin.S7-1200-3.NewItem52";

        /// <summary>
        /// 抓料处位置1来料信号地址。
        /// </summary>
        public static string ITEM_CATCH_A = "MicroWin.S7-1200-3.NewItem109";

        /// <summary>
        /// 抓料处位置2来料信号地址。
        /// </summary>
        public static string ITEM_CATCH_B = "MicroWin.S7-1200-3.NewItem110";

        /// <summary>
        /// tolocations for bad shape signal.
        /// </summary>
        public static Dictionary<string, string> BadShapeLocations;

        /// <summary>
        /// load LAYER_SHAPE_BAD from database.
        /// </summary>
        public static void loadLayerBadShapeLocation() {
            if (BadShapeLocations != null) {
                BadShapeLocations.Clear();
            } else {
                BadShapeLocations = new Dictionary<string, string>();
            }

            var sql = "select ToLocation, Slot from BadShapeLocation";
            var dt = DataAccess.DataAccess.CreateDataAccess.sa.Query(sql);
            if (dt != null) {
                foreach (DataRow row in dt.Rows) {
                    var k = row["ToLocation"].ToString().ToUpper();
                    var v = row["Slot"].ToString();
                    BadShapeLocations.Add(k, v);
                }
            } 
        }
    }
}
