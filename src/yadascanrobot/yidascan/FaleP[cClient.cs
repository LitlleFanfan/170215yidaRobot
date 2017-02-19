using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yidascan {
   class FakeOpcClient: IOpcClient {
        public string ReadString(string slot) {
            return "";
        }
        public int ReadInt(string slot) {
            return 1;
        }
        public bool ReadBool(string slot) {
            return true;
        }
        public decimal ReadDecimal(string slot) {
            return 0;
        }
        public bool Write(string slot, object value) {
            return true;
        }
        public bool Open(string mAddr) {
            return true;
        }
        public void AddSubscription(System.Data.DataTable p) {
            return;
        }
        public bool AddSubscription(string slot) {
            return true;
        }
        public void Close() {
            return;
        }
    }
}
