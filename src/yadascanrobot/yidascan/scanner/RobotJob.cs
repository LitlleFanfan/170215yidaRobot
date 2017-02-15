using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yidascan {
    public class RobotJobQueue {
        public Queue<RollPosition> Rolls = new Queue<RollPosition>();

        public bool AddRoll(RollPosition roll) {
            lock (Rolls) {
                if (!IsInRoll(roll)) {
                    Rolls.Enqueue(roll);
                    return true;
                }  else {
                    return false;
                }
            }
        }

        public RollPosition PeekRoll() {
            lock(Rolls) {
                return Rolls.Count == 0 ? null : Rolls.Peek();
            }
        }

        public RollPosition GetRoll() {
            lock (Rolls) {
                return Rolls.Count == 0 ?
                    null : Rolls.Dequeue();
            }
        }

        public void ResetRolls() {
            lock (Rolls) {
                while (Rolls.Count > 0) {
                    Rolls.Dequeue();
                }
            }
        }

        /// <summary>
        /// 判断队列中是否已经有该布卷。
        /// </summary>
        /// <param name="roll"></param>
        /// <returns></returns>
        public bool IsInRoll(RollPosition roll) {
            return Rolls.Any(item => {
                return item.IsSameLabel(roll);
            });
        }
    }
}
