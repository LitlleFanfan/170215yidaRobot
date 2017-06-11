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
using System.Threading;

namespace yidascan.DataAccess {
    public class PlcSignal {
        string guid = string.Empty;
        int readCount;
        int writeCount;
        string groupName;
        public string ReadSignal { get; set; }
        public string WriteSignal { get; set; }

        public void LoadSN(IOpcClient opc, string readSignal, string writeSignal, string _groupName = "") {
            groupName = _groupName;
            ReadSignal = readSignal;
            WriteSignal = writeSignal;
        }

        public bool ReadSN(IOpcClient opc) {
            int[] rw = GetSignal(opc);

#if DEBUG
            return true;
#endif
            if (rw[0] == 0 && rw[1] != rw[0]) {
                rw = ResetReadCount(opc);
                if (rw[0] == 0 && rw[1] != rw[0]) {
                    ResetSN(opc);
                    FrmMain.logOpt.Write($"来料归零 R {ReadSignal}: {rw[0]} W {WriteSignal}: {rw[1] },上次正常读到R:{readCount} W:{writeCount} ！{guid}",
                        LogType.SIGNAL, LogViewType.OnlyFile);
                    return false;
                }
            }

            if (rw[0] == (rw[1] + 1)) {
                guid = Guid.NewGuid().ToString();
                readCount = rw[0];
                writeCount = rw[1];
                FrmMain.logOpt.Write($"来料 R {ReadSignal}: {rw[0]} W {WriteSignal}: {rw[1] }！{guid}", LogType.SIGNAL, LogViewType.OnlyFile);
                return true;
            } else {
                if (rw[0] != rw[1] && rw[1] != writeCount) {//异常信号修正
                    if (string.IsNullOrEmpty(groupName)) {
                        opc.TryWrite(WriteSignal, writeCount);
                    } else {
                        opc.TryWrite(groupName, WriteSignal, writeCount);
                    }
                    FrmMain.logOpt.Write($"ERR来料 R {ReadSignal}: {rw[0]} W {WriteSignal}: {rw[1] },上次正常读到R:{readCount} W:{writeCount} ！{guid}", 
                        LogType.SIGNAL, LogViewType.OnlyFile);
                }
                return false;
            }
        }

        /// <summary>
        /// 复位校正
        /// </summary>
        /// <param name="opc"></param>
        private int[] ResetReadCount(IOpcClient opc) {
            int[] rw = new int[2];
            var count = 3;
            while (count-- > 0) {
                Thread.Sleep(100);
                rw = GetSignal(opc);
                if (rw[0] != 0) {
                    break;
                }
            }
            return rw;
        }

        private int[] GetSignal(IOpcClient opc) {
            int[] rw = new int[2];
            if (string.IsNullOrEmpty(groupName)) {
                rw[0] = opc.ReadInt(ReadSignal);
                rw[1] = opc.ReadInt(WriteSignal);
            } else {
                rw[0] = opc.ReadInt(groupName, ReadSignal);
                rw[1] = opc.ReadInt(groupName, WriteSignal);
            }
            return rw;
        }

        public bool WriteSN(IOpcClient opc) {
            bool wstate = false;
            writeCount = readCount;
            try {
                if (string.IsNullOrEmpty(groupName)) {
                    wstate = opc.TryWrite(WriteSignal, writeCount);
                } else {
                    wstate = opc.TryWrite(groupName, WriteSignal, writeCount);
                }
            } catch (Exception ex) {
                FrmMain.logOpt.Write($"!{WriteSignal}写失败！",LogType.SIGNAL,LogViewType.OnlyFile);
            }
            FrmMain.logOpt.Write($"来料复位 R {ReadSignal}: {readCount} W {WriteSignal}: {writeCount}！{guid}", LogType.SIGNAL, LogViewType.OnlyFile);
#if DEBUG
            return true;
#endif
            return wstate;
        }

        public void ResetSN(IOpcClient opc) {
            if (string.IsNullOrEmpty(groupName)) {
                opc.TryWrite(WriteSignal, 0);
            } else {
                opc.TryWrite(groupName, WriteSignal, 0);
            }
        }
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

        public string ReadSignal { get; set; }
        public string WriteSignal { get; set; }

        public PlcSignal PlcSn { get; set; }

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

            PlcSn = new PlcSignal();
            PlcSn.LoadSN(opc, ReadSignal, WriteSignal);
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

        public string ReadSignal { get; set; }
        public string WriteSignal { get; set; }

        public PlcSignal PlcSn { get; set; }

        //public string ReadSignalS { get; set; }
        //public string WriteSignalS { get; set; }

        //public PlcSignal PlcSnS { get; set; }

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

            PlcSn = new PlcSignal();
            PlcSn.LoadSN(opc, ReadSignal, WriteSignal);

            //PlcSnS = new PlcSignal();
            //PlcSnS.LoadSN(opc, ReadSignalS, WriteSignalS);
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

        public string ReadSignal { get; set; }
        public string WriteSignal { get; set; }

        public PlcSignal PlcSn { get; set; }

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

            PlcSn = new PlcSignal();
            PlcSn.LoadSN(opc, ReadSignal, WriteSignal);
        }
    }

    public class OPCRobotCarryParam {
        public string RobotCarryA { get; set; }

        public string RobotCarryB { get; set; }

        public string ReadSignalA { get; set; }
        public string WriteSignalA { get; set; }

        public PlcSignal PlcSnA { get; set; }

        public string ReadSignalB { get; set; }
        public string WriteSignalB { get; set; }

        public PlcSignal PlcSnB { get; set; }

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

            PlcSnA = new PlcSignal();
            PlcSnA.LoadSN(opc, ReadSignalA, WriteSignalA);

            PlcSnB = new PlcSignal();
            PlcSnB.LoadSN(opc, ReadSignalB, WriteSignalB);
        }
    }

    public class OPCRobotParam {
        public string RobotStartA { get; set; }

        public string RobotStartB { get; set; }

        public string RobotJobStart { get; set; }

        public string ReadSignalA { get; set; }
        public string WriteSignalA { get; set; }

        public PlcSignal PlcSnA { get; set; }

        public string ReadSignalB { get; set; }
        public string WriteSignalB { get; set; }

        public PlcSignal PlcSnB { get; set; }

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

            PlcSnA = new PlcSignal();
            PlcSnA.LoadSN(opc, ReadSignalA, WriteSignalA);

            PlcSnB = new PlcSignal();
            PlcSnB.LoadSN(opc, ReadSignalB, WriteSignalB);
        }
    }

    public class NoneOpcParame {
        /// <summary>
        /// ERP故障
        /// </summary>
        public string ERPAlarm { get; set; }

        public string CacheStateAlarm { get; set; }

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
