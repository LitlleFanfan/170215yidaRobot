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
            if (!string.IsNullOrEmpty(panelNo)) {
                // 这个从数据库取似更合理。                
                var data = LableCode.QueryLabelcodeByPanelNo(panelNo);

                if (data == null) {
                    msg = "!板号完成失败，未能查到数据库的标签。";
                    return false;
                }

                var erpParam = new Dictionary<string, string>() {
                        { "Board_No", panelNo },  // first item.
                        { "AllBarCode", string.Join(",", data.ToArray()) } // second item.
                    };
                var re = erpapi.Post(clsSetting.PanelFinish, erpParam);

                var mode = handwork ? "手工" : "自动";
                var paramJson = JsonConvert.SerializeObject(erpParam);
                var resultJson = JsonConvert.SerializeObject(re);
                // show result.
                if (re["ERPState"] == "OK") {
                    if (re["State"] == "Fail") {
                        msg = $"!{mode}板号{paramJson}完成失败, {re["ERR"]}";
                    } else {
                        msg = $"{mode}板号{paramJson}完成成功。{re["Data"]}";
                        return true;
                    }
                } else {
                    msg = $"{0}板号{1}完成失败, {re}";
                    return false;
                }
            }
            msg = "!板号完成失败，板号为空。";
            return false;
        }

        /// <summary>
        /// 手工称重函数。界面按钮调用此函数。
        /// 成功返回true, 失败返回false.
        /// </summary>
        /// <param name="handwork"></param>
        /// <param name="code">标签号码</param>
        /// <param name="callErpApi">IErpApi instance.</param>
        /// <returns></returns>
        public static bool NotifyWeigh(IErpApi callErpApi, string code) {
            var re = callErpApi.Post(clsSetting.ToWeight,
                new Dictionary<string, string>() { { "Fabric_Code", code } });

            if (re["ERPState"] == "OK") {
                var re1 = JsonConvert.DeserializeObject<DataTable>(re["Data"]);
                return (re1.Rows[0]["result"].ToString().ToUpper() != "FAIL");
            } else {
                return false;
            }
        }
    }
}
