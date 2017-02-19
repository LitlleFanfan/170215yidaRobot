using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IOpcClient {
    object Read(string slot);
    string ReadString(string slot);
    int ReadInt(string slot);
    bool ReadBool(string slot);
    bool Write(string slot, object value);
    bool Open(string mAddr);
    void AddSubscription(System.Data.DataTable p);
    void AddSubscription(string slot);
}

