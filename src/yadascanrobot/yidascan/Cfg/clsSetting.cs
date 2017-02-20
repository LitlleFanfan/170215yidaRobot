using System.IO;
using System.Reflection;
using System.Collections.Generic;
using ProduceComm;
using System;

namespace ProduceComm {
    public enum LogViewType {
        Both,
        OnlyForm,
        OnlyFile
    }

    public class TimeCount {
        public static long TimeIt(Action act) {
            var sp = new System.Diagnostics.Stopwatch();
            sp.Start();
            act?.Invoke();
            sp.Stop();
            return sp.ElapsedMilliseconds;
        }
    }

    public class LogOpreate {
        public yidascan.MessageCenter msgCenter = new yidascan.MessageCenter();

        public void Write(string msg, string group = "normal", LogViewType type = LogViewType.Both) {           
            switch (type) {
                case LogViewType.Both:
                    msgCenter.Push(msg, group);
                    clsSetting.loger.Error(string.Format("[{0}] {1}", group, msg));
                    break;
                case LogViewType.OnlyForm:
                    msgCenter.Push(msg, group);
                    break;
                case LogViewType.OnlyFile:
                    clsSetting.loger.Error(string.Format("[{0}] {1}", group, msg));
                    break;
            }
        }
    }

    public class clsSetting {
        public static Dictionary<string, int> AreaNo = new Dictionary<string, int> { { "A", 10 }, { "B", 11 }, { "C", 12 } };
        public const string filename = @"config.ini";
        // 常用常数
        public const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";
        public const string LABEL_CODE_DATE_FORMAT = "yyMMdd";
        public const string PRODUCT_NAME = "广州恒微-溢达机器人自动码垛软件";

        private const string ROBOT_IP = "RobotIP";

        /// <summary>
        /// 存于机器人本身的程序名。
        /// </summary>
        public static string JobName {
            get {
                string s = clsSetting.GetCfgValue("JobName");
                return string.IsNullOrEmpty(s) ? "NUMBER9" : s;
            }
            set { clsSetting.SetCfgValue("JobName", value); }
        }

        public static string RobotIP {
            get { return clsSetting.GetCfgValue(ROBOT_IP); }
            set { clsSetting.SetCfgValue(ROBOT_IP, value); }
        }

        public static string FactoryNo {
            get { return clsSetting.GetCfgValue("FactoryNo"); }
            set {
                clsSetting.SetCfgValue("FactoryNo", value);
            }
        }

        public static string LineNo {
            get { return clsSetting.GetCfgValue("LineNo"); }
            set {
                clsSetting.SetCfgValue("LineNo", value);
            }
        }

        public static string ERPService {
            get { return clsSetting.GetCfgValue("ERPService"); }
            set {
                clsSetting.SetCfgValue("ERPService", value);
            }
        }

        public static string GetLocation {
            get { return clsSetting.GetCfgValue("GetLocation"); }
            set {
                clsSetting.SetCfgValue("GetLocation", value);
            }
        }

        public static string PanelFinish {
            get { return clsSetting.GetCfgValue("PanelFinish"); }
            set {
                clsSetting.SetCfgValue("PanelFinish", value);
            }
        }

        public static string ToWeight {
            get { return clsSetting.GetCfgValue("ToWeight"); }
            set {
                clsSetting.SetCfgValue("ToWeight", value);
            }
        }

        public static decimal SplintLength {
            get {
                string tmp = clsSetting.GetCfgValue("SplintLength");
                return decimal.Parse(string.IsNullOrEmpty(tmp) ? "0" : tmp);
            }
            set {
                clsSetting.SetCfgValue("SplintLength", value.ToString());
            }
        }

        public static decimal SplintWidth {
            get {
                string tmp = clsSetting.GetCfgValue("SplintWidth");
                return decimal.Parse(string.IsNullOrEmpty(tmp) ? "0" : tmp);
            }
            set {
                clsSetting.SetCfgValue("SplintWidth", value.ToString());
            }
        }

        public static decimal SplintHeight {
            get {
                string tmp = clsSetting.GetCfgValue("SplintHeight");
                return decimal.Parse(string.IsNullOrEmpty(tmp) ? "0" : tmp);
            }
            set {
                clsSetting.SetCfgValue("SplintHeight", value.ToString());
            }
        }

        public static decimal ShelfWidth {
            get {
                string tmp = clsSetting.GetCfgValue("ShelfWidth");
                return decimal.Parse(string.IsNullOrEmpty(tmp) ? "0" : tmp);
            }
            set {
                clsSetting.SetCfgValue("ShelfWidth", value.ToString());
            }
        }

        public static decimal ShelfTallFirst {
            get {
                string tmp = clsSetting.GetCfgValue("ShelfTallFirst");
                return decimal.Parse(string.IsNullOrEmpty(tmp) ? "0" : tmp);
            }
            set {
                clsSetting.SetCfgValue("ShelfTallFirst", value.ToString());
            }
        }

        public static decimal ShelfTallSecond {
            get {
                string tmp = clsSetting.GetCfgValue("ShelfTallSecond");
                return decimal.Parse(string.IsNullOrEmpty(tmp) ? "0" : tmp);
            }
            set {
                clsSetting.SetCfgValue("ShelfTallSecond", value.ToString());
            }
        }

        public static decimal ShelfTallThird {
            get {
                string tmp = clsSetting.GetCfgValue("ShelfTallThird");
                return decimal.Parse(string.IsNullOrEmpty(tmp) ? "0" : tmp);
            }
            set {
                clsSetting.SetCfgValue("ShelfTallThird", value.ToString());
            }
        }

