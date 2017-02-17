using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yidascan {
    public interface IOpcClient {
        object Read(string slot);
        string ReadString(string slot);
        int ReadInt(string slot);
        bool ReadBool(string slot);
        void Write(string slot, object value);
    }
}
