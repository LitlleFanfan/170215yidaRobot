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
        bool JobTask(ref bool isrun, bool isSideA, Queue<RollPosition> robotRollQ, RollPosition roll, ListView lv);
        void NotifyOpcJobFinished(string panelNo, string tolocation, string reallocation);
        void NotifyOpcJobFinished(RollPosition roll);
        bool PanelAvailable(string tolocation);
        void WriteLocationState(ref bool isrunning, IOpcClient client, OPCParam param);
        void Dispose();
    }
}
