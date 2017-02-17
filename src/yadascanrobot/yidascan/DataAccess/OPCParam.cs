using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProduceComm;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using Newtonsoft.Json;

namespace yidascan.DataAccess {
    public enum ERPAlarmNo {
        // ERP通信故障
        COMMUNICATION_ERROR = 1,
        // 取交地失败
        TO_LOCATION_ERROR = 2
    }
    public class LCodeSignal {
        public string LCode1 { get; set; }
        public string LCode2 { get; set; }
        public string Signal { get; set; }

        public LCodeSignal(string cls) {
            DataTable dt = OPCParam.Query(string.Format("where Class='{0}'", cls));
            if (dt == null || dt.Rows.Count < 1) {
                return;
            }
            foreach (DataRow dr in dt.Rows) {
                foreach (PropertyInfo property in typeof(LCodeSignal).GetProperties()) {
                    if (property.Name == dr["Name"].ToString()) {
                        property.SetValue(this, dr["Code"].ToString());
                    }
                }
            }
        }
    }

    public class OPCScanParam {
        /// <summary>
        /// 尺寸处信号
        /// </summary>
        public string SizeState { get; set; }

        /// <summary>
        /// 直径
        /// </summary>
        public string Diameter { get; set; }

        /// <summary>
        /// 长度
        /// </summary>
        public string Length { get; set; }

        /// <summary>
        /// 采集处信号
        /// </summary>
        public string ScanState { get; set; }

        public string ScanLable1 { get; set; }

        public string ScanLable2 { get; set; }

        /// <summary>
        /// 交地
        /// </summary>
        public string ToLocationArea { get; set; }

        /// <summary>
        /// 交地
        /// </summary>
        public string ToLocationNo { get; set; }

        /// <summary>
        /// 相机编号
        /// </summary>
        public string CameraNo { get; set; }

        /// <summary>
        /// 称重
        /// </summary>
        public string GetWeigh { get; set; }

        public OPCScanParam() {
            DataTable dt = OPCParam.Query("where Class='Scan'");
            if (dt == null || dt.Rows.Count < 1) {
                return;
            }
            foreach (DataRow dr in dt.Rows) {
                foreach (PropertyInfo property in typeof(OPCScanParam).GetProperties()) {
                    if (property.Name == dr["Name"].ToString()) {
                        property.SetValue(this, dr["Code"].ToString());
                    }
                }
            }
        }

    }

    public class OPCBeforCacheParam {
        /// <summary>
        /// 缓存前信号
        /// </summary>
        public string BeforCacheStatus { get; set; }

        /// <summary>
        /// 是否缓存
        /// </summary>
        public string CacheStatus { get; set; }

        /// <summary>
        /// 从缓存取出的标签
        /// </summary>
        public string CachePoint { get; set; }
        /// <summary>
        /// 从缓存取出的标签
        /// </summary>
        public string GetPoint { get; set; }

        public OPCBeforCacheParam() {
            DataTable dt = OPCParam.Query("where Class='Cache'");
            if (dt == null || dt.Rows.Count < 1) {
                return;
            }
            foreach (DataRow dr in dt.Rows) {
                foreach (PropertyInfo property in typeof(OPCBeforCacheParam).GetProperties()) {
                    if (property.Name == dr["Name"].ToString()) {
                        property.SetValue(this, dr["Code"].ToString());
                    }
                }
            }
        }

    }

    public class NoneOpcParame {
        public NoneOpcParame() {
            DataTable dt = OPCParam.Query("where Class='None'");
            if (dt == null || dt.Rows.Count < 1) {
                return;
            }
            foreach (DataRow dr in dt.Rows) {
                foreach (PropertyInfo property in typeof(NoneOpcParame).GetProperties()) {
                    if (property.Name == dr["Name"].ToString()) {
                        property.SetValue(this, dr["Code"].ToString());
                    }
                }
            }
        }

        public string RobotWorkState { get; set; }

        public string RobotRunState { get; set; }

        /// <summary>
        /// OPC报警地址
        /// </summary>
        public string ALarmSlot { get; set; }

