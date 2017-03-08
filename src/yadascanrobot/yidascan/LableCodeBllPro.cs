using System;
using System.Collections.Generic;
using System.Linq;
using ProduceComm;
using yidascan.DataAccess;

namespace yidascan {
    class LableCodeBllPro {
        #region PRIVATE_FUNCTIONS
        private static bool IsRollInSameSide(LableCode lc, LableCode currlc) {
            return lc.FloorIndex > 0 && lc.FloorIndex % 2 == currlc.FloorIndex % 2 && lc.LCode != currlc.LCode;
        }
        private static bool IsRollInSameSide(LableCode lc, int findex) {
            return lc.FloorIndex > 0 && lc.FloorIndex % 2 == findex % 2;
        }

        private static void CalculatePosition(List<LableCode> lcs, LableCode lc) {
            lc.FloorIndex = CalculateFloorIndex(lcs);

            var z = lc.Floor == 1 ? -35 : LableCode.GetFloorMaxDiameter(lc.PanelNo, lc.Floor) - 10;
            var r = clsSetting.OddTurn ?
                (lc.Floor % 2 == 1 ? 0 : 90) : //奇数层横放
                (lc.Floor % 2 == 1 ? 90 : 0); //偶数层横放

            var xory = CalculateXory(lcs, lc);

            //z,r,x/y
            lc.Coordinates = string.Format("{0},{1},{2}", z, r, xory);
            lc.Cx = r == 0 ? 0 : xory;
            lc.Cy = r == 0 ? xory : 0;
            lc.Cz = z;
            lc.Crz = r;
        }

        private static decimal CalculateXory(List<LableCode> lcs, LableCode lc) {
            decimal xory;
            if (lc.FloorIndex <= 2) {
                xory = lc.FloorIndex % 2 == 1 ? 0 : -clsSetting.RollSep;
            } else {
                var lastRoll = (from s in lcs where IsRollInSameSide(s, lc)
                                orderby s.FloorIndex descending select s).First();
                xory = (Math.Abs(lastRoll.Cx + lastRoll.Cy) + lastRoll.Diameter + clsSetting.RollSep)
                    * (lc.FloorIndex % 2 == 1 ? 1 : -1);
                //FrmMain.logOpt.Write($"lastroll:{lastRoll.LCode} x:{lastRoll.Cx} y:{lastRoll.Cy}  currroll:{lc.LCode} xory:{xory}", LogType.BUFFER);
            }

            return xory;
        }

        private static decimal CalculateXory(List<LableCode> lcs) {
            var index = CalculateFloorIndex(lcs);
            decimal xory;
            if (index <= 2) {
                xory = index % 2 == 1 ? 0 : -clsSetting.RollSep;
            } else {
                var lastRoll = (from s in lcs where IsRollInSameSide(s, index)
                                orderby s.FloorIndex descending select s).First();
                xory = (Math.Abs(lastRoll.Cx + lastRoll.Cy) + lastRoll.Diameter + clsSetting.RollSep)
                    * (index % 2 == 1 ? 1 : -1);
            }

            return xory;
        }

        private static int CalculateFloorIndex(List<LableCode> lcs) {
            if (lcs == null) {
                return 1;
            }

            var oddcount = lcs.Count(x => { return x.isOddSide(); });
            var evencount = lcs.Count(x => { return x.isEvenSide(); });

            if (oddcount == 0) { return 1; }
            if (evencount == 0) { return 2; }
            return oddcount > evencount ? 2 * evencount + 2 : 2 * oddcount + 1;
        }

        private static void CalculatePosition(List<LableCode> lcs, LableCode lc, LableCode lc2) {
            CalculatePosition(lcs, lc);
            lc2.FloorIndex = lc.FloorIndex + 2;

            var d = Math.Abs(lc.Cx + lc.Cy) + lc.Diameter + clsSetting.RollSep;
            var xory = d * (lc.FloorIndex % 2 == 1 ? 1 : -1);

            lc2.Coordinates = string.Format("{0},{1},{2}", lc.Cz, lc.Crz, xory);

            lc2.Cx = lc.Crz == 0 ? 0 : xory;
            lc2.Cy = lc.Crz == 0 ? xory : 0;
            lc2.Cz = lc.Cz;
            lc2.Crz = lc.Crz;
        }

