using Newtonsoft.Json;
using ProduceComm;
using System;
using System.Collections.Generic;
using System.Linq;
using yidascan.DataAccess;

namespace yidascan {

    public class LableCodeBll {
        public LableCode CalculateCache(PanelInfo pinfo, LableCode lc, List<LableCode> lcs) {
            LableCode lc2 = null;
            int cachecount = (pinfo.OddStatus ? 0 : 1) + (pinfo.EvenStatus ? 0 : 1);
            var tmp = from s in lcs where s.FloorIndex == 0 orderby s.Diameter ascending select s;
            switch (tmp.Count()) {
                case 0://当前层已没有了缓存。//当前布卷直接缓存起来。
                    break;
                case 1://当前层只有一卷缓存。
                case 2://当前层有两卷缓存。
                    if (tmp.Count() == cachecount)//缓存卷数与需要缓存卷数相等
                    {
                        List<LableCode> lcObjs = new List<LableCode>();
                        foreach (LableCode l in tmp) {
                            if (l.Diameter + clsSetting.CacheIgnoredDiff < lc.Diameter) { lcObjs.Add(l); }
                        }
                        if (lcObjs.Count > 0) {
                            lc2 = lcObjs[0];
                            lc.GetOutLCode = lc2.LCode;//换掉的标签
                            CalculatePosition(lcs, lc2);//当前布卷直接缓存起来。缓存的两卷中小的拿出来并计算位置。
                        } else {
                            CalculatePosition(lcs, lc);//当前布卷不需要缓存，计算位置。
                        }
                    }
                    break;
            }
            return lc2;
        }

        /// <summary>
        /// 2期缓存计算办法
        /// </summary>
        /// <param name="pinfo"></param>
        /// <param name="lc"></param>
        /// <param name="lcs"></param>
        /// <param name="cacheq">缓存队列</param>
        /// <returns>返回的是需要从缓存位取出的布卷。如果不需要取出，返回null。</returns>
        public LableCode CalculateCachePro(PanelInfo pinfo, LableCode lc, List<LableCode> lcs, Queue<LableCode> cacheq) {
            LableCode lc2 = null;
            var cachecount = (pinfo.OddStatus ? 0 : 1) + (pinfo.EvenStatus ? 0 : 1);
            var cachedRools = from s in lcs
                      where s.FloorIndex == 0
                      orderby s.Diameter ascending
                      select s;
            switch (cachedRools.Count()) {
                case 0://当前层已没有了缓存。//当前布卷直接缓存起来。
                    break;
                case 1://当前层只有一卷缓存。
                case 2://当前层有两卷缓存。
                    if (cachedRools.Count() == cachecount)//缓存卷数与需要缓存卷数相等
                    {
                        var lcObjs = new List<LableCode>();
                        foreach (LableCode l in cachedRools) {
                            // 缓存的比当前的直径大
                            var cachedBiggerThanCurrent = (l.Diameter + clsSetting.CacheIgnoredDiff < lc.Diameter);
                            if (cachedBiggerThanCurrent && NoMoreBiggerRoolsInCacheQ(lc, cacheq)) {
                                lcObjs.Add(l);
                            }
                        }
                        if (lcObjs.Count > 0) {
                            lc2 = lcObjs[0];
                            lc.GetOutLCode = lc2.LCode;//换掉的标签
                            CalculatePosition(lcs, lc2);//当前布卷直接缓存起来。缓存的两卷中小的拿出来并计算位置。
                        } else {
                            CalculatePosition(lcs, lc);//当前布卷不需要缓存，计算位置。
                        }
                    }
                    break;
            }
            return lc2;
        }

        private static bool NoMoreBiggerRoolsInCacheQ(LableCode lc, Queue<LableCode> cacheq) {
            // 同一个板上直径比当前大的。
            var q = cacheq.Count((x) => {
                return x.PanelNo == lc.PanelNo && x.Diameter - lc.Diameter > clsSetting.CacheIgnoredDiff;
            });
            return q > 0;
        }

        private bool IsRollInSameSide(LableCode lc, int flindex) {
            return lc.FloorIndex > 0 && lc.FloorIndex % 2 == flindex % 2;
        }

