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

        public Dictionary<string, string> Post(string url, Dictionary<string, string> agr, int timeout = 100) {
            Thread.Sleep(30);
            // 取布卷交地。
            if (url == clsSetting.GetLocation) {
                var s = "{ \"State\":\"成功\",\"Msg\":null,\"Data\":\"\",\"ContinueCount\":0}";
                var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(s);
                dic["Data"] = "[{\"LOCATION\":\"A03\"}]";
                dic.Add("ERPState", "OK");
                return dic;
            } else if (url == clsSetting.ToWeight || url == clsSetting.PanelFinish) {
                var s = "{\"State\":\"成功\",\"Msg\":null,\"Data\":\"[{\"result\":\"OK\"}]\",\"ContinueCount\":0}";
                var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(s);
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