        /// <summary>
        /// 当前边是否满
        /// </summary>
        /// <param name="lcs">当前层所有标签</param>
        /// <param name="lc">当前标签</param>
        /// <returns></returns>
        private static LableCode IsPanelFull(List<LableCode> lcs, LableCode lc) {
            // 以下是新的做法。
            // 满则返回莫格缓存中的布卷
            // 不满则返回null
            var cachedRolls = (from s in lcs where s.FloorIndex == 0 select s).ToList();
            var MAX_WIDTH = FindMaxHalfWidth(lc);
            // 板上宽度
            var installedWidth = Math.Abs(CalculateXory(lcs));

            // FrmMain.logOpt.Write($"***loc: {lc.ToLocation}, cached count: {cachedRolls.Count()}, max width: {MAX_WIDTH}, installed width: {installedWidth}","buffer");

            return findSmallerFromCachedRolls(cachedRolls, lc, installedWidth, MAX_WIDTH);
        }

        /// <summary>
        /// 当前边是否满
        /// </summary>
        /// <param name="lc">边上的标签</param>
        /// <returns></returns>
        private static bool IsPanelFull(LableCode lc) {
            var MAX_WIDTH = FindMaxHalfWidth(lc);
            return expectedWidth(lc) > MAX_WIDTH;
        }

        private static decimal FindMaxHalfWidth(LableCode lc) {
            var lenOfUpperFloor = lc.Floor > 1
                ? LableCode.GetFloorHalfAvgLength(lc.PanelNo, lc.Floor)
                : 0;

            if (lenOfUpperFloor > 0) {
                return Math.Min(lenOfUpperFloor, clsSetting.SplintLength / 2);
            } else {
                // 默认最大宽度
                return (clsSetting.SplintLength / 2);
            }
        }

        /// <summary>
        /// 2期缓存计算办法
        /// </summary>
        /// <param name="pinfo"></param>
        /// <param name="lc"></param>
        /// <param name="lcs"></param>
        /// <param name="cacheq">缓存队列</param>
        /// <returns>返回的是需要从缓存位取出的布卷。如果不需要取出，返回null。</returns>
        private static CalResult CalculateCache(PanelInfo pinfo, LableCode lc, List<LableCode> lcs) {
            CalResult cr = new CalResult(CacheState.Go, lc, null);
            var cachecount = (pinfo.OddStatus ? 0 : 1) + (pinfo.EvenStatus ? 0 : 1);
            var cachedRools = from s in lcs
                              where s.FloorIndex == 0
                              orderby s.Diameter ascending
                              select s;
            switch (cachedRools.Count()) {
                case 0://当前层已没有了缓存。//当前布卷直接缓存起来。
                    cr.state = CacheState.Cache;
                    break;
                case 1://当前层只有一卷缓存。
                case 2://当前层有两卷缓存。
                    if (cachedRools.Count() == cachecount)//缓存卷数与需要缓存卷数相等
                    {
                        var lcObjs = new List<LableCode>();
                        foreach (LableCode l in cachedRools) {
                            // 缓存的比当前的直径小
                            var cachedBiggerThanCurrent = (l.Diameter + clsSetting.CacheIgnoredDiff < lc.Diameter);
                            if (cachedBiggerThanCurrent) {
                                lcObjs.Add(l);
                            }
                        }
                        if (lcObjs.Count > 0) {
                            cr.CodeFromCache = lcObjs[0];//前面数据源已经排过序
                            lc.GetOutLCode = cr.CodeFromCache.LCode;//换掉的标签---//当前布卷直接缓存起来。缓存的两卷中小的拿出来并计算位置。//当前布卷不需要缓存，计算位置。
                            cr.state = CacheState.GetThenCache;
                        }
                    } else {
                        cr.state = CacheState.Cache;
                    }
                    break;
            }
            return cr;
        }

        private static bool NoMoreBiggerRoolsInCacheQ(LableCode lc, IEnumerable<LableCode> cacheq) {
            // 同一个板上直径比当前大的。
            var q = cacheq.Count((x) => {
                return lc.Diameter > x.Diameter + clsSetting.CacheIgnoredDiff;
            });
            return q <= 0;
        }

