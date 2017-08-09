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
            var count = 500;
            var diffcount = 0;

            while (count-- > 0) {
                var rand = new Random();
                var value = count % 2;

                client.TryWrite(slot, value);
                Thread.Sleep(20);
                var rv = client.TryReadInt(slot);

                diffcount += value == rv ? 0 : 1;
                loghandler?.Invoke($"{value}, {rv}  diff count: {diffcount}  count: {count}");

                Thread.Sleep(100);
            }
        }
    }
}