        public static decimal ShelfTallFourth {
            get {
                string tmp = clsSetting.GetCfgValue("ShelfTallFourth");
                return decimal.Parse(string.IsNullOrEmpty(tmp) ? "0" : tmp);
            }
            set {
                clsSetting.SetCfgValue("ShelfTallFourth", value.ToString());
            }
        }

        /// <summary>
        /// 码垛布卷间隙
        /// </summary>
        public static decimal RollSep {
            get {
                string tmp = clsSetting.GetCfgValue("ShelfObligateLen");
                return decimal.Parse(string.IsNullOrEmpty(tmp) ? "0" : tmp);
            }
            set {
                clsSetting.SetCfgValue("ShelfObligateLen", value.ToString());
            }
        }

        /// <summary>
        /// 奇数层横放
        /// </summary>
        public static bool OddTurn {
            get {
                string tmp = clsSetting.GetCfgValue("OddTurn");
                return bool.Parse(string.IsNullOrEmpty(tmp) ? "True" : tmp);
            }
            set {
                clsSetting.SetCfgValue("OddTurn", value.ToString());
            }
        }

        /// <summary>
        /// 允许偏差宽度
        /// </summary>
        public static decimal AllowDeviation {
            get {
                string tmp = clsSetting.GetCfgValue("AllowDeviation");
                return decimal.Parse(string.IsNullOrEmpty(tmp) ? "0" : tmp);
            }
            set {
                clsSetting.SetCfgValue("AllowDeviation", value.ToString());
            }
        }

        /// <summary>
        /// 边缘预留宽度
        /// </summary>
        public static decimal EdgeSpace {
            get {
                string tmp = clsSetting.GetCfgValue("EdgeObligate");
                return decimal.Parse(string.IsNullOrEmpty(tmp) ? "0" : tmp);
            }
            set {
                clsSetting.SetCfgValue("EdgeObligate", value.ToString());
            }
        }

        /// <summary>
        /// 缓存忽略直径偏差
        /// </summary>
        public static decimal CacheIgnoredDiff {
            get {
                string tmp = clsSetting.GetCfgValue("CacheIgnoredDiff");
                return decimal.Parse(string.IsNullOrEmpty(tmp) ? "0" : tmp);
            }
            set {
                clsSetting.SetCfgValue("CacheIgnoredDiff", value.ToString());
            }
        }

        public const int MaxFloor = 7;

        /// <summary>
        /// OPC服务IP
        /// </summary>
        public static string OPCServerIP {
            get {
                return clsSetting.GetCfgValue("OPCServerIP");
            }
            set {
                clsSetting.SetCfgValue("OPCServerIP", value.ToString());
            }
        }

        public static string ConStr {
            //get { return Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, @"Data.db"); }
            get { return clsSetting.GetCfgValue("ConStr"); }
        }
        #region "params"
        public static string GetCfgValue(string key) {
            string str = null;
            List<string> lst = new FileIo().ReaderFiles(Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, filename));
            if (lst != null) {
                foreach (string s in lst) {
                    int index = s.IndexOf('=');
                    if (index <= 0) {
                        continue;
                    }
                    string k = s.Substring(0, index);
                    string v = s.Substring(index + 1, s.Length - index - 1);
                    if (k == key) {
                        str = v;
                        break;
                    }
                }
            }
            return str;
        }

        public static bool SetCfgValue(string key, string value) {
            List<string> lst = new FileIo().ReaderFiles(Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, filename));
            if (lst != null) {
                for (int i = 0; i < lst.Count; i++) {
                    if (lst[i].Length == 0) {
                        continue;
                    }
                    string[] st = lst[i].Split('=');
                    if (st.Length == 1) {
                        continue;
                    }
                    if (st[0] == key) {
                        lst[i] = key + "=" + value;
                        new FileIo().WriterFile(Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, filename), lst.ToArray());
                        return true;
                    }
                }
            } else {
                lst = new List<string>();
            }
            lst.Add(key + "=" + value);
            new FileIo().WriterFile(Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, filename), lst.ToArray());
            return false;
        }
        #endregion

        /// <summary>
        /// 日志对象
        /// </summary>
        public static NLog.Logger loger = NLog.LogManager.GetCurrentClassLogger();
    }

    public class CommunicationCfg {
        private string Prdfix = string.Empty;
        public CommunicationCfg(string prdfix) {
            Prdfix = prdfix;
        }

        public string CommunicationType {
            get { return clsSetting.GetCfgValue(string.Format("{0}CommunicationType{1}", Prdfix, "")); }
            set {
                clsSetting.SetCfgValue(string.Format("{0}CommunicationType{1}", Prdfix, ""), value);
            }
        }

        public string IPAddr {
            get { return clsSetting.GetCfgValue(string.Format("{0}IPAddr{1}", Prdfix, "")); }
            set {
                clsSetting.SetCfgValue(string.Format("{0}IPAddr{1}", Prdfix, ""), value);
            }
        }
        public string IPPort {
            get { return clsSetting.GetCfgValue(string.Format("{0}IPPort{1}", Prdfix, "")); }
            set {
                clsSetting.SetCfgValue(string.Format("{0}IPPort{1}", Prdfix, ""), value);
            }
        }

        public string ComPort {
            get { return clsSetting.GetCfgValue(string.Format("{0}ComPort{1}", Prdfix, "")); }
            set {
                clsSetting.SetCfgValue(string.Format("{0}ComPort{1}", Prdfix, ""), value);
            }
        }
        public string BaudRate {
            get { return clsSetting.GetCfgValue(string.Format("{0}BaudRate{1}", Prdfix, "")); }
            set {
                clsSetting.SetCfgValue(string.Format("{0}BaudRate{1}", Prdfix, ""), value);
            }
        }
    }
}
