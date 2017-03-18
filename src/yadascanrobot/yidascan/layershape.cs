using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yidascan.DataAccess;

namespace yidascan {
    /// <summary>
    /// 一组判断层直径分布的函数。
    /// </summary>
    class LayerShape {
        private static bool IsSlope(decimal d1, decimal d2) {
            return Math.Abs(d1 - d2) > 50;
        }

        private static bool IsVshape(IList<decimal> ordered) {
            var first = ordered.First();
            var last = ordered.Last();

            var len = ordered.Count();
            var meanMiddle = (double)(ordered[len / 2] + ordered[len / 2 + 1]) / 2;

            const double FACTOR = 3.0 / 4.0;
            var meanEnds = (double)(first + last) / 2.0 * FACTOR;

            return meanMiddle < meanEnds;
        }

        /// <summary>
        /// 判断一层布卷的直径分布是否规则。
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public static bool isBadShape(IList<LableCode> layer) {
            var ordered = layer.OrderBy(x => x.FloorIndex).Select(x => x.Diameter).ToList();

            if (ordered.Count == 1) {
                return true;
            }

            if (ordered.Count == 2 || ordered.Count == 0) {
                return true;
            }

            if (ordered.Count == 3) {
                return IsSlope(ordered.First(), ordered.Last());
            }

            if (ordered.Count >= 4) {
                return IsSlope(ordered.First(), ordered.Last()) || IsVshape(ordered);
            }

            return false;
        }
    }
}
