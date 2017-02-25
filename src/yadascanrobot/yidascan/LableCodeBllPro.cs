﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using ProduceComm;
using yidascan.DataAccess;

namespace yidascan.labelcodepro {
    class LableCodeBllPro {
        public LableCode CalculateCache(PanelInfo pinfo, LableCode lc, List<LableCode> lcs) {
            LableCode lc2 = null;
            var cachecount = (pinfo.OddStatus ? 0 : 1) + (pinfo.EvenStatus ? 0 : 1);
            var tmp = from s in lcs where s.FloorIndex == 0 orderby s.Diameter ascending select s;
            switch (tmp.Count()) {
                case 0://当前层已没有了缓存。//当前布卷直接缓存起来。
                    break;
                case 1://当前层只有一卷缓存。
                case 2://当前层有两卷缓存。
                    if (tmp.Count() == cachecount)//缓存卷数与需要缓存卷数相等
                    {
                        var lcObjs = new List<LableCode>();
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

        private static bool IsRollInSameSide(LableCode lc, int flindex) {
            return lc.FloorIndex > 0 && lc.FloorIndex % 2 == flindex % 2;
        }

        public void CalculatePosition(List<LableCode> lcs, LableCode lc) {
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

        private static int CalculateFloorIndex(List<LableCode> lcs) {
            var oddcount = lcs.Count(x => { return x.isOddSide(); });
            var evencount = lcs.Count(x => { return x.isEvenSide(); });

            if (oddcount == 0) { return 1; }
            if (evencount == 0) { return 2; }
            return oddcount > evencount ? 2 * evencount + 2 : 2 * oddcount + 1;
        }

        public void CalculatePosition(List<LableCode> lcs, LableCode lc, LableCode lc2) {
            lc.Floor = lc2.Floor;
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
            LableCode result = null;
            var MAX_LEN = clsSetting.SplintLength / 2;
            if (lc.Floor > 1) {
                MAX_LEN = LableCode.GetFloorHalfAvgLength(lc.PanelNo, lc.Floor);
                MAX_LEN = MAX_LEN == 0 ? (clsSetting.SplintLength / 2) : MAX_LEN;
                FrmMain.logOpt.Write(string.Format("板号{0}层号{1}层的平均长度{2}", lc.PanelNo, lc.Floor - 1, MAX_LEN), LogType.BUFFER, LogViewType.OnlyFile);
            }
            var cache = from s in lcs where s.FloorIndex == 0 select s;
            var xory = CalculateXory(lcs);//计算要码上去布的Xory

            foreach (LableCode item in cache) {
                // 当前卷的坐标。
                var itemXory = Math.Abs(xory) + lc.Diameter + clsSetting.RollSep;
                // 当前对应的边界余量。
                var tmp = MAX_LEN - (item.Diameter + Math.Abs(itemXory));

                if (tmp > clsSetting.EdgeSpace) { continue; }

                if (result == null || (item.Diameter < result.Diameter)) {
                    result = item;
                }
            }
            return result;
        }

        public static PanelInfo GetPanelNo(LableCode lc, string dateShiftNo) {
            var pf = LableCode.GetTolactionCurrPanelNo(lc.ToLocation, dateShiftNo);
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
                    var re = ErpHelper.NotifyPanelEnd(erpapi, lc.PanelNo, out msg);
                    if (re == false) {
                        // FrmMain.ERPAlarm(FakeOpcClient, ParamArrayAttribute, ERPAlarmNo.COMMUNICATION_ERROR);
                        FrmMain.logOpt.Write(msg);
                    }
                    FrmMain.logOpt.Write(string.Format("{0} {1}", lc.ToLocation, msg), LogType.NORMAL);
                }
                msg = string.Format(@"交地:{0};当前标签:{1};直径:{2};长:{3};缓存状态:{4};取出标签:{5};直径:{6};长:{7};",
                       lc.ToLocation, lc.LCode, lc.Diameter, lc.Length, cState, outCacheLable == null ? "" : outCacheLable.LCode,
                       (lc2 == null ? 0 : lc2.Diameter),//outCacheLable
                       (lc2 == null ? 0 : lc2.Length));//outCacheLable
            }
            return cState;
        }
    }
}