using System;
namespace ProduceComm {
    public enum CommunicationType {
        Network,
        SerialPort
    }

    public interface ICommunication {
        string GetIPOrCom();
        int GetPortOrBaudRate();
        bool Open();
        void Write(byte[] sendData);
        byte[] Read(int length, int? timeout = null);
        string ReadLine(int? timeout = null);
        void Close();
    }
}
