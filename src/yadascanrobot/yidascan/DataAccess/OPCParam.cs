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

        /// <summary>
        /// 初始化参数同时添加订阅
        /// </summary>
        /// <param name="opc"></param>
        public LCodeSignal(IOpcClient opc, string cls) {
            DataTable dt = OPCParam.Query(string.Format("where Class='{0}'", cls));
            if (dt == null || dt.Rows.Count < 1) {
                return;
            }
            foreach (DataRow dr in dt.Rows) {
                foreach (PropertyInfo property in typeof(LCodeSignal).GetProperties()) {
                    if (property.Name == dr["Name"].ToString()) {
                        property.SetValue(this, dr["Code"].ToString());
                        opc.AddSubscription(dr["Code"].ToString());
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
        /// 通知PLC勾料信号
        /// </summary>
        public string PushAside { get; set; }

        /// <summary>
        /// PLC勾料信号（扫描超时）
        /// </summary>
        public string PlcPushAside { get; set; }

        public const string CFG = "Scan";
        /// <summary>
        /// 初始化参数同时添加订阅
        /// </summary>
        /// <param name="opc"></param>
        public OPCScanParam(IOpcClient opc) {
            DataTable dt = OPCParam.Query($"where Class='{CFG}'");
            if (dt == null || dt.Rows.Count < 1) {
                return;
            }
            foreach (DataRow dr in dt.Rows) {
                foreach (PropertyInfo property in typeof(OPCScanParam).GetProperties()) {
                    if (property.Name == dr["Name"].ToString()) {
                        property.SetValue(this, dr["Code"].ToString());
                        opc.AddSubscription(dr["Code"].ToString());
                    }
                }
            }
        }
    }

    public class OPCWeighParam {
        /// <summary>
        /// 称重
        /// </summary>
        public string GetWeigh { get; set; }
        public string LabelPart1 { get; set; }
        public string LabelPart2 { get; set; }

        public const string CFG = "Weigh";
        /// <summary>
        /// 初始化参数同时添加订阅
        /// </summary>
        /// <param name="opc"></param>
        public OPCWeighParam(IOpcClient opc) {
            DataTable dt = OPCParam.Query($"where Class='{CFG}'");

           FrmMain.logOpt.Write(JsonConvert.SerializeObject(dt), LogType.NORMAL, LogViewType.OnlyFile);
            if (dt == null || dt.Rows.Count < 1) {
                return;
            }
            foreach (DataRow dr in dt.Rows) {
                foreach (PropertyInfo property in typeof(OPCWeighParam).GetProperties()) {
                    if (property.Name == dr["Name"].ToString()) {
                        property.SetValue(this, dr["Code"].ToString());
                        opc.AddSubscription(dr["Code"].ToString());
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
        /// 缓存前标签 
        /// </summary>
        public string BeforCacheLable1 { get; set; }
        /// <summary>
        /// 缓存前标签 
        /// </summary>
        public string BeforCacheLable2 { get; set; }

        /// <summary>
        /// 缓存状态（1走；2存；3取存，同抓子；4走取；5取走；6存取，异抓子）
        /// </summary>
        public string CacheStatus { get; set; }
        /// <summary>
        /// 存入缓存区的位置 
        /// </summary>
        public string CachePoint { get; set; }
        /// <summary>
        /// 从缓存区取出的位置 
        /// </summary>
        public string GetPoint { get; set; }

        public const string CFG = "Cache";
        /// <summary>
        /// 初始化参数同时添加订阅
        /// </summary>
        /// <param name="opc"></param>
        public OPCBeforCacheParam(IOpcClient opc) {
            DataTable dt = OPCParam.Query($"where Class='{CFG}'");
            if (dt == null || dt.Rows.Count < 1) {
                return;
            }
            foreach (DataRow dr in dt.Rows) {
                foreach (PropertyInfo property in typeof(OPCBeforCacheParam).GetProperties()) {
                    if (property.Name == dr["Name"].ToString()) {
                        property.SetValue(this, dr["Code"].ToString());
                        opc.AddSubscription(dr["Code"].ToString());
                    }
                }
            }
        }

    }

    public class OPCLableUpParam {
        /// <summary>
        /// 标签朝上采集（读完写好结果后置空）
        /// </summary>
        public string Signal { get; set; }
        /// <summary>
        /// 直径 （单位毫米）
        /// </summary>
        public string Diameter { get; set; }
        /// <summary>
        /// 去哪道（1-2）
        /// </summary>
        public string Goto { get; set; }

        public const string CFG = "LableUp";
        /// <summary>
        /// 初始化参数同时添加订阅
        /// </summary>
        /// <param name="opc"></param>
        public OPCLableUpParam(IOpcClient opc) {
            DataTable dt = OPCParam.Query($"where Class='{CFG}'");
            if (dt == null || dt.Rows.Count < 1) {
                return;
            }
            foreach (DataRow dr in dt.Rows) {
                foreach (PropertyInfo property in typeof(OPCLableUpParam).GetProperties()) {
                    if (property.Name == dr["Name"].ToString()) {
                        property.SetValue(this, dr["Code"].ToString());
                        opc.AddSubscription(dr["Code"].ToString());
                    }
                }
            }
        }
    }

    public class OPCRobotCarryParam {
        public string RobotCarryA { get; set; }

        public string RobotCarryB { get; set; }

        public const string CFG = "RobotCarry";
        /// <summary>
        /// 初始化参数同时添加订阅
        /// </summary>
        /// <param name="opc"></param>
        public OPCRobotCarryParam(IOpcClient opc) {
            DataTable dt = OPCParam.Query($"where Class='{CFG}'");
            if (dt == null || dt.Rows.Count < 1) {
                return;
            }
            foreach (DataRow dr in dt.Rows) {
                foreach (PropertyInfo property in typeof(OPCRobotCarryParam).GetProperties()) {
                    if (property.Name == dr["Name"].ToString()) {
                        property.SetValue(this, dr["Code"].ToString());
                        opc.AddSubscription(dr["Code"].ToString());
                    }
                }
            }
        }
    }

    public class OPCRobotParam {
        public string RobotStartA { get; set; }

        public string RobotStartB { get; set; }

        public string RobotJobStart { get; set; }

        public const string CFG = "Robot";
        /// <summary>
        /// 初始化参数同时添加订阅
        /// </summary>
        /// <param name="opc"></param>
        public OPCRobotParam(IOpcClient opc) {
            DataTable dt = OPCParam.Query($"where Class='{CFG}'");
            if (dt == null || dt.Rows.Count < 1) {
                return;
            }
            foreach (DataRow dr in dt.Rows) {
                foreach (PropertyInfo property in typeof(OPCRobotParam).GetProperties()) {
                    if (property.Name == dr["Name"].ToString()) {
                        property.SetValue(this, dr["Code"].ToString());
                        opc.AddSubscription(dr["Code"].ToString());
                    }
                }
            }
        }
    }

    public class NoneOpcParame {
        public const string CFG = "None";
        /// <summary>
        /// 初始化参数同时添加订阅
        /// </summary>
        /// <param name="opc"></param>
        public NoneOpcParame(IOpcClient opc) {
            DataTable dt = OPCParam.Query($"where Class='{CFG}'");
            if (dt == null || dt.Rows.Count < 1) {
                return;
            }
            foreach (DataRow dr in dt.Rows) {
                foreach (PropertyInfo property in typeof(NoneOpcParame).GetProperties()) {
                    if (property.Name == dr["Name"].ToString()) {
                        property.SetValue(this, dr["Code"].ToString());
                        opc.AddSubscription(dr["Code"].ToString());
                    }
                }
            }
        }

        /// <summary>
        /// ERP故障
        /// </summary>
        public string ERPAlarm { get; set; }
    }

    public class OPCParam {
        public OPCScanParam ScanParam;

        public OPCWeighParam WeighParam;
        public OPCBeforCacheParam CacheParam;
        public OPCLableUpParam LableUpParam;
        public OPCRobotCarryParam RobotCarryParam;
        public OPCRobotParam RobotParam;

        public LCodeSignal DeleteLCode;

        public Dictionary<string, LCodeSignal> ACAreaPanelFinish;

        public Dictionary<string, string> BAreaPanelFinish;

        public Dictionary<string, string> BAreaFloorFinish;

        public Dictionary<string, string> BAreaPanelState;

        public Dictionary<string, string> BAreaUserFinalLayer;

        public Dictionary<string, string> BadShapeLocations;

        public NoneOpcParame None;

        public static DataTable Query(string where = "") {
            string sql = $"select Name,Code,Class,Remark from NewOPCParam {where}";
            return DataAccess.CreateDataAccess.sa.Query(sql);
        }
        
        public bool InitBAreaPanelFinish(IOpcClient opc) {
            BAreaPanelFinish = new Dictionary<string, string>();
            DataTable dt = Query(string.Format("where Class='BAreaPanelFinish'"));
            if (dt == null || dt.Rows.Count < 1) {
                return false;
            }
            foreach (DataRow dr in dt.Rows) {
                BAreaPanelFinish.Add(dr["Name"].ToString(), dr["Code"].ToString());
                opc.AddSubscription(dr["Code"].ToString());
            }
            return true;
        }
        public bool InitBAreaFloorFinish(IOpcClient opc) {
            BAreaFloorFinish = new Dictionary<string, string>();
            DataTable dt = Query(string.Format("where Class='BAreaFloorFinish'"));
            if (dt == null || dt.Rows.Count < 1) {
                return false;
            }
            foreach (DataRow dr in dt.Rows) {
                BAreaFloorFinish.Add(dr["Name"].ToString(), dr["Code"].ToString());
                opc.AddSubscription(dr["Code"].ToString());
            }
            return true;
        }
        public bool InitBAreaPanelState(IOpcClient opc) {
            BAreaPanelState = new Dictionary<string, string>();
            DataTable dt = Query(string.Format("where Class='BAreaPanelState'"));
            if (dt == null || dt.Rows.Count < 1) {
                return false;
            }
            foreach (DataRow dr in dt.Rows) {
                BAreaPanelState.Add(dr["Name"].ToString(), dr["Code"].ToString());
                opc.AddSubscription(dr["Code"].ToString());
            }
            return true;
        }
        public bool InitBAreaUserFinalLayer(IOpcClient opc) {
            BAreaUserFinalLayer = new Dictionary<string, string>();
            DataTable dt = Query(string.Format("where Class='BAreaUserFinalLayer'"));
            if (dt == null || dt.Rows.Count < 1) {
                return false;
            }
            foreach (DataRow dr in dt.Rows) {
                BAreaUserFinalLayer.Add(dr["Name"].ToString(), dr["Code"].ToString());
                opc.AddSubscription(dr["Code"].ToString());
            }
            return true;
        }
        public bool InitBadShapeLocations(IOpcClient opc) {
            BadShapeLocations = new Dictionary<string, string>();
            DataTable dt = Query(string.Format("where Class='BadShapeLocations'"));
            if (dt == null || dt.Rows.Count < 1) {
                return false;
            }
            foreach (DataRow dr in dt.Rows) {
                BadShapeLocations.Add(dr["Name"].ToString(), dr["Code"].ToString());
                opc.AddSubscription(dr["Code"].ToString());
            }
            return true;
        }

        public bool InitACAreaFinish(IOpcClient opc) {
            DataTable dt = Query("where Class like 'AArea%' or Class like 'CArea%'");
            if (dt == null || dt.Rows.Count < 1) {
                return false;
            }
            ACAreaPanelFinish = new Dictionary<string, yidascan.DataAccess.LCodeSignal>();
            string tmp;
            foreach (DataRow dr in dt.Rows) {
                tmp = dr["Class"].ToString();
                if (!ACAreaPanelFinish.ContainsKey(tmp)) {
                    LCodeSignal p = new LCodeSignal(opc, tmp);
                    ACAreaPanelFinish.Add(tmp, p);
                }
            }
            return true;
        }
    }
}
