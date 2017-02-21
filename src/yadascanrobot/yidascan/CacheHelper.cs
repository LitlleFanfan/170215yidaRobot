﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yidascan.DataAccess;

namespace yidascan {
    public class CachePos {
        /// <summary>
        /// 位置编号。
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 布卷
        /// </summary>
        public LableCode labelcode { get; set; }

        public CachePos(int id, LableCode lable) {
            this.id = id;
            this.labelcode = lable;
        }

        public void Reset() {
            labelcode = null;
        }
    }

    [Serializable]
    public class CacheResult {
        public int savepos { get; set; }
        public int getpos { get; set; }
        public CacheState state { get; set; }

        public CacheResult(CacheState state, int savepos, int getpos) {
            this.state = state;
            this.savepos = savepos;
            this.getpos = getpos;
        }
    }

    public class CacheHelper {
        public CachePos[] cacheposes { get; set; }

        public CacheHelper() {
            int len = 20;
            cacheposes = new CachePos[len];
            for (int i = 0; i < len; i++) {
                cacheposes[i] = new CachePos(i + 1, null);
            }
        }

        /// <summary>
        /// 全部缓存位复位。
        /// </summary>
        public void Reset() {
            foreach (var p in cacheposes) {
                p.Reset();
            }
        }
        
        /// <summary>
        /// 选最近的位置。
        /// </summary>
        /// <param name="lst"></param>
        /// <returns></returns>
        public static int findNearestPos(IList<CachePos> lst) {
            if (lst.Count() == 0) {
                throw new Exception("空list");
            }

            var head = lst.First();
            if (head.id > 10) {
                var mean = 15.5;
                var dmin = (from x in lst select Math.Abs(x.id - mean)).Min();
                return lst.First(x => { return Math.Abs(x.id - mean) == dmin; }).id;
            } else {
                var mean = 5.5;
                var dmin = (from x in lst select Math.Abs(x.id - mean)).Min();
                return lst.First(x => { return Math.Abs(x.id - mean) == dmin; }).id;
            }
        }

        /// <summary>
        /// 选择最靠近轨道的缓存位。
        /// </summary>
        /// <param name="tolocation">交地</param>
        /// <param name="awayfrom">todo: describe awayfrom parameter on SelectNearestNullPos</param>
        /// <returns></returns>
        public int SelectNearestNullPos(string tolocation, int awayfrom) {
            // 号码按奇数和偶数分侧。
            // 按照交地区分位置的重要性???。

            var smallside = from p in cacheposes
                            where p.labelcode == null && p.id <= 10
                            select p;
            var bigside = from p in cacheposes
                          where p.labelcode == null && p.id > 10
                          select p;

            if (smallside.Count() == 0 && bigside.Count() == 0) {
                throw new Exception("缓存区没有空位。");
            }

            if (awayfrom == -1) {
                if (smallside.Count() == 0) {
                    return findNearestPos(bigside.ToList());
                }

                if (bigside.Count() == 0) {
                    return findNearestPos(smallside.ToList());
                }
                
                var p1 = findNearestPos(smallside.ToList());
                var p2 = findNearestPos(bigside.ToList());
                
                if (Math.Abs(p1 - 5.5) >= Math.Abs(p2 - 15.5)) {
                    // p2近
                    return p2;
                } else {
                    return p1;
                }
            } else {
                IList<CachePos> side = null;

                if ((smallside.Count() != 0 && awayfrom <= 10) || bigside.Count() == 0) {
                    side = smallside.ToList();
                } else if ((bigside.Count() != 0 && awayfrom > 10) || smallside.Count() == 0) {
                    side = bigside.ToList();
                }

                return findNearestPos(side);
            }            
        }

        public int getPosByCode(LableCode getcode) {
            var pp = from p in cacheposes
                     where p.labelcode != null && p.labelcode.LCode == getcode.LCode
                     select p;
            if (pp.Count() == 1) {
                return pp.First().id;
            } else {
                throw new Exception("取缓存位异常。");
            }
        }

        public void save(LableCode lablecode, int pos) {
            foreach (var p in cacheposes) {
                if (p.id == pos) {
                    p.labelcode = lablecode;
                    return;
                }
            }
        }

        /// <summary>
        /// 主要函数。
        /// </summary>
        /// <param name="state"></param>
        /// <param name="saveCode"></param>
        /// <param name="getCode"></param>
        /// <returns></returns>
        public CacheResult WhenRollArrived(CacheState state, LableCode saveCode, LableCode getCode) {
            CacheResult result = null;
            const int NULL_POS = 0;
            var posToGet = 0;
            var posToSave = 0;

            switch (state) {
                case CacheState.Go:
                    result = new CacheResult(state, NULL_POS, NULL_POS);
                    break;
                case CacheState.Cache:
                    posToSave = SelectNearestNullPos(saveCode.ToLocation, -1);
                    save(saveCode, posToSave);
                    result = new CacheResult(state, NULL_POS, posToSave);
                    break;
                case CacheState.GetThenCache:
                    // 考虑错开机械手。
                    posToGet = getPosByCode(getCode);
                    posToSave = SelectNearestNullPos(saveCode.ToLocation, posToGet);
                    save(saveCode, posToSave);
                    result = new CacheResult(state, posToGet, posToSave);
                    break;
                case CacheState.GetThenGo:
                    posToGet = getPosByCode(getCode);
                    result = new CacheResult(state, posToGet, NULL_POS);
                    break;
                case CacheState.GoThenGet:
                    posToGet = getPosByCode(getCode);
                    result = new CacheResult(state, posToGet, NULL_POS);
                    break;
            }

            return result;
        }
    }
}
