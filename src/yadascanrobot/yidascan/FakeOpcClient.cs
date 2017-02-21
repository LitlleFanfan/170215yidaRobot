using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using yidascan.DataAccess;

namespace yidascan {
    /// <summary>
    /// 模拟信号发生。
    /// </summary>
    public class SignalGen {
        static System.Windows.Forms.Timer timerWeigh;
        static System.Windows.Forms.Timer timerCache;
        static System.Windows.Forms.Timer timerLabelUp;
        static System.Windows.Forms.Timer timerItemCatchA;
        static System.Windows.Forms.Timer timerItemCatchB;
        
        public static int WEIGTH_SIGNAL = 0;
        public static bool CACHE_SIGNAL = false;
        public static bool LABELUP_SIGNAL = false;
        public static bool ITEMCATCH_A_SIGNAL = false;
        public static bool ITEMCATCH_B_SIGNAL = false;

        private static int DELAY = 5000;

        public static void startTimerWeigh() {
            timerWeigh = new System.Windows.Forms.Timer {
                Interval = 1000
            };
            timerWeigh.Tick += TimerWeigh_Tick;
            timerWeigh.Enabled = true;
        }

        public static void startTimerCache() {
            timerCache = new System.Windows.Forms.Timer {
                Interval = DELAY
            };
            timerCache.Tick += TimerCache_Tick;
            timerCache.Enabled = true;
        }

        public static void startTimerLabelUp() {
            timerLabelUp = new System.Windows.Forms.Timer {
                Interval = DELAY
            };
            timerLabelUp.Tick += TimerLabelUp_Tick;
            timerLabelUp.Enabled = true;
        }

        public static void startTimerItemCatchA() {
            timerItemCatchA = new System.Windows.Forms.Timer {
                Interval = DELAY
            };
            timerItemCatchA.Tick += TimerItemCatchA_Tick;
            timerItemCatchA.Enabled = true;
        }

        public static void startTimerItemCatchB() {
            timerItemCatchB = new System.Windows.Forms.Timer {
                Interval = DELAY
            };
            timerItemCatchB.Tick += TimerItemCatchB_Tick;
            timerItemCatchB.Enabled = true;
        }

        private static void TimerWeigh_Tick(object sender, EventArgs e) {
            WEIGTH_SIGNAL = 1;
        }

        private static void TimerCache_Tick(object sender, EventArgs e) {
            CACHE_SIGNAL = true;
        }

        private static void TimerLabelUp_Tick(object sender, EventArgs e) {
            LABELUP_SIGNAL = true;
        }

        private static void TimerItemCatchA_Tick(object sender, EventArgs e) {
            ITEMCATCH_A_SIGNAL = true;
        }

        private static void TimerItemCatchB_Tick(object sender, EventArgs e) {
            ITEMCATCH_B_SIGNAL = true;
        }
    }

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
            // 称重处信号
            if (slot == param.ScanParam.GetWeigh) {
                return getIntSignal(ref SignalGen.WEIGTH_SIGNAL);
            }

            return 1;
        }

        private static int getIntSignal(ref int signal) {
            var b = signal;
            if (b==1) { signal = 0; }
            return b;
        }

        private static bool getSignal(ref bool signal) {
            var b = signal;
            if (b) { signal = false; }
            return b;
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

            // 缓存区来料信号
            if (slot == PlcSlot.CACHE_SIGNAL) {
                return getSignal(ref SignalGen.CACHE_SIGNAL);
            }

            // 标签向上处来料信号
            if (slot == PlcSlot.LABEL_UP_SIGNAL) {
                return getSignal(ref SignalGen.LABELUP_SIGNAL);
            }
            
            // 抓料处A
            if (slot == PlcSlot.ITEM_CATCH_A) {
                return getSignal(ref SignalGen.ITEMCATCH_A_SIGNAL);
            }

            // 抓料处B
            if (slot == PlcSlot.ITEM_CATCH_B) {
                return getSignal(ref SignalGen.ITEMCATCH_B_SIGNAL);
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
