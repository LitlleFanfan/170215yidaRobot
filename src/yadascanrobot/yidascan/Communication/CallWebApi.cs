using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using yidascan;

namespace ProduceComm {
    public enum ERPAlarmNo {
        // ERP通信故障
        COMMUNICATION_ERROR = 1,
        // ERP通信正常
        COMMUNICATION_OK = 0
    }
    public class CallWebApi : IErpApi {
        public bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) {
            //直接确认，否则打不开    
            return true;
        }

        public Dictionary<string, string> Post(string url, Dictionary<string, string> agr, int timeout = 100) {
            System.GC.Collect();//垃圾回收，回收没有正常关闭的http连接

            var result = new Dictionary<string, string>();//返回结果

            System.Net.ServicePointManager.Expect100Continue = true;

            HttpWebRequest myHttpWebRequest = null;

            var re = new List<string>();

            foreach (KeyValuePair<string, string> k in agr) {
                re.Add(string.Format("{0}={1}", k.Key, k.Value));
            }
            url = string.Format("{0}{1}{2}", url, re.Count > 0 ? "?" : "", re.Count > 0 ? string.Join("&", re.ToArray()) : "");

            //如果是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase)) {
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
                myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                myHttpWebRequest.ProtocolVersion = HttpVersion.Version10;
            } else {
                myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            }
            myHttpWebRequest.Timeout = timeout * 1000;
            try {
                byte[] bs;
                myHttpWebRequest.Method = "POST";
                myHttpWebRequest.ContentType = "application/json";
                myHttpWebRequest.Accept = "application/json";
                bs = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(agr));

                myHttpWebRequest.ContentLength = bs.Length;

                using (Stream reqStream = myHttpWebRequest.GetRequestStream()) {
                    reqStream.Write(bs, 0, bs.Length);
                }
                using (WebResponse myWebResponse = myHttpWebRequest.GetResponse())
                using (var readStream = new StreamReader(myWebResponse.GetResponseStream(), Encoding.UTF8)) {
                    result = JsonConvert.DeserializeObject<Dictionary<string, string>>(readStream.ReadToEnd());
                    result.Add("ERPState", "OK");
                    lock (FrmMain.opcErp) {
                        // FrmMain.ERPAlarm(FrmMain.opcErp, FrmMain.opcParam, ERPAlarmNo.COMMUNICATION_OK);
                        PlcHelper.ERPAlarmNo(FrmMain.opcErp, FrmMain.opcParam, (int)ERPAlarmNo.COMMUNICATION_OK);
                    }
                }
            } catch (Exception ex) {
                lock (FrmMain.opcErp) {
                    // FrmMain.ERPAlarm(FrmMain.opcErp, FrmMain.opcParam, ERPAlarmNo.COMMUNICATION_ERROR);
                    PlcHelper.ERPAlarmNo(FrmMain.opcErp, FrmMain.opcParam, (int)ERPAlarmNo.COMMUNICATION_ERROR);
                }
                result = new Dictionary<string, string>() { { "ERPState", "Fail" }, { "ERR", "请求接口信息出错 " + ex } };
            }
            return result;
        }

        /// <summary>
        /// 处理http GET请求，返回数据
        /// </summary>
        /// <param name="url">请求的url地址</param>
        /// <returns>http GET成功后返回的数据，失败抛WebException异常</returns>
        public string Get(string url) {
            System.GC.Collect();
            var result = "";

            HttpWebRequest request = null;
            HttpWebResponse response = null;

            //请求url以获取数据
            try {
                //设置最大连接数
                ServicePointManager.DefaultConnectionLimit = 200;
                //设置https验证方式
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase)) {
                    ServicePointManager.ServerCertificateValidationCallback =
                            new RemoteCertificateValidationCallback(CheckValidationResult);
                }

                /***************************************************************
                * 下面设置HttpWebRequest的相关属性
                * ************************************************************/
                request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "GET";

                //设置代理
                //WebProxy proxy = new WebProxy();
                //proxy.Address = new Uri(WxPayConfig.PROXY_URL);
                //request.Proxy = proxy;

                //获取服务器返回
                response = (HttpWebResponse)request.GetResponse();

                //获取HTTP返回数据
                var sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                result = sr.ReadToEnd().Trim();
                sr.Close();
            } catch (System.Threading.ThreadAbortException e) {
                System.Threading.Thread.ResetAbort();
            } catch (WebException e) {
                if (e.Status == WebExceptionStatus.ProtocolError) {
                }
            } catch (Exception e) {
            } finally {
                //关闭连接和流
                if (response != null) {
                    response.Close();
                }
                if (request != null) {
                    request.Abort();
                }
            }
            return result;
        }
    }
}
