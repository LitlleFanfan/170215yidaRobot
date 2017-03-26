using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yidascan.DataAccess;
using ProduceComm;

namespace yidascan {
    /// <summary>
    /// 一组判断层直径分布的函数。
    /// </summary>
    class LayerShape {
        public static int diffSlope = 100;
        public static int diffVshape = 100;

        // 在窗口启动时执行此函数。
        public static void loadconf() {
            diffSlope = clsSetting.DiffSlope;
            diffVshape = clsSetting.DiffVshape;
        }

        public static bool IsSlope(IEnumerable<LableCode> layer) {
            try {
                var count = layer.Count();
                if (count <= 1) { return false; }

                var oddmax = layer.Where(x => x.isOddSide()).Max(x => x.FloorIndex);
                var evenmax = layer.Where(x => x.isEvenSide()).Max(x => x.FloorIndex);
                // 取两端的直径。
                var oddend = layer.Where(x => x.FloorIndex == oddmax).Select(x => x.Diameter).First();
                var evenend = layer.Where(x => x.FloorIndex == evenmax).Select(x => x.Diameter).First();
                
                return Math.Abs(evenend - oddend) > diffSlope;
            } catch (Exception ex) {
                FrmMain.logOpt.Write($"isvshape异常: {ex}", LogType.NORMAL, LogViewType.OnlyFile);
                return false;
            }
        }

        public static bool IsVshape(IEnumerable<LableCode> layer) {
            try {
                var count = layer.Count();
                if (count <= 2) { return false; }

                var oddmax = layer.Where(x => x.isOddSide()).Max(x => x.FloorIndex);
                var evenmax = layer.Where(x => x.isEvenSide()).Max(x => x.FloorIndex);
                // 取两端的直径。
                var oddend = layer.Where(x => x.FloorIndex == oddmax).Select(x => x.Diameter).First();
                var evenend = layer.Where(x => x.FloorIndex == evenmax).Select(x => x.Diameter).First();

                // 计算两端直径均值加权。
                var endht = (double)(oddend + evenend) / 2.0;

                // 计算中间较大一卷的直径。
                var oddfirst = layer.Where(x => x.FloorIndex == 1).Select(x => x.Diameter).First();
                var evenfirst = layer.Where(x => x.FloorIndex == 2).Select(x => x.Diameter).First();

                var cenht = (double)Math.Max(oddfirst, evenfirst);

                return Math.Abs(endht - cenht) > diffVshape;
            } catch (Exception ex) {
                FrmMain.logOpt.Write($"isvshape异常: {ex}", LogType.NORMAL, LogViewType.OnlyFile);
                return false;
            }
        }
    }
}
