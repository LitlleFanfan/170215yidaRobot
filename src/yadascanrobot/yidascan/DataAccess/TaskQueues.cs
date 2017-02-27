using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ProduceComm;

namespace yidascan.DataAccess {
    public class TaskQueues {
        /// <summary>
        /// 用于访问锁定。
        /// </summary>
        public Queue<LableCode> WeighQ = new Queue<LableCode>();
        public Queue<LableCode> CacheQ = new Queue<LableCode>();
        public Queue<LableCode> LableUpQ = new Queue<LableCode>();
        public Queue<LableCode> CatchAQ = new Queue<LableCode>();
        public Queue<LableCode> CatchBQ = new Queue<LableCode>();
        public Queue<RollPosition> RobotRollAQ = new Queue<RollPosition>();
        public Queue<RollPosition> RobotRollBQ = new Queue<RollPosition>();
        public CachePos[] CacheSide = null;

        public static Action<string, string> onlog;

        public TaskQueues() {
            int len = 20;
            CacheSide = new CachePos[len];
            for (int i = 0; i < len; i++) {
                CacheSide[i] = new CachePos(i + 1, null);
            }
        }

        /// <summary>
        /// CatchBQ -> RobotRollQ_B
        /// </summary>
        /// <returns></returns>
        public LableCode GetCatchBQ() {
            LableCode code = null;
            lock (CatchBQ) {
                if (CatchBQ.Count > 0) {
                    code = CatchBQ.Dequeue();
                }
            }
            if (code != null) {
                var roll = AddRobotRollQ(code.LCode, "B");
                FrmMain.logOpt.Write($"B {Newtonsoft.Json.JsonConvert.SerializeObject(roll)}", LogType.ROLL_QUEUE);
                if (roll != null) {
                    lock (RobotRollBQ) {
                        RobotRollBQ.Enqueue(roll);
                    }
                }
            }
            return code;
        }

        /// <summary>
        /// 清空所有队列数据
        /// </summary>
        public void clearAll() {
            WeighQ.Clear();
            CacheQ.Clear();
            LableUpQ.Clear();
            CatchAQ.Clear();
            CatchBQ.Clear();
            RobotRollAQ.Clear();
            RobotRollBQ.Clear();
            for (int i = 0; i < CacheSide.Count(); i++) {
                CacheSide[i].labelcode = null;
            }
        }

        /// <summary>
        /// CatchAQ -> RobotRollQ_A
        /// </summary>
        /// <returns></returns>
        public LableCode GetCatchAQ() {
            LableCode code = null;
            lock (CatchAQ) {
                if (CatchAQ.Count > 0) {
                    code = CatchAQ.Dequeue();
                }
            }
            if (code != null) {
                var roll = AddRobotRollQ(code.LCode, "A");
                FrmMain.logOpt.Write($"A {Newtonsoft.Json.JsonConvert.SerializeObject(roll)}", LogType.ROLL_QUEUE);
                if (roll != null) {
                    lock (RobotRollAQ) {
                        RobotRollAQ.Enqueue(roll);
                    }
                }
            }
            return code;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="side"></param>
        /// <param name="lcode">todo: describe lcode parameter on AddRobotRollQ</param>
        /// <returns></returns>
        private RollPosition AddRobotRollQ(string lcode, string side) {
            LableCode label = LableCode.QueryByLCode(lcode);
            if (label == null) {
                FrmMain.logOpt.Write($"!{side} {label.LCode}找不到", LogType.ROLL_QUEUE);
                return null;
            }
            if (label.Status >= (int)LableState.OnPanel) {
                onlog?.Invoke($"!{side} {label.LCode}已在板上,未加入队列,交地{label.ToLocation}.", LogType.ROLL_QUEUE);
                return null;
            }
            if (label.CoordinatesIsEmpty()) {
                onlog?.Invoke("!{side} {label.LCode}未算位置，未加入队列,交地{label.ToLocation}.", LogType.ROLL_QUEUE);
                return null;
            }

            var pinfo = LableCode.GetPanel(label.PanelNo);
            var state = FrmMain.GetPanelState(label, pinfo);
            onlog?.Invoke($"{label.LCode} {label.ToLocation} {Enum.GetName(typeof(PanelState), state)}", LogType.ROLL_QUEUE);

            var x = label.Cx;
            var y = label.Cy;
            var z = label.Cz + FrmMain.zStart;
            var rz = label.Crz;

            if (rz == 0) {
                y = RollPosition.GetToolOffSet(y);
            } else {
                x = RollPosition.GetToolOffSet(x);
            }

            if (x + y > 0) {
                if (rz == 0) {
                    rz = -180;
                }
            } else {
                if (rz != 0) {
                    rz = rz * -1;
                }
            }
            if (RollPosition.robotRSidePanel.Contains(label.ToLocation)) {
                rz = rz + 180;
            }
            var roll = new RollPosition(label.LCode, side, label.ToLocation, state, x, y, z, rz);
            onlog?.Invoke($"{side} {label.LCode} {label.ToLocation}", LogType.ROLL_QUEUE);
            return roll;
        }

        /// <summary>
        /// LableUpQ -> CatchAQ or CatchBQ
        /// </summary>
        /// <returns></returns>
        public LableCode GetLableUpQ() {
            LableCode code = null;
            lock (LableUpQ) {
                if (LableUpQ.Count > 0) {
                    code = LableUpQ.Dequeue();
                }
            }
            if (code != null) {
                if (int.Parse(code.ParseLocationNo()) < 6) {
                    lock (CatchAQ) {
                        CatchAQ.Enqueue(code);
                    }
                } else {
                    lock (CatchBQ) {
                        CatchBQ.Enqueue(code);
                    }
                }
            }
            return code;
        }

        /// <summary>
        /// CatcheQ -> LableUpQ
        /// </summary>
        /// <returns></returns>
        public LableCode GetCacheQ() {
            LableCode code = null;
            lock (CacheQ) {
                if (CacheQ.Count > 0) {
                    code = CacheQ.Dequeue();
                }
            }
            return code;
        }

        /// <summary>
        /// WeighQ -> CacheQ
        /// </summary>
        /// <returns></returns>
        public LableCode GetWeighQ() {
            LableCode code = null;
            lock (WeighQ) {
                if (WeighQ.Count > 0) {
                    code = WeighQ.Dequeue();
                }
            }
            if (code != null && code.ToLocation.Substring(0, 1) == "B") {
                lock (CacheQ) {
                    CacheQ.Enqueue(code);
                }
            }
            return code;
        }
    }
}