        public void CalculatePosition(List<LableCode> lcs, LableCode lc) {
            lc.FloorIndex = CalculateFloorIndex(lcs);

            decimal z = lc.Floor == 1 ? -35 : LableCode.GetFloorMaxDiameter(lc.PanelNo, lc.Floor) - 10;
            decimal r = clsSetting.OddTurn ?
                (lc.Floor % 2 == 1 ? 0 : 90) : //奇数层横放
                (lc.Floor % 2 == 1 ? 90 : 0); //偶数层横放

            decimal xory = CalculateXory(lcs, lc);

            //z,r,x/y
            lc.Coordinates = string.Format("{0},{1},{2}", z, r, xory);
            lc.Cx = r == 0 ? 0 : xory;
            lc.Cy = r == 0 ? xory : 0;
            lc.Cz = z;
            lc.Crz = r;
        }

        private decimal CalculateXory(List<LableCode> lcs, LableCode lc) {
            decimal xory;
            if (lc.FloorIndex <= 2) {
                xory = lc.FloorIndex % 2 == 1 ? 0 : -clsSetting.RollSep;
            } else {
                var lastRoll = (from s in lcs where IsRollInSameSide(s, lc.FloorIndex)
                                orderby s.FloorIndex descending select s).First();
                xory = (Math.Abs(lastRoll.Cx + lastRoll.Cy) + lastRoll.Diameter + clsSetting.RollSep)
                    * (lc.FloorIndex % 2 == 1 ? 1 : -1);
            }

            return xory;
        }

