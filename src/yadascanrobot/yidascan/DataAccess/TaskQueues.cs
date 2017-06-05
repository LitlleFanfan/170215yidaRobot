using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ProduceComm;

namespace yidascan.DataAccess {
    public class TaskQueues {
        public DateTime PanelNoFrefix;

        // 交地映射类。
        public static LocationHelper lochelper;
        public static object LOCK_LOCHELPER = new object();

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

        public IEnumerable<LableCode> GetBeforCacheLables(LableCode lc) {
            var lcs = from s in CacheQ where s.ToLocation == lc.ToLocation && s.LCode != lc.LCode select s;
            return lcs.ToList();
        }

        public static Action<string, string> onlog;

        public TaskQueues() {
            PanelNoFrefix = DateTime.Now;
            const int CACHE_LEN = 20;
            CacheSide = new CachePos[CACHE_LEN];
            for (int i = 0; i < CACHE_LEN; i++) {
                CacheSide[i] = new CachePos(i + 1, null);
            }
        }

        /// <summary>
        /// CatchBQ -> RobotRollQ_B
        /// </summary>
        /// <returns></returns>
        public LableCode GetCatchBQ() {
            LableCode code = null;

            if (CatchBQ.Count > 0) {
                code = CatchBQ.Dequeue();
            }

            if (code != null) {
                var roll = AddRobotRollQ(code.LCode, "B");
                if (roll != null) {
                    RobotRollBQ.Enqueue(roll);
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

            if (CatchAQ.Count > 0) {
                code = CatchAQ.Dequeue();
            }

            if (code != null) {
                var roll = AddRobotRollQ(code.LCode, "A");
                if (roll != null) {
                    RobotRollAQ.Enqueue(roll);
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
        private static RollPosition AddRobotRollQ(string lcode, string side) {
            var label = LableCode.QueryByLCode(lcode);
            if (label == null) {
                onlog?.Invoke($"!{side} {lcode}找不到", LogType.ROLL_QUEUE);
                return null;
            }
            if (label.Status >= (int)LableState.OnPanel) {
                onlog?.Invoke($"!{side} {label.LCode}已在板上,未加入机器人队列,交地{label.ToLocation}.", LogType.ROLL_QUEUE);
                return null;
            }
            if (label.CoordinatesIsEmpty()) {
                onlog?.Invoke($"!{side} {label.LCode}未算位置，未加入机器人队列,交地{label.ToLocation}.", LogType.ROLL_QUEUE);
                return null;
            }

            var pinfo = LableCode.GetPanel(label.PanelNo);
            var state = FrmMain.GetPanelState(label, pinfo);

            var x = label.Cx;
            var y = label.Cy;
            var z = label.Cz + RollPosition.Z_START;
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

            if (RollPosition.robotRSidePanel.Contains(label.RealLocation)) {
                rz = rz + 180;
            }

            var roll = new RollPosition(label, side, state, x, y, z, rz);
            onlog?.Invoke($"来源: {nameof(AddRobotRollQ)}, {side} {label.LCode} 名义交地: {label.ToLocation}, 真实交地: {label.RealLocation}, {Enum.GetName(typeof(PanelState), state)}", LogType.ROLL_QUEUE);

            return roll;
        }

        /// <summary>
        /// LableUpQ -> CatchAQ or CatchBQ
        /// </summary>
        /// <param name="isrun"></param>
        /// <returns></returns>
        public LableCode GetLableUpQ() {
            LableCode code = null;

            if (LableUpQ.Count > 0) {
                code = LableUpQ.Peek();
            }

            if (code != null) {
                code.RealLocation = "";

                code.RealLocation = lochelper.Convert(code.ToLocation, code.PanelNo);

                if (string.IsNullOrEmpty(code.RealLocation)) {
                    var msg = $"!来源: {nameof(GetLableUpQ)},{code.LCode} 获取真实交地失败: {code.ToLocation}";
                    onlog?.Invoke(msg, LogType.ROLL_QUEUE);

                    onlog?.Invoke("请查看交地状态。", LogType.ROLL_QUEUE);
                    return null;
                }

                var ok = LableCode.UpdateRealLocation(code);
                if (!ok) { throw new Exception($"保存标签的实际交地失败"); }

                code = LableUpQ.Dequeue();
                if (int.Parse(LableCode.ParseRealLocationNo(code.RealLocation)) < 6) {
                    CatchAQ.Enqueue(code);
                } else {
                    CatchBQ.Enqueue(code);
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

            if (CacheQ.Count > 0) {
                code = CacheQ.Dequeue();
            }

            return code;
        }

        /// <summary>
        /// WeighQ -> CacheQ
        /// </summary>
        /// <returns></returns>
        public LableCode GetWeighQ() {
            LableCode code = null;
            if (WeighQ.Count > 0) {
                code = WeighQ.Dequeue();
            }

            if (code != null && code.ToLocation.Substring(0, 1) == "B") {
                CacheQ.Enqueue(code);
            }
            return code;
        }

        private RollPosition FindodeFromRobotQue(Queue<RollPosition> qu, string tolocation, string panelNo) {
            var lb = RobotRollAQ.LastOrDefault(item => item.RealLocation == tolocation && item.PanelNo == panelNo);
            return lb;
        }

        /// <summary>
        /// 人工满板,找板最后一卷.
        /// </summary>
        /// <param name="tolocation"></param>
        /// <param name="panelNo"></param>
        /// <returns></returns>
        public string UFGetPanelLastRoll(string tolocation, string panelNo) {
            string lcode = string.Empty;
            LableCode lbup;

            // 检索标签朝上队列。            
            lbup = LableUpQ.LastOrDefault(item => item.ToLocation == tolocation && item.PanelNo == panelNo);

            if (lbup != null) {
                lbup.Status = (int)LableState.FloorLastRoll;
                return lbup.LCode;
            }

            var tolocationareaNo = int.Parse(tolocation.Substring(1, 2));

            if (tolocationareaNo < 6) {
                lbup = CatchAQ.LastOrDefault(item => item.ToLocation == tolocation && item.PanelNo == panelNo);

                if (lbup != null) {
                    lbup.Status = (int)LableState.FloorLastRoll;
                    return lbup.LCode;
                }

                var rp = FindodeFromRobotQue(RobotRollAQ, tolocation, panelNo);
                if (rp != null) {
                    rp.Status = (int)LableState.FloorLastRoll;
                    rp.PnlState = PanelState.Full;
                    return rp.LabelCode;
                }
            } else {
                lbup = CatchBQ.LastOrDefault(item => item.ToLocation == tolocation && item.PanelNo == panelNo);

                if (lbup != null) {
                    lbup.Status = (int)LableState.FloorLastRoll;
                    return lbup.LCode;
                }

                var rp = FindodeFromRobotQue(RobotRollBQ, tolocation, panelNo);
                if (rp != null) {
                    rp.Status = (int)LableState.FloorLastRoll;
                    rp.PnlState = PanelState.Full;
                    return rp.LabelCode;
                }
            }
            return lcode;
        }

        /// <summary>
        /// 返回多出额定值(2)的交地列表。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> ValidateCacheSide() {
            const int max = 2;
            var q = CacheSide.Where(x => x.labelcode != null)
                .Select(x => x.labelcode.ToLocation)
                .GroupBy(x => x)
                .Where(x => x.Count() > max)
                .Select(x => x.Key);
            return q;
        }
    }
}