        /// <summary>
        /// ERP故障
        /// </summary>
        public string ERPAlarm { get; set; }

        /// <summary>
        /// 机器人报警地址。
        /// </summary>
        public string RobotAlarmSlot { get; set; }
    }

    public class OPCParam {
        public OPCScanParam ScanParam;

        public OPCBeforCacheParam CacheParam;

        public LCodeSignal RobotCarryA;

        public LCodeSignal RobotCarryB;

        public LCodeSignal DeleteLCode;

        public Dictionary<string, LCodeSignal> ACAreaPanelFinish;

        public Dictionary<string, string> BAreaPanelFinish;

        public Dictionary<string, string> BAreaFloorFinish;

        public Dictionary<string, string> BAreaPanelState;

        public Dictionary<string, string> BAreaUserFinalLayer;

        public NoneOpcParame None;

        public static DataTable Query(string where = "") {
            string sql = string.Format("select IndexNo,Name,Code,Class,Remark from OPCParam {0} order by IndexNo DESC", where);
            return DataAccess.CreateDataAccess.sa.Query(sql);
        }

        public void Init() {
            InitBAreaPanelFinish();
            InitBAreaFloorFinish();
            InitBAreaPanelState();
            InitBAreaUserFinalLayer();

            None = new NoneOpcParame();
            ScanParam = new OPCScanParam();
            CacheParam = new OPCBeforCacheParam();

            RobotCarryA = new LCodeSignal("RobotCarryA");

            RobotCarryB = new LCodeSignal("RobotCarryB");

            DeleteLCode = new LCodeSignal("DeleteLCode");

            GetACAreaFinishCfg();
        }

        private bool InitBAreaPanelFinish() {
            BAreaPanelFinish = new Dictionary<string, string>();
            DataTable dt = Query(string.Format("where Class='BAreaPanelFinish'"));
            if (dt == null || dt.Rows.Count < 1) {
                return false;
            }
            foreach (DataRow dr in dt.Rows) {
                BAreaPanelFinish.Add(dr["Name"].ToString(), dr["Code"].ToString());
            }
            return true;
        }
        private bool InitBAreaFloorFinish() {
            BAreaFloorFinish = new Dictionary<string, string>();
            DataTable dt = Query(string.Format("where Class='BAreaFloorFinish'"));
            if (dt == null || dt.Rows.Count < 1) {
                return false;
            }
            foreach (DataRow dr in dt.Rows) {
                BAreaFloorFinish.Add(dr["Name"].ToString(), dr["Code"].ToString());
            }
            return true;
        }
        private bool InitBAreaPanelState() {
            BAreaPanelState = new Dictionary<string, string>();
            DataTable dt = Query(string.Format("where Class='BAreaPanelState'"));
            if (dt == null || dt.Rows.Count < 1) {
                return false;
            }
            foreach (DataRow dr in dt.Rows) {
                BAreaPanelState.Add(dr["Name"].ToString(), dr["Code"].ToString());
            }
            return true;
        }
        private bool InitBAreaUserFinalLayer() {
            BAreaUserFinalLayer = new Dictionary<string, string>();
            DataTable dt = Query(string.Format("where Class='BAreaUserFinalLayer'"));
            if (dt == null || dt.Rows.Count < 1) {
                return false;
            }
            foreach (DataRow dr in dt.Rows) {
                BAreaUserFinalLayer.Add(dr["Name"].ToString(), dr["Code"].ToString());
            }
            return true;
        }

        private bool GetACAreaFinishCfg() {
            DataTable dt = Query("where Class like 'AArea%' or Class like 'CArea%'");
            if (dt == null || dt.Rows.Count < 1) {
                return false;
            }
            ACAreaPanelFinish = new Dictionary<string, yidascan.DataAccess.LCodeSignal>();
            string tmp;
            foreach (DataRow dr in dt.Rows) {
                tmp = dr["Class"].ToString();
                if (!ACAreaPanelFinish.ContainsKey(tmp)) {
                    LCodeSignal p = new LCodeSignal(tmp);
                    ACAreaPanelFinish.Add(tmp, p);
                }
            }
            return true;
        }
    }
}
