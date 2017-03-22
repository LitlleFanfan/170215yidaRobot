using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
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

            var z = lc.Floor == 1 ? clsSetting.InitHeigh : LableCode.GetFloorMaxDiameter(lc.PanelNo, lc.Floor) - (lc.Floor * 5);
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
            }

            return xory;
        }

        /// <summary>
        /// 下一卷的坐标
        /// </summary>
        /// <param name="lcs"></param>
        /// <returns></returns>
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
        [Obsolete("use isPanelFull instead.")]
        private static LableCode IsPanelFull(List<LableCode> lcs, LableCode lc) {
            // 以下是新的做法。
            // 满则返回莫格缓存中的布卷
            // 不满则返回null
            var cachedRolls = (from s in lcs where s.FloorIndex == 0 select s).ToList();
            var MAX_WIDTH = FindMaxHalfWidth(lc);
            // 板上宽度
            var installedWidth = Math.Abs(CalculateXory(lcs));
            return findSmallerFromCachedRolls(cachedRolls, lc, installedWidth, MAX_WIDTH);
        }

        private static PanelFullState IsPanelFullPro(List<LableCode> lcs, LableCode lc) {
            // 以下是新的做法。
            // 满则返回莫格缓存中的布卷
            // 不满则返回null
            var cachedRolls = (from s in lcs where s.FloorIndex == 0 select s).ToList();
            var MAX_WIDTH = FindMaxHalfWidth(lc);
            // 板上宽度
            var installedWidth = Math.Abs(CalculateXory(lcs));
            return findSmallerFromCachedRollsPro(cachedRolls, lc, installedWidth, MAX_WIDTH);
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
            var cr = new CalResult(CacheState.Go, lc, null);
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

        // 
        private static decimal expectedWidthNoEdgeSpace(decimal installedWidth, LableCode l1, LableCode l2) {
            // 预期宽度
            var expected = installedWidth + l1.Diameter + clsSetting.RollSep + l2.Diameter;
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
        [Obsolete("use findSmallerFromCachedRollsPro instead.")]
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

        private static PanelFullState findSmallerFromCachedRollsPro(List<LableCode> cachedRolls, LableCode current, decimal installedWidth, decimal max) {
            // 预计宽度超出
            var e = clsSetting.EdgeSpace;
            var rt = cachedRolls.Where(x => (expectedWidthNoEdgeSpace(installedWidth, current, x) > max - e))
                .OrderBy(x => x.Diameter)
                .FirstOrDefault();

            if (rt == null) {
                return new PanelFullState(PanelFullState.NO_FULL, null);
            } else {
                var w = expectedWidthNoEdgeSpace(installedWidth, current, rt);

                if (w >= max) {
                    return new PanelFullState(PanelFullState.EXCEED, rt);
                } else if (w > max - e && w < max) {
                    return new PanelFullState(PanelFullState.FULL, rt);
                } else {
                    return null;
                }
            }
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

        #region PUBLIC_FUNCTIONS

        /// <summary>
        /// 从数据库里取板号，并赋给标签对象。
        /// 如果取出的空的，就新产生一个板号。
        /// </summary>
        /// <param name="lc">标签</param>
        /// <param name="dateShiftNo">任务码</param>
        /// <returns></returns>
        public static PanelInfo GetPanelNo(LableCode lc, string dateShiftNo) {
            var pf = LableCode.GetTolactionCurrPanelNo(lc.ToLocation, dateShiftNo);
            lc.SetupPanelInfo(pf);
            return pf;
        }

        public static CalResult AreaBCalculate(IOpcClient client, LableCode lc, string dateShiftNo, IEnumerable<LableCode> cacheq, Action<string> onlog) {
            var rt = new CalResult(CacheState.Cache, lc, null);
            var pinfo = GetPanelNo(rt.CodeCome, dateShiftNo);

            var fp = FloorPerformance.None;
            var layerLabels = new List<LableCode>();

            // 解决在缓存位，同一交地出现多个不同板号的情况.
            if (pinfo == null) { LableCode.Update(fp, new PanelInfo(rt.CodeCome.PanelNo), rt.CodeCome, null); }

            if (pinfo != null) {
                // 取当前交地、当前板、当前层所有标签。
                layerLabels = LableCode.GetLableCodesOfRecentFloor(rt.CodeCome.ToLocation, pinfo.PanelNo, pinfo.CurrFloor);

                if (layerLabels != null && layerLabels.Count > 0) {
                    // 最近一层没满。
                    var fullstate = IsPanelFullPro(layerLabels, rt.CodeCome);
                    rt.CodeFromCache = fullstate.fromCache;

                    if (fullstate.state == PanelFullState.FULL) {
                        if (rt.CodeCome.Diameter > rt.CodeFromCache.Diameter + clsSetting.CacheIgnoredDiff) {
                            rt.state = CacheState.GetThenGo;
                        } else {
                            rt.state = CacheState.GoThenGet;
                        }
                    } else if (fullstate.state == PanelFullState.NO_FULL) {
                        var cr = CalculateCache(pinfo, rt.CodeCome, layerLabels);
                        rt.CodeFromCache = cr.CodeFromCache;
                        rt.state = cr.state;
                    } else if (fullstate.state == PanelFullState.EXCEED) {
                        onlog($"!交地: {lc.ToLocation}, current: {lc.LCode} {lc.Coordinates}, from cache: {rt.CodeFromCache.LCode} {rt.CodeFromCache.Coordinates}, 超出板宽。");
                        if (rt.CodeCome.Diameter > rt.CodeFromCache.Diameter + clsSetting.CacheIgnoredDiff) {
                            rt.state = CacheState.GetThenGo;
                        } else {
                            rt.state = CacheState.GoThenGet;
                        }
                    } else {
                        // can not happen.
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
                    var cancachesum = pinfo == null ? 2 : (pinfo.OddStatus ? 0 : 1) + (pinfo.EvenStatus ? 0 : 1);
                    var cachelcs = (from s in layerLabels
                                    where s.FloorIndex == 0
                                    orderby s.Diameter ascending
                                    select s).Count();

                    var go = CanIgo(cacheq, rt, cancachesum - cachelcs);
                    if (go) {
                        rt.state = CacheState.Go;
                        CalculatePosition(layerLabels, rt.CodeCome);

                        if (pinfo != null && IsPanelFull(rt.CodeCome)) {
                            fp = SetFullFlag(rt, pinfo);
                        }

                        msg = $"cache change go";
                    }
                    break;
                default:
                    break;
            }


            // 记录层最后一卷
            if (fp == FloorPerformance.BothFinish) {
                if (rt.state == CacheState.GoThenGet) {
                    rt.CodeFromCache.Status = 2;
                    rt.CodeFromCache.Remark = $"{rt.CodeFromCache.Remark} 层最后一卷";
                } else {
                    rt.CodeCome.Status = 2;
                    rt.CodeCome.Remark = $"{rt.CodeCome.Remark} 层最后一卷";
                }
            }

            var savestate = false;

            if (pinfo == null) {
                // 产生新板号赋予当前标签。
                //板第一卷
                savestate = LableCode.Update(rt.CodeCome);
            } else if (rt.CodeFromCache != null) {
                savestate = LableCode.Update(fp, pinfo, rt.CodeCome, rt.CodeFromCache);
            } else {
                savestate = LableCode.Update(fp, pinfo, rt.CodeCome);
            }
            rt.message = msg;
            return rt;
        }

        [Obsolete("应当使用ErpHelper.NotifyPanelEnd")]
        public static bool NotifyPanelEnd(IErpApi erpapi, string panelNo, out string msg, bool handwork = false) {
            if (!string.IsNullOrEmpty(panelNo)) {
                // 这个从数据库取似更合理。                
                var data = LableCode.QueryLabelcodeByPanelNo(panelNo);

                if (data == null) {
                    msg = "!板号完成失败，未能查到数据库的标签。";
                    return false;
                }

                var erpParam = new Dictionary<string, string>() {
                        { "Board_No", panelNo },  // first item.
                        { "AllBarCode", string.Join(",", data.ToArray()) } // second item.
                    };
                var re = erpapi.Post(clsSetting.PanelFinish, erpParam);

                // show result.
                if (re["ERPState"] == "OK") {
                    if (re["State"] == "Fail") {
                        msg = string.Format("!{0}板号{1}完成失败。{2}", (handwork ? "手工" : "自动"),
                            JsonConvert.SerializeObject(erpParam), re["ERR"]);
                    } else {
                        msg = string.Format("{0}板号{1}完成成功。{2}", (handwork ? "手工" : "自动"),
                            JsonConvert.SerializeObject(erpParam), re["Data"]);
                        return true;
                    }
                } else {
                    FrmMain.ERPAlarm(FrmMain.opcNone, FrmMain.opcParam, ERPAlarmNo.COMMUNICATION_ERROR);
                }
            }
            msg = "!板号完成失败，板号为空。";
            return false;
        }
    }

    #endregion


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

    class PanelFullState {
        public int state { get; set; }
        public LableCode fromCache { get; set; }

        /// <summary>
        /// 未满
        /// </summary>
        public const int NO_FULL = 0;

        /// <summary>
        /// 已满
        /// </summary>
        public const int FULL = 1;

        /// <summary>
        /// 超出
        /// </summary>
        public const int EXCEED = 2;

        public PanelFullState(int state_, LableCode fromCache_) {
            state = state_;
            fromCache = fromCache_;
        }
    }
}
