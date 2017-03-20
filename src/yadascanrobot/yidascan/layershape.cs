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

        private static bool IsVshape(IList<decimal> lst) {
            var first = lst.First();
            var last = lst.Last();
            
            // 计算中间较大一卷的直径。
            var idx = (lst.Count() - 1) / 2;
            var cenht = Math.Max((double)lst[idx], (double)lst[idx + 1]);

            // 计算两端直径均值加权。
            const double DIFF = 70;
            var endht = (double)(first + last) / 2.0 - DIFF;

            return cenht < endht;
        }

        /// <summary>
        /// 判断一层布卷的直径分布是否规则。
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public static bool isBadShape(IList<LableCode> layer) {
            var lst = layer.OrderBy(x => x.FloorIndex)
                .Select(x => x.Diameter);

            var count = lst.Count();

            if (count == 0 ) {
                return false;
            } else  if (count == 1) {
                return true;
            } else if (count == 2) {
                return IsSlope(lst.First(), lst.Last());
            } else if (count >= 3) {
                return IsSlope(lst.First(), lst.Last()) || IsVshape(lst.ToList());
            }

            return false;
        }
    }
}
