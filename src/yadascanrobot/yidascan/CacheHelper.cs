using System;
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

        public string brief() {
            if (labelcode == null) {
                return $"{id.ToString().PadLeft(2, ' ')}";
            } else {
                return $"{id.ToString().PadLeft(2, ' ')} {labelcode.LCode} {labelcode.ToLocation} {labelcode.Diameter}";
            }
        }
    }

    [Serializable]
    public class CacheResult {
        public int savepos { get; set; }
        public int getpos { get; set; }
        public CacheState state { get; set; }

        public CacheResult(CacheState state, int getpos, int savepos) {
            this.state = state;
            this.savepos = savepos;
            this.getpos = getpos;
        }
    }

    public class CacheHelper {
        public object LOCK_CACHE_AREA = new object();

        public CachePos[] cacheposes { get; set; }

        public CacheHelper(CachePos[] _cacheposes) {
            cacheposes = _cacheposes;
        }

        /// <summary>
        /// 全部缓存位复位。
        /// </summary>
        public void Reset() {
            foreach (var p in cacheposes) {
                p.Reset();
            }
        }

        public LableCode getOutLabel(CacheResult cr) {
            if (cr.state == CacheState.GetThenCache
                || cr.state == CacheState.GetThenGo
                || cr.state == CacheState.GoThenGet) {
                return cacheposes[cr.getpos - 1].labelcode;
            } else {
                return null;
            }
        }

        /// <summary>
        /// 选最近的位置。
        /// </summary>
        /// <param name="lst"></param>
        /// <returns></returns>
        public static int findNearestPos(IList<CachePos> lst) {
            if (lst.Count() == 0) {
                throw new Exception("IList<CachePos> side = null, 空lst");
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

                if (smallside.Count() != 0 && bigside.Count() == 0) {
                    side = smallside.ToList();
                } else if (smallside.Count() == 0 && bigside.Count() != 0) {
                    side = bigside.ToList();
                } else { // 两侧都有空位。
                    if (awayfrom > 10) { // 避开长边
                        side = smallside.ToList();
                    } else { // 避开短边。
                        side = bigside.ToList();
                    }
                }

                return findNearestPos(side);
            }
        }

        public int getPosByCode(LableCode getcode) {
            var pp = from p in cacheposes
                     where p.labelcode != null && p.labelcode.LCode == getcode.LCode
                     select p;
            if (pp.Count() == 1) {
                var id = pp.First().id;
                cacheposes[id - 1].labelcode = null;
                return id;
            } else {
                throw new Exception($"取缓存位异常: getcode: {getcode.LCode} count: {pp.Count()}");
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
        /// 判断存取号码是否在同一侧
        /// </summary>
        /// <param name="pos1">缓存位1</param>
        /// <param name="pos2">缓存位2</param>
        /// <returns></returns>
        public static bool isInSameCacheChannel(int pos1, int pos2) {
            var ch1 = pos1 <= 10 && pos2 <= 10;
            var ch2 = pos1 >= 11 && pos2 >= 11;
            return ch1 || ch2;
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
                case CacheState.CacheAndGet:
                    posToGet = getPosByCode(getCode);
                    posToSave = SelectNearestNullPos(saveCode.ToLocation, posToGet); ;
                    result = new CacheResult(state, posToGet, posToSave);
                    break;
            }

            return result;
        }

        /// <summary>
        /// 重新计算缓存区布卷坐标。
        /// 其他队列的布卷坐标不动。
        /// 原板的最高层设为当前层。
        /// </summary>
        /// <param name="tolocation">交地</param>
        /// <param name="panelno">板号</param>
        public bool ReCalculateCoordinate(string panelno, string tolocation) {
            // 设新的板号，层数设为1
            var que = from x in cacheposes
                      where x.labelcode.ToLocation == tolocation
                      select x.labelcode.LCode;

            if (que.Count() == 0) return true;

            return LableCode.Update(panelno, tolocation, que.ToList());
        }

    }
}
