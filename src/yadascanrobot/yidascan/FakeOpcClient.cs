using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using yidascan.DataAccess;

namespace yidascan {
   class FakeOpcClient: IOpcClient {
        OPCParam param;
        private Random rand = new Random();

        public FakeOpcClient(OPCParam param_) {
            param = param_;
        }

        public string ReadString(string slot) {
            Thread.Sleep(100);
            return "";
        }
        public int ReadInt(string slot) {
            Thread.Sleep(100);
            return 1;
        }
        public bool ReadBool(string slot) {
            Thread.Sleep(100);
            foreach(var p in param.ACAreaPanelFinish) {
                if (p.Value.Signal == slot) {
                    return false;
                }
            }
            if(slot== param.ScanParam.ScanState) {
                return false;
            }
            
            return true;
        }
        public decimal ReadDecimal(string slot) {
            Thread.Sleep(100);
            if (slot == param.ScanParam.Diameter) {
                var d = 50 + (decimal)(rand.NextDouble() * 150);
                return Math.Floor(d);
            }

            if (slot == param.ScanParam.Length) {
                var d = 1000 + (decimal)(rand.NextDouble() * 1000);
                return Math.Floor(d);
            }
            return 0;
        }
        public bool Write(string slot, object value) {
            Thread.Sleep(100);
            return true;
        }
        public bool Open(string mAddr) {
            Thread.Sleep(100);
            return true;
        }
        public void AddSubscription(System.Data.DataTable p) {
            Thread.Sleep(100);
            return;
        }
        public bool AddSubscription(string slot) {
            Thread.Sleep(100);
            return true;
        }
        public void Close() {
            return;
        }
    }
}
