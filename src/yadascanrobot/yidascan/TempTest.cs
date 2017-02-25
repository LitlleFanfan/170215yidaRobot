using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Opc;
using ProduceComm.OPC;
using yidascan.DataAccess;

namespace yidascan {
    public static class TempTest {
        public static void testWeigthConfirmSignel(IOpcClient client, string slot, Action<string> loghandler) {
            var count = 1000;

            while (count-- > 0) {
                var rand = new Random();
                var value = (rand.Next() % 2 == 1) ? 1 : 0;

                client.Write(slot, value);
                Thread.Sleep(20);
                var rv = client.ReadString(slot);

                loghandler?.Invoke($"{value}, {rv}");
            }
        }
    }
}
