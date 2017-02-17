using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace yidascan.DataAccess {
    public class TaskQueues {
        public Queue<LableCode> WeighQ = new Queue<LableCode>();
        public Queue<LableCode> CacheQ = new Queue<LableCode>();
        public Queue<LableCode> LableUpQ = new Queue<LableCode>();
        public Queue<LableCode> CatchAQ = new Queue<LableCode>();
        public Queue<LableCode> CatchBQ = new Queue<LableCode>();
        public Queue<RollPosition> RobotRollAQ = new Queue<RollPosition>();
        public Queue<RollPosition> RobotRollBQ = new Queue<RollPosition>();
        public LableCode[] CacheSide = new LableCode[20];

        public LableCode GetCatchBQ() {
            LableCode code = null;
            lock (CatchBQ) {
                if (CatchBQ.Count > 0) {
                    code = CatchBQ.Dequeue();
                }
            }
            if (code != null) {
                var roll = AddRobotRollQ(code, "B");
                if (roll != null) {
                    lock (RobotRollBQ) {
                        RobotRollBQ.Enqueue(roll);
                    }
                }
            }
            return code;
        }

        public LableCode GetCatchAQ() {
            LableCode code = null;
            lock (CatchAQ) {
                if (CatchAQ.Count > 0) {
                    code = CatchAQ.Dequeue();
                }
            }
            if (code != null) {
                var roll = AddRobotRollQ(code, "A");
                if (roll != null) {
                    lock (RobotRollAQ) {
                        RobotRollAQ.Enqueue(roll);
                    }
                }
            }
            return code;
        }

        private RollPosition AddRobotRollQ(LableCode label, string side) {
            if (label == null) {
                FrmMain.logOpt.Write($"!{side} {label.LCode}找不到", LogType.ROLL_QUEUE);
                return null;
            }
            if (label.Status >= (int)LableState.OnPanel) {
                FrmMain.logOpt.Write($"!{side} {label.LCode}已在板上,未加入队列,交地{label.ToLocation}.", LogType.ROLL_QUEUE);
                return null;
            }
            if (label.CoordinatesIsEmpty()) {
                FrmMain.logOpt.Write("!{side} {label.LCode}未算位置，未加入队列,交地{label.ToLocation}.", LogType.ROLL_QUEUE);
                return null;
            }

            var pinfo = LableCode.GetPanel(label.PanelNo);
            var state = FrmMain.GetPanelState(label, pinfo);
            FrmMain.logOpt.Write(string.Format("{0} {1} {2}", label.LCode, label.ToLocation, Enum.GetName(typeof(PanelState), state)), LogType.ROLL_QUEUE, ProduceComm.LogViewType.OnlyFile);

            decimal x = label.Cx;
            decimal y = label.Cy;
            decimal z = label.Cz + FrmMain.zStart;
            decimal rz = label.Crz;

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
            var success = RobotHelper.robotJobs.AddRoll(roll);

            var msg = success
                ? string.Format("布卷:{0}。", roll.LabelCode)
                : string.Format("重复:{0}", roll.LabelCode);

            FrmMain.logOpt.Write(string.Format((success ? "" : "!") + "{0} {1} {2}", side, msg, label.ToLocation), LogType.ROLL_QUEUE);
            return roll;
        }

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

        public LableCode GetCacheQ() {
            LableCode code = null;
            lock (CacheQ) {
                if (CacheQ.Count > 0) {
                    code = CacheQ.Dequeue();
                }
            }
            if (code != null) {
                lock (LableUpQ) {
                    LableUpQ.Enqueue(code);
                }
            }
            return code;
        }

        public LableCode GetWeighQ() {
            LableCode code = null;
            lock (WeighQ) {
                if (WeighQ.Count > 0) {
                    code = WeighQ.Dequeue();
                }
            }
            if (code != null) {
                lock (CacheQ) {
                    CacheQ.Enqueue(code);
                }
            }
            return code;
        }

    }
}
