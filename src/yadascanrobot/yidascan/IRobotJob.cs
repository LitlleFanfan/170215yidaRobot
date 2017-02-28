using ProduceComm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using yidascan.DataAccess;

namespace yidascan {
    public interface IRobotJob {
        void setup(Action<string, string, LogViewType> loghandler, IOpcClient client, OPCParam param);
        bool IsConnected();
        void JobLoop(ref bool isrunning, ListView viewA, ListView viewB);
        void Dispose();
    }
}
