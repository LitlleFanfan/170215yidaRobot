using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace ProduceComm {
    /// <summary>
    /// 暂时不用。
    /// </summary>
    public interface IErpApi {
        bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors err);
        Dictionary<string, string> Post(string url, Dictionary<string, string> agr, int timeout = 100);
        string Get(string url);
    }
}