        /// <summary>
        /// 当前布卷和缓存中的一个布卷可能占用的坐标
        /// </summary>
        /// <param name="l1"></param>
        /// <param name="l2"></param>
        /// <param name="installedWidth">板上已有布卷总宽度</param>
        /// <returns></returns>
        private static decimal expectedWidth(decimal installedWidth, LableCode l1, LableCode l2) {
            // 预期宽度
            var expected = installedWidth + l1.Diameter + clsSetting.RollSep + l2.Diameter + clsSetting.EdgeSpace;
            return expected;
        }

        /// <summary>
        /// 当前布卷和缓存中的一个布卷可能占用的坐标
        /// </summary>
        /// <param name="l1"></param>
        /// <returns></returns>
        private static decimal expectedWidth(LableCode l1) {
            // 预期宽度
            var expected = Math.Abs(l1.Cx + l1.Cy) + clsSetting.EdgeSpace;
            return expected;
        }

        /// <summary>
        /// 从缓存布卷中，小于当前直径的布卷中，取最小的那个。
        /// </summary>
        /// <param name="cachedRolls"></param>
        /// <param name="current"></param>
        /// <param name="installedWidth"></param>
        /// <param name="maxedWidth"></param>
        /// <returns></returns>
        private static LableCode findSmallerFromCachedRolls(List<LableCode> cachedRolls, LableCode current, decimal installedWidth, decimal maxedWidth) {
            LableCode rt = null;
            foreach (var item in cachedRolls) {
                if (expectedWidth(installedWidth, current, item) < maxedWidth) {
                    continue;
                }
                if (rt == null || item.Diameter < rt.Diameter) {
                    rt = item;
                }
            }
            return rt;
        }
        #endregion

        #region PUBLIC_FUNCTIONS
        public static PanelInfo GetPanelNo(LableCode lc, string dateShiftNo) {
            var pf = LableCode.GetTolactionCurrPanelNo(lc.ToLocation, dateShiftNo);
            lc.SetupPanelInfo(pf);
            return pf;
        }