        private decimal CalculateXory(List<LableCode> lcs) {
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

        private int CalculateFloorIndex(List<LableCode> lcs) {
            var oddcount = lcs.Count(x => { return x.isOddSide(); });
            var evencount = lcs.Count(x => { return x.isEvenSide(); });

            if (oddcount == 0) { return 1; }
            if (evencount == 0) { return 2; }
            return oddcount > evencount ? 2 * evencount + 2 : 2 * oddcount + 1;
        }

        public void CalculatePosition(List<LableCode> lcs, LableCode lc, LableCode lc2) {
            lc.Floor = lc2.Floor;

            //if (lc.Diameter > lc2.Diameter) {
            //    LableCode tmp = lc2;
            //    lc2 = lc;
            //    lc = tmp;
            //}

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
        public LableCode IsPanelFull(List<LableCode> lcs, LableCode lc) {
            // LableCode result = null;

            //var MAX_LEN = clsSetting.SplintLength / 2;
            //if (lc.Floor > 1) {
            //    MAX_LEN = LableCode.GetFloorHalfAvgLength(lc.PanelNo, lc.Floor);
            //    MAX_LEN = MAX_LEN == 0 ? (clsSetting.SplintLength / 2) : MAX_LEN;
            //    FrmMain.logOpt.Write(string.Format("板号{0}层号{1}层的平均长度{2}", lc.PanelNo, lc.Floor - 1, MAX_LEN), LogType.BUFFER, LogViewType.OnlyFile);
            //}

            // var MAX_LEN = FindMaxWidth(lc);

            // var cached = (from s in lcs where s.FloorIndex == 0 select s).ToList();

            //var xory = CalculateXory(lcs);//计算要码上去布的Xory

            //foreach (LableCode item in cached) {
            //    // 当前卷的坐标。
            //    var itemXory = Math.Abs(xory) + lc.Diameter + clsSetting.RollSep;
            //    // 当前对应的边界余量。
            //    var tmp = MAX_LEN - (item.Diameter + Math.Abs(itemXory));

            //    // not full
            //    if (tmp > clsSetting.EdgeSpace) { continue; }

            //    if (result == null || (item.Diameter < result.Diameter)) {
            //        result = item; // 满
            //    }
            //}

            // 以下是新的做法。
            // 满则返回莫格缓存中的布卷
            // 不满则返回null
            var cachedRolls = (from s in lcs where s.FloorIndex == 0 select s).ToList();
            var MAX_WIDTH = FindMaxHalfWidth(lc);
            // 板上宽度
            var installedWidth = Math.Abs(CalculateXory(lcs));
            return findSmallerFromCachedRolls(cachedRolls, lc, installedWidth, MAX_WIDTH);
        }

        private static decimal FindMaxHalfWidth(LableCode lc) {
            var lenOfUpperFloor = lc.Floor > 1 
                ? LableCode.GetFloorHalfAvgLength(lc.PanelNo, lc.Floor)
                : 0;
            
           if (lenOfUpperFloor > 0) {
                return lenOfUpperFloor;
            } else {
                // 默认最大宽度
                return (clsSetting.SplintLength / 2);
            }
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
            return installedWidth + l1.Diameter + clsSetting.RollSep + l2.Diameter + clsSetting.EdgeSpace;
        }

        /// <summary>
        /// 从缓存布卷中，小于当前直径的布卷中，取最小的那个。
        /// </summary>
        /// <param name="cachedRolls"></param>
        /// <param name="current"></param>
        /// <param name="installedWidth">todo: describe installedWidth parameter on findSmallerFromCachedRolls</param>
        /// <param name="maxedWidth">todo: describe maxedWidth parameter on findSmallerFromCachedRolls</param>
        /// <returns></returns>
        private static LableCode findSmallerFromCachedRolls(List<LableCode> cachedRolls, LableCode current, decimal installedWidth, decimal maxedWidth) {
            var p = from x in cachedRolls
                    where (x.Diameter < current.Diameter) && (expectedWidth(installedWidth, current, x) > maxedWidth)
                    orderby x.Diameter ascending
                    select x;
            return p.FirstOrDefault();
        }

        public static PanelInfo GetPanelNo(LableCode lc, string dateShiftNo) {
            var pf = LableCode.GetTolactionCurrPanelNo(lc.ToLocation, dateShiftNo);
            //if (pf == null) {
            //    var panelNo = PanelGen.NewPanelNo();
            //    lc.PanelNo = panelNo;
            //    lc.FloorIndex = 0;
            //    lc.Floor = 1;
            //    lc.Coordinates = "";
            //} else {
            //    lc.SetupPanelInfo(pf);
            //}
            lc.SetupPanelInfo(pf);
            return pf;
        }

        public CacheState AreaBCalculate(IErpApi erpapi, LableCode lc, string dateShiftNo, out LableCode outCacheLable, out string msg) {
            var cState = CacheState.Error;
            outCacheLable = null;
            msg = string.Empty;

            var pinfo = GetPanelNo(lc, dateShiftNo);

            if (pinfo == null) {
                // 产生新板号赋予当前标签。
                //板第一卷
                LableCode.Update(lc);
                cState = CacheState.Cache;
                msg = string.Format(@"交地:{0};当前标签:{1};直径:{2};长:{3};缓存状态:{4};取出标签:{5};直径:{6};长:{7};",
                       lc.ToLocation, lc.LCode, lc.Diameter, lc.Length, cState, outCacheLable, 0, 0);
            } else {
                LableCode lc2 = null;
                var fp = FloorPerformance.None;

                // 取当前交地、当前板、当前层所有标签。
                var lcs = LableCode.GetLableCodesOfRecentFloor(lc.ToLocation, pinfo);

                if (lcs != null && lcs.Count > 0) {
                    // 最近一层没满。
                    lc2 = IsPanelFull(lcs, lc);

                    if (lc2 != null)//不为NULL，表示满
                    {
                        CalculatePosition(lcs, lc, lc2);//计算位置坐标

                        if (lc.FloorIndex % 2 == 0) {
                            pinfo.EvenStatus = true;
                            fp = FloorPerformance.EvenFinish;
                        } else {
                            pinfo.OddStatus = true;
                            fp = FloorPerformance.OddFinish;
                        }
                    } else {
                        lc2 = CalculateCache(pinfo, lc, lcs);//计算缓存，lc2不为NULL需要缓存
                    }
                }

                if (pinfo.EvenStatus && pinfo.OddStatus)
                    fp = FloorPerformance.BothFinish;

                if (lc2 != null) {
                    if (LableCode.Update(fp, pinfo, lc, lc2))
                        outCacheLable = lc2;
                    cState = lc.FloorIndex == 0 ? CacheState.GetThenCache : CacheState.GoThenGet;
                } else {
                    if (LableCode.Update(fp, pinfo, lc))
                        cState = lc.FloorIndex == 0 ? CacheState.Cache : CacheState.Go;
                }

                if (fp == FloorPerformance.BothFinish && lc.Floor == pinfo.MaxFloor) {
                    bool re = NotifyPanelEnd(erpapi, lc.PanelNo, out msg);
                    FrmMain.logOpt.Write(string.Format("{0} {1}", lc.ToLocation, msg), LogType.NORMAL);
                }
                msg = string.Format(@"交地:{0};当前标签:{1};直径:{2};长:{3};缓存状态:{4};取出标签:{5};直径:{6};长:{7};",
                       lc.ToLocation, lc.LCode, lc.Diameter, lc.Length, cState, outCacheLable == null ? "" : outCacheLable.LCode,
                       (lc2 == null ? 0 : lc2.Diameter),//outCacheLable
                       (lc2 == null ? 0 : lc2.Length));//outCacheLable
            }
            return cState;
        }

        /// <summary>
        /// 计算指定号码需要采取的缓存区动作。
        /// </summary>
        /// <param name="erpapi"></param>
        /// <param name="lc"></param>
        /// <param name="dateShiftNo"></param>
        /// <param name="outCacheLable"></param>
        /// <param name="msg"></param>
        /// <param name="cacheq">缓存队列</param>
        /// <returns></returns>
        public CalResult AreaBCalculatePro(IErpApi erpapi, LableCode lc, string dateShiftNo, Queue<LableCode> cacheq) {
            var rt = new CalResult(CacheState.Error, lc, null);

            var pinfo = GetPanelNo(lc, dateShiftNo);

            if (pinfo == null) {
                // 产生新板号赋予当前标签。
                //板第一卷
                LableCode.Update(lc);
                rt.state = CacheState.Cache;
            } else {
                var fp = FloorPerformance.None;

                // 取当前交地、当前板、当前层所有标签。
                var layerLabels = LableCode.GetLableCodesOfRecentFloor(lc.ToLocation, pinfo);

                LableCode lc2 = null;
                if (layerLabels != null && layerLabels.Count > 0) {
                    // 最近一层没满。
                    lc2 = IsPanelFull(layerLabels, lc);

                    if (lc2 != null) //不为NULL，表示满
                    {
                        //计算位置坐标, 赋予层号
                        CalculatePosition(layerLabels, lc, lc2);

                        if (lc.FloorIndex % 2 == 0) {
                            pinfo.EvenStatus = true;
                            fp = FloorPerformance.EvenFinish;
                        } else {
                            pinfo.OddStatus = true;
                            fp = FloorPerformance.OddFinish;
                        }
                    } else {
                        //计算缓存，lc2不为NULL需要缓存
                        lc2 = CalculateCachePro(pinfo, lc, layerLabels, cacheq);
                    }
                }

                if (pinfo.EvenStatus && pinfo.OddStatus)
                    fp = FloorPerformance.BothFinish;

                if (lc2 != null) {
                    if (LableCode.Update(fp, pinfo, lc, lc2)) {
                        rt.CodeFromCache = lc2;
                    }
                    rt.state = lc.FloorIndex == 0
                        ? CacheState.GetThenCache
                        : CacheState.GoThenGet;
                } else {
                    if (LableCode.Update(fp, pinfo, lc))
                        rt.state = lc.FloorIndex == 0 
                            ? CacheState.Cache 
                            : CacheState.Go;
                }

                var msg = "";
                if (fp == FloorPerformance.BothFinish && lc.Floor == pinfo.MaxFloor) {
                    var re = NotifyPanelEnd(erpapi, lc.PanelNo, out msg);
                    rt.message = msg;
                }
            }
            return rt;
        }

        // AreaBCalculate调用之后， 用此函数显示缓存操作状态。
        public string ShowCacheOperationInfo(LableCode in_, LableCode out_, CacheState state) {
            return string.Format(@"缓存状态:{0},交地:{1},当前标签:{2},直径:{3},长:{4},取出标签:{5},直径:{6},长:{7};",
                state,
                in_.ToLocation,
                in_.LCode, in_.Diameter, in_.Length,
                out_ == null ? "" : out_.LCode,
                (out_ == null ? 0 : out_.Diameter),
                (out_ == null ? 0 : out_.Length));
        }

        public bool NotifyPanelEnd(IErpApi erpapi, string panelNo, out string msg, bool handwork = false) {
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
                    FrmMain.ERPAlarm(FrmMain.opcClient, FrmMain.opcParam, ERPAlarmNo.COMMUNICATION_ERROR);
                }
            }
            msg = "!板号完成失败，板号为空。";
            return false;
        }
    }

    /// <summary>
    /// 用于传递缓存区号码比对处理的结果。
    /// </summary>
    public class CalResult {
        public CacheState state { get; set; }
        /// <summary>
        /// 缓存区来料标签
        /// </summary>
        public LableCode CodeToCache { get; set; }

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
            CodeToCache = codeToCache_;
            CodeFromCache = codeFromCache_;
            message = "";
        }
    }
}
