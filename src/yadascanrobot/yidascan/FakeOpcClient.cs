﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using yidascan.DataAccess;

namespace yidascan {

#if DEBUG
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

        private static int DELAY = 1000;

        public static void startTimerWeigh() {
            timerWeigh = new System.Windows.Forms.Timer {
                Interval = DELAY
            };
            timerWeigh.Tick += TimerWeigh_Tick;
            timerWeigh.Enabled = true;
        }

        public static void startTimerCache() {
            timerCache = new System.Windows.Forms.Timer {
                Interval = 2000
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

    /// <summary>
    /// 仅用于测试
    /// </summary>
    class FakeOpcClient : IOpcClient {
        OPCParam param;
        private Random rand = new Random();

        #region hand_panel_complete
        // 板完成信号
        private static string SIGNAL_PANEL_HAND_COMPLETE_B01 = "0";

        public static void setPanelLayerCompleteSignal(string tolocation) {
            if (tolocation == "B01") {
                SIGNAL_PANEL_HAND_COMPLETE_B01 = "1";
            }
        }

        public static void resetPanelLayerCompleteSignal(string tolocation) {
            if (tolocation == "B01") {
                SIGNAL_PANEL_HAND_COMPLETE_B01 = "0";
            }
        }

        public static string getPanelLayerCompleteSignalValue(string tolocation) {
            if (tolocation == "B01") {
                return SIGNAL_PANEL_HAND_COMPLETE_B01;
            } else {
                return "0";
            }
        }
        #endregion
        public FakeOpcClient(OPCParam param_) {
            param = param_;
        }

        public string ReadString(string slot) {
            Thread.Sleep(100);

            #region SEND_HAND_COMPLETE_SIGNAL
            // 板完成信号
            var SLOT_B01 = "MicroWin.S7-1200.NewItem29";
            if (slot == SLOT_B01) {
                return SIGNAL_PANEL_HAND_COMPLETE_B01;
            }
            #endregion

            return "";
        }
        public int ReadInt(string slot) {
            Thread.Sleep(100);
            // 称重处信号
            if (slot == param.WeighParam.GetWeigh) {
                return getIntSignal(ref SignalGen.WEIGTH_SIGNAL);
            }

            return 1;
        }

        private static int getIntSignal(ref int signal) {
            var b = signal;
            if (b == 1) { signal = 0; }
            return b;
        }

        private static bool getSignal(ref bool signal) {
            var b = signal;
            if (b) { signal = false; }
            return b;
        }

        public bool ReadBool(string slot) {
            Thread.Sleep(100);

            foreach (var p in param.ACAreaPanelFinish) {
                if (p.Value.Signal == slot) {
                    return false;
                }
            }
            if (slot == param.ScanParam.ScanState) {
                return false;
            }

            // 缓存区来料信号
            if (slot == param.CacheParam.BeforCacheStatus) {
                return getSignal(ref SignalGen.CACHE_SIGNAL);
            }

            // 标签向上处来料信号
            if (slot == param.LableUpParam.Signal) {
                return getSignal(ref SignalGen.LABELUP_SIGNAL);
            }

            // 抓料处A
            if (slot == param.RobotCarryParam.RobotCarryA) {
                return getSignal(ref SignalGen.ITEMCATCH_A_SIGNAL);
            }

            // 抓料处B
            if (slot == param.RobotCarryParam.RobotCarryB) {
                return getSignal(ref SignalGen.ITEMCATCH_B_SIGNAL);
            }

            return true;
        }

        int diameterindex = 0;
        decimal[] diameters = new decimal[] { 215, 270, 270, 140, 240, 275, 165, 205, 210, 140, 140, 170, 180, 140, 175, 145, 146, 150, 190, 130, 110, 140, 120, 215, 270, 270, 140, 240, 275, 215, 270, 270, 140, 240, 275, 175, 145, 146, 150, 193, 135, 110, 144, 220, 175, 145, 146, 150, 290, 130, 110, 140, 120, 90, 86, 215, 270, 270, 140, 240, 275, 165, 205, 210, 140, 140, 170, 180, 140, 175, 145, 146, 150, 190, 75, 69, 100, 90, 86, 75, 69, 100, 90, 86, 75, 69, 215, 270, 270, 140, 240, 275, 215, 270, 270, 140, 240, 275, 100 };
        public decimal ReadDecimal(string slot) {
            Thread.Sleep(100);
            if (slot == param.ScanParam.Diameter) {
                var d = 50 + (decimal)(rand.NextDouble() * 150);//diameters[diameterindex];//

                if (diameterindex == diameters.Length - 1) {
                    diameterindex = 0;
                } else { diameterindex++; }

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
#endif
}
