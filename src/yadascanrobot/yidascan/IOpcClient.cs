public interface IOpcClient {
    string ReadString(string slot);
    int ReadInt(string slot);
    bool ReadBool(string slot);
    decimal ReadDecimal(string slot);
    bool Write(string slot, object value);
    bool Set(string slot, object value);
    bool Open(string mAddr);
    void Close();
    void AddSubscription(System.Data.DataTable p);
    bool AddSubscription(string slot);
}

