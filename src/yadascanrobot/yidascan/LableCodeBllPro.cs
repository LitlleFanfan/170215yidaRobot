﻿using System;
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

        private static void CalculatePosition(List<LableCode> lcs, LableCode lc, int state) {
            if (lc == null) {
                return;
                // throw new Exception($"nameof(CalculatePosition), lc is null.");
            }
            lc.FloorIndex = CalculateFloorIndex(lcs);
            OnlinCalculatePosition(lcs, lc, state);
        }

        private static void OnlinCalculatePosition(List<LableCode> lcs, LableCode lc, int state) {
            if (lc == null) {
                throw new Exception($"{nameof(OnlinCalculatePosition)}, lc is null.");
            }

            var z = lc.Floor == 1 ? clsSetting.InitHeigh : LableCode.GetFloorMaxDiameter(lc.PanelNo, lc.Floor) - (lc.Floor * 5);
            var r = clsSetting.OddTurn ?
                (lc.Floor % 2 == 1 ? 0 : 90) : //奇数层横放
                (lc.Floor % 2 == 1 ? 90 : 0); //偶数层横放

            var xory = CalculateXory(lcs, lc);
            xory = OffsetSideLastRollXory(state, lc, xory);

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

        private static void CalculatePosition(List<LableCode> lcs, LableCode lc, LableCode lc2, int state) {
            CalculatePosition(lcs, lc, SideFullState.NO_FULL);
            lc2.FloorIndex = lc.FloorIndex + 2;

            var d = Math.Abs(lc.Cx + lc.Cy) + lc.Diameter + clsSetting.RollSep;
            var xory = d * (lc.FloorIndex % 2 == 1 ? 1 : -1);
            xory = OffsetSideLastRollXory(state, lc2, xory);

            lc2.Coordinates = string.Format("{0},{1},{2}", lc.Cz, lc.Crz, xory);

            lc2.Cx = lc.Crz == 0 ? 0 : xory;
            lc2.Cy = lc.Crz == 0 ? xory : 0;
            lc2.Cz = lc.Cz;
            lc2.Crz = lc.Crz;
        }

        private static decimal OffsetSideLastRollXory(int state, LableCode lc, decimal xory) {
            if (state == SideFullState.NO_FULL || lc.Floor == 1) {//未满不须要靠边放。
                return xory;
            }
            var maxwidth = FindMaxHalfWidth(lc);
            var newxory = (maxwidth - lc.Diameter - 50) * (lc.FloorIndex % 2 == 1 ? 1 : -1);

            if (Math.Abs(xory) > Math.Abs(newxory)) {
                return xory;
            } else {
                lc.Remark = $"{lc.Remark} offsetSLR[{(int)(Math.Abs(newxory) - Math.Abs(xory))}mm]";
                return newxory;
            }
        }

        private static SideFullState IsPanelFullPro(List<LableCode> lcs, LableCode lc) {
            // 以下是新的做法。
            // 满则返回莫格缓存中的布卷
            // 不满则返回null
            var MAX_WIDTH = FindMaxHalfWidth(lc);
            // 板上宽度
            var installedWidth = Math.Abs(CalculateXory(lcs));
            return findSmallerFromCachedRollsPro(lc, installedWidth, MAX_WIDTH);
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
        private static CalResult CalculateCache(PanelInfo pinfo, LableCode lc) {
            var cr = new CalResult(CacheState.Go, lc, null);
            var cachecount = (pinfo.OddStatus ? 0 : 1) + (pinfo.EvenStatus ? 0 : 1);

            List<LableCode> cachedRolls;
            lock (TaskQueues.LOCK_LOCHELPER) {
                cachedRolls = FrmMain.taskQ.CacheSide.Where(x => x.labelcode != null && x.labelcode.ToLocation == lc.ToLocation)
                    .Select(x => x.labelcode)
                    .ToList();
            }

            switch (cachedRolls.Count()) {
                case 0://当前层已没有了缓存。//当前布卷直接缓存起来。
                    cr.state = CacheState.Cache;
                    break;
                case 1://当前层只有一卷缓存。
                case 2://当前层有两卷缓存。
                    if (cachedRolls.Count() == cachecount)//缓存卷数与需要缓存卷数相等
                    {
                        var smaller = cachedRolls.Where(x => x.Diameter + clsSetting.CacheIgnoredDiff < lc.Diameter)
                            .OrderBy(x => x.Diameter)
                            .FirstOrDefault();
                        if (smaller != null) {
                            cr.CodeFromCache = smaller;
                            lc.GetOutLCode = cr.CodeFromCache.LCode;
                            cr.state = CacheState.GetThenCache;
                        } else {
                            cr.state = CacheState.Go;
                        }
                    } else if (cachedRolls.Count() < cachecount) {
                        cr.state = CacheState.Cache;
                    } else {
                        cr.state = CacheState.Go;
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
            var edgespace = GetEdgeSpace(FrmMain.taskQ.CacheSide, l1.ToLocation, clsSetting.EdgeSpace);
            var expected = installedWidth + l1.Diameter + clsSetting.RollSep + l2.Diameter + edgespace;
            return expected;
        }

        // 
        private static decimal expectedWidthNoEdgeSpace(decimal installedWidth, LableCode l1, LableCode l2) {
            // 预期宽度
            // l2可能是空值
            if (l1 == null) {
                throw new Exception($"{nameof(expectedWidthNoEdgeSpace)}, lablecode l1 is null");
            }

            var cachedRollDiameter = l2 != null ? l2.Diameter : 0;
            var expected = installedWidth + l1.Diameter + clsSetting.RollSep + cachedRollDiameter;
            return expected;
        }

        /// <summary>
        /// 当前布卷和缓存中的一个布卷可能占用的坐标
        /// </summary>
        /// <param name="l1"></param>
        /// <returns></returns>
        private static decimal expectedWidth(LableCode l1) {
            // 预期宽度
            var edgespace = GetEdgeSpace(FrmMain.taskQ.CacheSide, l1.ToLocation, clsSetting.EdgeSpace);
            var expected = Math.Abs(l1.Cx + l1.Cy) + edgespace;
            return expected;
        }

        private static SideFullState findSmallerFromCachedRollsPro(LableCode current, decimal installedWidth, decimal max) {
            // 预计宽度超出
            var e = GetEdgeSpace(FrmMain.taskQ.CacheSide, current.ToLocation, clsSetting.EdgeSpace);

            var cachedRolls = FrmMain.taskQ.CacheSide.Where(x => x.labelcode != null && x.labelcode.ToLocation == current.ToLocation)
                .Select(x => x.labelcode)
                .ToList();

            // 缓存位中，会导致满板或超出的布卷，取第一个。
            var rt = cachedRolls.Where(x => (expectedWidthNoEdgeSpace(installedWidth, current, x) > max - e))
                .OrderBy(x => x.Diameter)
                .FirstOrDefault();

            // 缓存位的floor有时候不对。floor这个值是哪里算的？
            if (rt != null && rt.Floor != current.Floor) {
                rt.Remark = $"{rt.Remark}@{rt.Floor}";
                rt.Floor = current.Floor;
            }

            var w = expectedWidthNoEdgeSpace(installedWidth, current, rt);
            if (w >= max + clsSetting.WidthFix) {
                return new SideFullState(SideFullState.EXCEED, rt);
            } else if (w > max - e) {
                return new SideFullState(SideFullState.FULL, rt);
            } else {
                return new SideFullState(SideFullState.NO_FULL, rt);
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

        private static FloorPerformance SetFullFlag(int edgeFloorIndex, PanelInfo pinfo) {
            FloorPerformance fp;
            if (edgeFloorIndex % 2 == 0) {
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

        /// <summary>
        /// 取某板最佳边缘预留
        /// </summary>
        /// <param name="tolocation">交地</param>
        /// <param name="cachq">缓存位队列</param>
        /// <param name="defaultEdgeSpace">默认的边缘预留</param>
        /// <returns></returns>
        private static decimal GetEdgeSpace(IEnumerable<CachePos> cachq, string tolocation, decimal defaultEdgeSpace) {
            lock (cachq) {
                //var tmp = cachq.Where(x => x.labelcode != null && x.labelcode.ToLocation == tolocation);
                decimal maxd = 0;
                //if (tmp != null && tmp.Count() > 0) {
                //    maxd = tmp.Max(x => x.labelcode.Diameter);
                //}
                return Math.Max(maxd, defaultEdgeSpace);
            }
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

        private static bool isTooSmall(decimal dia) {
            const int DIAMETER_THRESHOLD = 100;
            return dia < DIAMETER_THRESHOLD;
        }

        public static CalResult AreaBCalculate(LableCode lc, string dateShiftNo, IEnumerable<LableCode> cacheq, Action<string> onlog) {
            var cre = new CacheResult {
                CResult = new CalResult(CacheState.Cache, lc, null)
            };

            // 创建新板号
            var pinfo = GetPanelNo(cre.CResult.CodeCome, dateShiftNo);

            var fp = FloorPerformance.None;
            List<LableCode> layerLabels;

            // 解决在缓存位，同一交地出现多个不同板号的情况.
            if (pinfo == null) {
                pinfo = new PanelInfo(cre.CResult.CodeCome.PanelNo);
                cre.CResult.CodeCome.PanelNo = pinfo.PanelNo;
                cre.CResult.CodeCome.Floor = pinfo.CurrFloor;
            }
            #region 缓存布卷板号和层号的修正
            var cachedRolls = FrmMain.taskQ.CacheSide.Where(x => x.labelcode != null && x.labelcode.ToLocation == lc.ToLocation)
                .Select(x => x.labelcode)
                .ToList();
            foreach (LableCode tmp in cachedRolls) {
                tmp.PanelNo = pinfo.PanelNo;
                tmp.Floor = pinfo.CurrFloor;
            }
            #endregion

            // 取当前交地、当前板、当前层所有标签。
            layerLabels = LableCode.GetLableCodesOfRecentFloor(cre.CResult.CodeCome.ToLocation, pinfo.PanelNo, pinfo.CurrFloor);
            cre = CalculateCacheState(cre.CResult, pinfo, layerLabels, onlog);

            if (cre.SideState.state == SideFullState.FULL || cre.SideState.state == SideFullState.EXCEED) {
                fp = SetFullFlag(CalculateFloorIndex(layerLabels), pinfo);
            }

            var msg = $"";
            switch (cre.CResult.state) {
                case CacheState.Go:
                    CalculatePosition(layerLabels, cre.CResult.CodeCome, cre.SideState == null ? SideFullState.NO_FULL : cre.SideState.state);
                    break;
                case CacheState.GetThenCache:
                case CacheState.CacheAndGet:
                    CalculatePosition(layerLabels, cre.CResult.CodeFromCache, cre.SideState == null ? SideFullState.NO_FULL : cre.SideState.state);
                    break;
                case CacheState.GoThenGet:
                    //计算位置坐标, 赋予层号
                    cre.CResult.CodeCome.Floor = cre.CResult.CodeFromCache.Floor;
                    CalculatePosition(layerLabels, cre.CResult.CodeCome, cre.CResult.CodeFromCache, cre.SideState == null ? SideFullState.NO_FULL : cre.SideState.state);
                    break;
                case CacheState.GetThenGo:
                    cre.CResult.CodeCome.Floor = cre.CResult.CodeFromCache.Floor;
                    CalculatePosition(layerLabels, cre.CResult.CodeFromCache, cre.CResult.CodeCome, cre.SideState == null ? SideFullState.NO_FULL : cre.SideState.state);
                    break;
                case CacheState.Cache:
                    var cancachesum = (pinfo.OddStatus ? 0 : 1) + (pinfo.EvenStatus ? 0 : 1);
                    var cachelcs = FrmMain.taskQ.CacheSide.Count(x => x.labelcode != null && x.labelcode.ToLocation == lc.ToLocation);

                    var go = CanIgo(cacheq, cre.CResult, cancachesum - cachelcs);
                    if (go) {
                        cre.CResult.state = CacheState.Go;
                        bool isfull = IsPanelFull(cre.CResult.CodeCome);
                        CalculatePosition(layerLabels, cre.CResult.CodeCome, isfull ? SideFullState.FULL : SideFullState.NO_FULL);

                        if (isfull) {
                            fp = SetFullFlag(cre.CResult.CodeCome.FloorIndex, pinfo);
                        }

                        msg = $"cache change go";
                    }
                    break;
                default:
                    break;
            }

            // 记录层最后一卷
            if (fp == FloorPerformance.BothFinish) {
                if (cre.CResult.state == CacheState.Go) {
                    // 在没有缓存的情况下满板。
                    cre.CResult.CodeCome.Status = (int)LableState.FloorLastRoll;
                    cre.CResult.CodeCome.Remark = $"{cre.CResult.CodeCome.Remark} floor last roll";
                } else if (cre.CResult.state == CacheState.GetThenGo) {
                    cre.CResult.CodeCome.Status = (int)LableState.FloorLastRoll;
                    cre.CResult.CodeCome.Remark = $"{cre.CResult.CodeCome.Remark} floor last roll";
                } else if (cre.SideState.state == SideFullState.EXCEED) {
                    if (cre.CResult.CodeFromCache != null) {
                        cre.CResult.CodeFromCache.Status = (int)LableState.FloorLastRoll;
                        cre.CResult.CodeFromCache.Remark = $"{cre.CResult.CodeFromCache.Remark} floor last roll";
                    } else {
                        cre.CResult.CodeCome.Status = (int)LableState.FloorLastRoll;
                        cre.CResult.CodeCome.Remark = $"{cre.CResult.CodeCome.Remark} floor last roll*{cre.CResult.state}";
                    }
                } else {
                    cre.CResult.CodeFromCache.Status = (int)LableState.FloorLastRoll;
                    cre.CResult.CodeFromCache.Remark = $"{cre.CResult.CodeFromCache.Remark} floor last roll";
                }
            }

            if (!PanelGen.HasPanelNo(pinfo.PanelNo)) {
                LableCode.Update(cre.CResult.CodeCome);
            }

            if (cre.SideState.state == SideFullState.EXCEED) {
                msg = $"{msg} EXCEED";
                pinfo.HasExceed = true;
            }

            if (cre.CResult.CodeFromCache != null) {
                if (fp == FloorPerformance.BothFinish && cre.SideState.state == SideFullState.EXCEED) {//层满上超出处理
                    LableCode.UpdateEdgeExceed(fp, pinfo, cre.CResult.CodeCome, cre.CResult.CodeFromCache);
                } else {
                    LableCode.Update(fp, pinfo, cre.CResult.CodeCome, cre.CResult.CodeFromCache);
                }
            } else {
                LableCode.Update(fp, pinfo, cre.CResult.CodeCome);
            }

            cre.CResult.message = msg;
            return cre.CResult;
        }

        private static CacheResult CalculateCacheState(CalResult rt, PanelInfo pinfo, List<LableCode> layerLabels, Action<string> onlog) {
            // debug 2017.97.24
            if (rt == null) { throw new Exception("rt空值。"); }
            if (rt.CodeCome == null) { throw new Exception("rt.codecome空值。"); }

            var cre = new CacheResult {
                SideState = new SideFullState(SideFullState.NO_FULL, null)
            };
            if (layerLabels != null && layerLabels.Count > 0) {
                // 最近一层没满。
                cre.SideState = IsPanelFullPro(layerLabels, rt.CodeCome);
                rt.CodeFromCache = cre.SideState.fromCache;

                if (cre.SideState.state == SideFullState.FULL) {
                    // rt.CodeFromCache可能是null.
                    var cachedRollDiameter = rt.CodeFromCache != null ? rt.CodeFromCache.Diameter : 0;
                    if (rt.CodeFromCache == null) {
                        rt.state = CacheState.Go;
                    } else if (rt.CodeCome.Diameter > cachedRollDiameter + clsSetting.CacheIgnoredDiff) {
                        rt.state = CacheState.GetThenGo;
                    } else {
                        rt.state = CacheState.GoThenGet;
                    }
                } else if (cre.SideState.state == SideFullState.NO_FULL) {
                    var cr = CalculateCache(pinfo, rt.CodeCome);
                    rt.CodeFromCache = cr.CodeFromCache;
                    rt.state = cr.state;
                } else if (cre.SideState.state == SideFullState.EXCEED) {
                    if (rt.CodeFromCache == null) { onlog($"布卷exceed状态时，codefromcache空值。"); } // debug 2017.97.24
                    var cachedRollDiameter = rt.CodeFromCache != null ? rt.CodeFromCache.Diameter : 0;
                    if (rt.CodeCome.Diameter + clsSetting.CacheIgnoredDiff > cachedRollDiameter) {
                        rt.CodeFromCache = null;
                        rt.state = CacheState.Go;
                    } else {
                        rt.state = CacheState.GetThenCache;
                    }
                    pinfo.HasExceed = true;
                }
            }
            cre.CResult = rt;

            if (cre.CResult.CodeFromCache != null && cre.CResult.CodeCome.PanelNo != cre.CResult.CodeFromCache.PanelNo) {
                cre.CResult.CodeFromCache.PanelNo = cre.CResult.CodeCome.PanelNo;
                cre.CResult.CodeFromCache.Floor = cre.CResult.CodeCome.Floor;
            }

            if (cre.CResult.CodeFromCache != null && cre.CResult.CodeFromCache.Floor != cre.CResult.CodeCome.Floor) {
                cre.CResult.CodeFromCache.Floor = cre.CResult.CodeCome.Floor;
            }

            return cre;
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

    public class SideFullState {
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

        public SideFullState(int state_, LableCode fromCache_) {
            state = state_;
            fromCache = fromCache_;
        }
    }

    public class CacheResult {
        public SideFullState SideState;
        public CalResult CResult;
    }
}
