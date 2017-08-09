using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using Newtonsoft.Json;

using ProduceComm; // ierp interface.
using yidascan.DataAccess; // Lablecode defination.

namespace yidascan {
    public static class ErpHelper {
        public static Action<string> onlog;

        /// <summary>
        /// 通知失败时，要告知plc
        /// FrmMain.ERPAlarm(FrmMain.opcClient, FrmMain.opcParam, ERPAlarmNo.COMMUNICATION_ERROR);
        /// </summary>
        /// <param name="erpapi"></param>
        /// <param name="panelNo"></param>
        /// <param name="msg"></param>
        /// <param name="handwork"></param>
        /// <returns></returns>
        public static bool NotifyPanelEnd(IErpApi erpapi, string panelNo, out string msg, bool handwork = false) {
            if (string.IsNullOrEmpty(panelNo)) {
                msg = "!板号完成失败，板号为空。";
                return false;
            }
            // 这个从数据库取似更合理。
            var data = LableCode.QueryLabelcodeByPanelNo(panelNo);

            if (data == null) {
                msg = $"!{panelNo}板号完成失败，未能查到数据库的标签。";
                return false;
            }

            var erpParam = new Dictionary<string, string> {
                        { "Board_No", panelNo },  // first item.
                        { "AllBarCode", string.Join(",", data.ToArray()) } // second item.
                    };
            var re = erpapi.Post(clsSetting.PanelFinish, erpParam, clsSetting.ErpTimeout);

            var mode = handwork ? "手工" : "自动";
            var paramJson = JsonConvert.SerializeObject(erpParam);
            var resultJson = JsonConvert.SerializeObject(re);
            // show result.
            if (re["ERPState"] == "OK") {
                if (re["State"] == "Fail") {
                    msg = $"!{mode}板号{paramJson}完成失败, {re["ERR"]}";
                    return false;
                } else {
                    msg = $"{mode}板号{paramJson}完成成功。{re["Data"]}";
                    return true;
                }
            } else {
                msg = $"{mode}板号{paramJson}完成失败, {resultJson}";
                return false;
            }
        }

        /// <summary>
        /// 手工称重函数。界面按钮调用此函数。
        /// 成功返回true, 失败返回false.
        /// </summary>
        /// <param name="code">标签号码</param>
        /// <param name="callErpApi">IErpApi instance.</param>
        /// <returns></returns>
        public static bool NotifyWeigh(IErpApi callErpApi, string code) {
            var re = callErpApi.Post(clsSetting.ToWeight,
                new Dictionary<string, string> { { "Fabric_Code", code } }, clsSetting.ErpTimeout);

            if (re["ERPState"] == "OK") {
                var re1 = JsonConvert.DeserializeObject<DataTable>(re["Data"]);
                return (re1.Rows[0]["result"].ToString().ToUpper() != "FAIL");
            } else {
                return false;
            }
        }

        //private bool NotifyWeigh(string code, bool handwork = true) {
        //    try {
        //        var re = callErpApi.Post(clsSetting.ToWeight,
        //            new Dictionary<string, string>() { { "Fabric_Code", code } }, clsSetting.ErpTimeout);

        //        var msg = string.Format("{0} {1}称重{2}", code, (handwork ? "手工" : "自动"), JsonConvert.SerializeObject(re));
        //        logOpt.Write(msg, LogType.NORMAL, LogViewType.OnlyFile);

        //        if (re["ERPState"] == "OK") {
        //            var re1 = JsonConvert.DeserializeObject<DataTable>(re["Data"]);
        //            return (re1.Rows[0]["result"].ToString().ToUpper() != "FAIL");
        //        } else {
        //            return false;
        //        }
        //    } catch (Exception ex) {
        //        logOpt.Write($"!来源: {nameof(NotifyWeigh)}, {ex}");
        //        return false;
        //    }
        //}

        // 取交地失败的话，应当告知plc。
        private static string GetLocation(IErpApi erpapi, string code) {
            var re = "";
            Dictionary<string, string> str;
            try {
                str = erpapi.Post(clsSetting.GetLocation,
                    new Dictionary<string, string> { { "Bar_Code", code } },
                    clsSetting.ErpTimeout);
                var res = JsonConvert.DeserializeObject<DataTable>(str["Data"].ToString());
                if (str["ERPState"] == "OK") {
                    if (res.Rows.Count > 0 && res.Rows[0]["LOCATION"].ToString() != "Fail") {
                        re = res.Rows[0]["LOCATION"].ToString();
                    }
                }
            } catch (Exception ex) {
                onlog?.Invoke($"!{ex}");
                return string.Empty;
            }

            return re;
        }

        /// <summary>
        ///  [交地]|[长度]
        /// </summary>
        /// <param name="code"></param>
        /// <param name="erpapi"></param>
        /// <param name="warnhandler"></param>
        /// <returns>[交地]|[长度]</returns>
        public static string GetLocationAndLength(IErpApi erpapi, string code, Action<string, bool> warnhandler) {
            var tolocation = string.Empty;
            var dt = LableCode.QueryByLCode(code);

            if (dt != null) {
                FrmMain.logOpt.Write("!号码重复: {code}}", LogType.NORMAL);
                warnhandler?.Invoke("重复扫码", true);
            } else {
                var str = new Dictionary<string, string>();
                try {
                    str = erpapi.Post(clsSetting.GetLocation,
                        new Dictionary<string, string> { { "Bar_Code", code } },
                        clsSetting.ErpTimeout);
                    if (str["ERPState"] == "OK") {
                        var res = JsonConvert.DeserializeObject<DataTable>(str["Data"].ToString());
                        if (res.Rows.Count > 0 && res.Rows[0]["LOCATION"].ToString() != "Fail") {
                            tolocation = res.Rows[0]["LOCATION"].ToString();
                            FrmMain.logOpt.Write($"号码: {code}, 交地: {tolocation}", LogType.NORMAL);
                        } else {
                            warnhandler?.Invoke("取交地失败", true);
                            FrmMain.logOpt.Write($"!{code}获取交地失败。{JsonConvert.SerializeObject(str)}", LogType.NORMAL);
                        }
                    } else {
                        warnhandler?.Invoke("取交地失败", true);
                        FrmMain.logOpt.Write($"!{code}获取交地失败。{JsonConvert.SerializeObject(str)}", LogType.NORMAL);
                    }
                } catch (Exception ex) {
                    warnhandler?.Invoke("取交地失败", true);
                    FrmMain.logOpt.Write($"!来源: {nameof(GetLocationAndLength)}, {ex}, {JsonConvert.SerializeObject(str)}", LogType.NORMAL);
                }
            }
            return tolocation;
        }
    }
}
