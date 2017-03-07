using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

using ProduceComm;
using Newtonsoft.Json;

namespace yidascan {
    public class FakeWebApi : IErpApi {
        public bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) {
            //直接确认，否则打不开    
            return true;
        }

        readonly List<string> location = new List<string>() {  "B01" };//, "B02", "B03", "B04", "B05", "B06", "B07", "B08", "B09", "B10", "A01", "A02", "A03", "C01", "C02", "C03"
        public Dictionary<string, string> Post(string url, Dictionary<string, string> agr, int timeout = 100) {
            Thread.Sleep(30);
            // 取布卷交地。
            if (url == clsSetting.GetLocation) {
                var s = "{ \"State\":\"成功\",\"Msg\":null,\"Data\":\"\",\"ContinueCount\":0}";
                var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(s);
                dic["Data"] = "[{\"LOCATION\":\""+ location[new Random().Next(0, location.Count - 1)] + "\"}]";
                dic.Add("ERPState", "OK");
                return dic;
            } else if (url == clsSetting.ToWeight || url == clsSetting.PanelFinish) {
                var s = "{\"State\":\"成功\",\"Msg\":null,\"Data\":\"\",\"ContinueCount\":0}";
                var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(s);
                dic["Data"] = "[{\"result\":\"OK\"}]";
                dic.Add("ERPState", "OK");
                return dic;
            } else {
                return new Dictionary<string, string> {
                    {"ERPState", "OK" }
                };
            }
        }
        public string Get(string url) {
            return "";
        }
    }
}