        public static CalResult AreaBCalculate(IErpApi erpapi, LableCode lc, string dateShiftNo, IEnumerable<LableCode> cacheq) {
            var rt = new CalResult(CacheState.Cache, lc, null);
            var pi = GetPanelNo(rt.CodeCome, dateShiftNo);
            var pinfo = pi == null ? new PanelInfo() : pi;
            var fp = FloorPerformance.None;
            var layerLabels = new List<LableCode>();

            if (pinfo != null) {
                // 取当前交地、当前板、当前层所有标签。
                layerLabels = LableCode.GetLableCodesOfRecentFloor(rt.CodeCome.ToLocation, pinfo);

                if (layerLabels != null && layerLabels.Count > 0) {
                    // 最近一层没满。
                    rt.CodeFromCache = IsPanelFull(layerLabels, rt.CodeCome);

                    if (rt.CodeFromCache != null) //不为NULL，表示满
                    {
                        if (rt.CodeCome.Diameter > rt.CodeFromCache.Diameter + clsSetting.CacheIgnoredDiff) {
                            rt.state = CacheState.GetThenGo;
                        } else {
                            rt.state = CacheState.GoThenGet;
                        }
                    } else {
                        //计算缓存，lc2不为NULL需要缓存
                        CalResult cr = CalculateCache(pinfo, rt.CodeCome, layerLabels);
                        rt.CodeFromCache = cr.CodeFromCache;
                        rt.state = cr.state;
                    }
                }
            }

            var msg = $"";
            switch (rt.state) {
                case CacheState.Go:
                    CalculatePosition(layerLabels, rt.CodeCome);
                    break;
                case CacheState.GetThenCache:
                case CacheState.CacheAndGet:
                    CalculatePosition(layerLabels, rt.CodeFromCache);
                    break;
                case CacheState.GoThenGet:
                    //计算位置坐标, 赋予层号
                    rt.CodeCome.Floor = rt.CodeFromCache.Floor;
                    CalculatePosition(layerLabels, rt.CodeCome, rt.CodeFromCache);

                    fp = SetFullFlag(rt, pinfo);
                    break;
                case CacheState.GetThenGo:
                    rt.CodeCome.Floor = rt.CodeFromCache.Floor;
                    CalculatePosition(layerLabels, rt.CodeFromCache, rt.CodeCome);

                    fp = SetFullFlag(rt, pinfo);
                    break;
                case CacheState.Cache:
                    var cancachesum = (pinfo.OddStatus ? 0 : 1) + (pinfo.EvenStatus ? 0 : 1);
                    var cachelcs = (from s in layerLabels
                                    where s.FloorIndex == 0
                                    orderby s.Diameter ascending
                                    select s).Count();

                    bool go = CanIgo(cacheq, rt, cancachesum - cachelcs);
                    if (go) {
                        rt.state = CacheState.Go;
                        CalculatePosition(layerLabels, rt.CodeCome);

                        if (IsPanelFull(rt.CodeCome)) {
                            fp = SetFullFlag(rt, pinfo);
                        }

                        msg = $"cache change go";
                    }
                    break;
                default:
                    break;
            }

            var savestate = false;

            if (pi == null) {
                // 产生新板号赋予当前标签。
                //板第一卷
                savestate = LableCode.Update(rt.CodeCome);
            } else if (rt.CodeFromCache != null) {
                savestate = LableCode.Update(fp, pinfo, rt.CodeCome, rt.CodeFromCache);
            } else {
                savestate = LableCode.Update(fp, pinfo, rt.CodeCome);
            }

            if (fp == FloorPerformance.BothFinish && rt.CodeCome.Floor == pinfo.MaxFloor) {
                var re = ErpHelper.NotifyPanelEnd(erpapi, pinfo.PanelNo, out msg);
            }
            rt.message = msg;
            return rt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheq"></param>
        /// <param name="rt"></param>
        /// <param name="cancachecount">可缓存卷数</param>
        /// <returns></returns>
        private static bool CanIgo(IEnumerable<LableCode> cacheq, CalResult rt, int cancachecount) {
            var go = false;
            var biggers = 0;
            switch (cancachecount) {
                case 0:
                    break;
                case 1:
                    biggers = GetBiggerCount(cacheq, rt, 1);
                    if (biggers >= 1) {
                        go = true;
                    }
                    break;
                case 2:
                    biggers = GetBiggerCount(cacheq, rt, 2);
                    if (biggers >= 2) {
                        go = true;
                    }
                    break;
            }
            return go;
        }

        private static int GetBiggerCount(IEnumerable<LableCode> cacheq, CalResult rt, int takeCount) {
            return (from s in cacheq.Take(takeCount) where s.Diameter > rt.CodeCome.Diameter + clsSetting.CacheIgnoredDiff select s).Count();
        }

        private static FloorPerformance SetFullFlag(CalResult rt, PanelInfo pinfo) {
            FloorPerformance fp;
            if (rt.CodeCome.FloorIndex % 2 == 0) {
                pinfo.EvenStatus = true;
                fp = FloorPerformance.EvenFinish;
            } else {
                pinfo.OddStatus = true;
                fp = FloorPerformance.OddFinish;
            }
            if (pinfo.EvenStatus && pinfo.OddStatus)
                fp = FloorPerformance.BothFinish;
            return fp;
        }
        #endregion
    }

    /// <summary>
    /// 用于传递缓存区号码比对处理的结果。
    /// </summary>
    public class CalResult {
        public CacheState state { get; set; }
        /// <summary>
        /// 缓存区来料标签
        /// </summary>
        public LableCode CodeCome { get; set; }

        /// <summary>
        /// 将要从缓存区取出的标签
        /// </summary>
        public LableCode CodeFromCache { get; set; }

        /// <summary>
        /// 附加消息
        /// </summary>
        public string message { get; set; }

        public CalResult(CacheState state_, LableCode codeToCache_, LableCode codeFromCache_) {
            state = state_;
            CodeCome = codeToCache_;
            CodeFromCache = codeFromCache_;
            message = "";
        }
    }

    public class SideFull {
        bool IsFull;
        LableCode FromCache;
    }
}
