using System.Collections.Generic;

public interface IOpcClient {
    string ReadString(string slot);
    int ReadInt(string slot);
    bool ReadBool(string slot);
    decimal ReadDecimal(string slot);
    string ReadString(string groupname, string slot);
    int ReadInt(string groupname, string slot);
    bool ReadBool(string groupname, string slot);
    decimal ReadDecimal(string groupname, string slot);
    bool Write(string slot, object value);
    bool Write(string groupname, Dictionary<string, object> codeValue);
    bool Set(string slot, object value);
    bool Open(string mAddr);
    void Close();
    void AddSubscription(System.Data.DataTable p);
    bool AddSubscription(string slot);
    bool AddSubscriptions(string goupname, List<string> codes, int updateRate = 500);
}

