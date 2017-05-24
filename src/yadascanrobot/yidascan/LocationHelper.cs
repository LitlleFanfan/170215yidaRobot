using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Reflection;
using Newtonsoft.Json;

namespace yidascan {
    public enum LocationState {
        IDLE = 0,
        BUSY = 1,
        FULL = 2
    }

    public enum Priority {
        DISABLE = 0,
        LOW = 1,
        MEDIUM = 2,
        HIGH = 3
    }

    public class VirtualLoc {
        public string virtualloc;
        public Priority priority;

        public VirtualLoc(string loc, Priority p = Priority.MEDIUM) {
            this.virtualloc = loc;
            priority = p;
        }

        public static VirtualLoc Create(string loc, Priority p) {
            return new VirtualLoc(loc, p);
        }
    }

    public class RealLoc {
        public string realloc { get; set; }
        public LocationState state { get; set; }
        public Priority priority { get; set; }
        public string panelno { get; set; }

        public RealLoc(string loc, LocationState s, Priority p) {
            this.realloc = loc;
            this.state = s;
            this.priority = p;
        }

        public static RealLoc Create(string loc, LocationState s, Priority p) {
            return new RealLoc(loc, s, p);
        }
    }

    public class LocationHelper {
        // 虚拟板号，key是虚拟板号，value是真实板号。
        public Dictionary<string, string> LocMap;

        // key是真实板号，value是板状态。
        // 板满时，value是false，空时，value是true。
        public RealLoc[] RealLocations;

        public VirtualLoc[] VirtualLocations;

        public LocationHelper() {
            LocMap = new Dictionary<string, string> {
                ["B01"] = "",
                ["B02"] = "",
                ["B03"] = "",
                ["B04"] = "",
                ["B05"] = "",
                ["B06"] = "",
                ["B07"] = "",
                ["B08"] = "",
                ["B09"] = "",
                ["B10"] = ""
            };

            RealLocations = new RealLoc[] {
                RealLoc.Create("B01", LocationState.IDLE, Priority.MEDIUM),
                RealLoc.Create("B02", LocationState.IDLE, Priority.MEDIUM),
                RealLoc.Create("B03", LocationState.IDLE, Priority.DISABLE),
                RealLoc.Create("B04", LocationState.IDLE, Priority.HIGH),

                RealLoc.Create("B05", LocationState.IDLE, Priority.HIGH),
                RealLoc.Create("B06", LocationState.IDLE, Priority.DISABLE),
                RealLoc.Create("B07", LocationState.IDLE, Priority.DISABLE),
                RealLoc.Create("B08", LocationState.IDLE, Priority.DISABLE),
                RealLoc.Create("B09", LocationState.IDLE, Priority.DISABLE),
                RealLoc.Create("B10", LocationState.IDLE, Priority.DISABLE),
                RealLoc.Create("B11", LocationState.IDLE, Priority.DISABLE)
            };

            VirtualLocations = new VirtualLoc[] {
                VirtualLoc.Create("B01", Priority.MEDIUM),
                VirtualLoc.Create("B02", Priority.MEDIUM),
                VirtualLoc.Create("B03", Priority.MEDIUM),
                VirtualLoc.Create("B04", Priority.MEDIUM),
                VirtualLoc.Create("B05", Priority.MEDIUM),
                VirtualLoc.Create("B06", Priority.MEDIUM),
                VirtualLoc.Create("B07", Priority.MEDIUM),
                VirtualLoc.Create("B08", Priority.MEDIUM),
                VirtualLoc.Create("B09", Priority.MEDIUM),
                VirtualLoc.Create("B10", Priority.MEDIUM)
            };
        }

        #region private_func
        private void SetState(string realloc, LocationState state, string panelno) {
            if (!IsRealLocExists(realloc)) {
                throw new Exception($"函数: {nameof(SetState)}, 交地错误： {realloc}");
            }

            var loc = RealLocations.First(x => x.realloc == realloc);

            if (loc.priority != Priority.DISABLE) {
                loc.state = state;
                if (!string.IsNullOrEmpty(panelno)) {
                    loc.panelno = panelno;
                }
            }
        }
        
        private void Unmap(string realloc, string includePanelno, string excludePanelno) {
            var real = RealLocations.Single(x => x.realloc == realloc);

            foreach (var k in LocMap.Keys.ToList()) {
                var cond = true;
                if (!string.IsNullOrEmpty(includePanelno)) {
                    cond = cond && real.panelno == includePanelno;
                }
                if (!string.IsNullOrEmpty(excludePanelno)) {
                    cond = cond && real.panelno != excludePanelno;
                }
                if (LocMap[k] == realloc && cond) {
                    LocMap[k] = "";
                }
            }

            // 检查结果的正确性
            if (LocMap.Any(x => x.Value == realloc)) {
                throw new Exception($"来源: {nameof(Unmap)}, 未能解除交地对应: {realloc}");
            }
        }

        private void Unmap(string realloc) {
            foreach (var k in LocMap.Keys.ToList()) {
                if (LocMap[k] == realloc) {
                    LocMap[k] = "";
                }
            }

            // 检查结果的正确性
            if (LocMap.Any(x => x.Value == realloc)) {
                throw new Exception($"来源: {nameof(Unmap)}, 未能解除交地对应: {realloc}");
            }
        }

        private bool IsRealLocExists(string loc) {
            var q = RealLocations.Count(x => x.realloc == loc) == 1;
            return q;
        }

        // 交地可用
        private bool IsRealEnabled(string realloc) {
            var count = RealLocations.Count(x => x.realloc == realloc && x.priority != Priority.DISABLE);
            return count == 1;
        }

        private void Map(string virtualloc, string realloc, string panelno) {
            if (!LocMap.ContainsKey(virtualloc)) {
                throw new Exception($"来源: {nameof(Map)}, 名义交地错误: {virtualloc}");
            }

            if (!IsRealLocExists(realloc)) {
                throw new Exception($"函数: {nameof(Map)}, 实际交地错误： {realloc}");
            }

            if (!IsRealEnabled(realloc)) {
                throw new Exception($"函数: {nameof(Map)}, 交地禁用： {realloc}");
            }

            Unmap(realloc, "", panelno);
            LocMap[virtualloc] = realloc;
            SetState(realloc, LocationState.BUSY, panelno);
        }

        // 找到最近的一个空板
        private string FindAvailableRealLoc(Priority p) {
            // 如果有板闲置，返回该板号, 无则返回空字符串。
            RealLoc locs = null;
            if (p == Priority.HIGH) {
                locs = RealLocations.Where(x => x.state == LocationState.IDLE && x.priority != Priority.DISABLE)
                    .OrderByDescending(x => x.priority)
                    .FirstOrDefault();
            } else {
                // 先从中低优先级较低中查找。
                locs = RealLocations.Where(x => x.state == LocationState.IDLE && (x.priority == Priority.MEDIUM || x.priority == Priority.LOW))
                    .OrderByDescending(x => x.priority)
                    .FirstOrDefault();
                // 中低优先级没有，再从高优先级中查找。
                if (locs == null) {
                    locs = RealLocations.Where(x => x.state == LocationState.IDLE && (x.priority != Priority.DISABLE))
                    .OrderByDescending(x => x.priority)
                    .FirstOrDefault();
                }
            }

            //if (locs == null) {
            //    locs = RealLocations.Where(x => x.state == LocationState.FULL && x.priority != Priority.DISABLE)
            //        .OrderBy(x => x.priority)
            //        .FirstOrDefault();
            //}

            if (locs != null) {
                return locs.realloc;
            } else {
                return "";
            }
        }

        private string FindInMapByRealLoc(string realloc) {
            return LocMap.Where(x => x.Value == realloc).Select(x => x.Key)
                .FirstOrDefault();
        }

        private bool automap(string virtualloc, string panelno) {
            var p = VirtualLocations.Single(x => x.virtualloc == virtualloc)
                .priority;
            var realloc = FindAvailableRealLoc(p);
            if (!string.IsNullOrEmpty(realloc)) {
                Map(virtualloc, realloc, panelno);
                return true;
            } else {
                return false;
            }
        }

        #endregion

        #region public_func
        // 根据erp所指的交地，换算出真实交地
        public string Convert(string virtualloc, string panelno) {
            var rt = string.Empty;
            var trytimes = 120; // 尝试时间, 120s.

            while (true) {
                var realoc1 = RealLocations.FirstOrDefault(x => x.panelno == panelno);
                if (realoc1 != null) {
                    return realoc1.realloc;
                }

                if (!LocMap.ContainsKey(virtualloc)) {
                    throw new Exception($"来源: {nameof(Convert)}, 名义交地错误: {virtualloc}");
                }

                if (LocMap[virtualloc] == string.Empty) {
                    automap(virtualloc, panelno);
                } else {
                    var realoc = RealLocations.Single(x => x.realloc == LocMap[virtualloc]);

                    if (realoc.panelno == panelno) { return realoc.realloc; } else {
                        Unmap(realoc.realloc, "", "");
                        // SetState(realoc.realloc, LocationState.FULL, "");
                        automap(virtualloc, panelno);
                    };
                }

                // 验证是否成功
                rt = LocMap[virtualloc];
                trytimes--;

                if (trytimes >= 0 && string.IsNullOrEmpty(rt)) {
                    Thread.Sleep(1000);
                } else {
                    break;
                }
            }

            return rt;
        }

        public override string ToString() {
            var s = new StringBuilder();
            foreach (var item in LocMap) {
                s.Append($"{item.Key} => {item.Value}");
            }
            return s.ToString();
        }

        public static string state_s(LocationState s) {
            var v = "";
            switch (s) {
                case LocationState.BUSY:
                    v = "忙";
                    break;
                case LocationState.FULL:
                    v = "满板";
                    break;
                case LocationState.IDLE:
                    v = "空闲";
                    break;
            }
            return v;
        }

        public static string priority_s(Priority p) {
            var v = "";
            switch (p) {
                case Priority.HIGH:
                    v = "高";
                    break;
                case Priority.MEDIUM:
                    v = "中";
                    break;
                case Priority.LOW:
                    v = "低";
                    break;
                case Priority.DISABLE:
                    v = "禁用";
                    break;
            }
            return v;
        }

        public void Resetall() {
            ResetVirtual();
            ResetReal();
        }

        public void ResetVirtual() {
            var keys = LocMap.Select(x => x.Key).ToList();
            foreach (var k in keys) {
                LocMap[k] = "";
            }
        }

        public string lookupVirtual(string reallocation) {
            var v = LocMap.Where(x => x.Value == reallocation);
            if (v.Count() == 1) {
                return v.First().Key;
            } else {
                throw new Exception($"来源: {nameof(lookupVirtual)}, 查找不到{reallocation}对应的虚拟交地。");
            }
        }

        public void ResetReal() {
            for (var i = 0; i < RealLocations.Length; i++) {
                if (RealLocations[i].priority != Priority.DISABLE) {
                    RealLocations[i].state = LocationState.IDLE;
                }
            }
        }

        public static LocationHelper LoadRealDefaultPriority() {
            var exe = Assembly.GetExecutingAssembly().Location;
            var exepath = Path.GetDirectoryName(exe);
            return LoadConf(Path.Combine("location_default.json"));
        }

        public static LocationHelper LoadConf(string fn) {
            var jsonstr = File.ReadAllText(fn);
            var loc = JsonConvert.DeserializeObject<LocationHelper>(jsonstr);
            if (loc.VirtualLocations.Count() != 10) {
                throw new Exception($"名义交地配置异常: {fn}");
            }

            if (loc.RealLocations.Count() != 11) {
                throw new Exception($"实际交地配置异常: {fn}");
            }
            return loc;
        }

        public void SaveConf(string fn) {
            var jsonstr = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(fn, jsonstr);
        }
        #endregion

        #region event_handler
        /// <summary>
        /// 当板满的时候，调用此函数。
        /// </summary>
        /// <param name="realloc"></param>
        public void OnFull(string realloc) {
            if (!IsRealLocExists(realloc)) {
                throw new Exception($"来源: {nameof(OnFull)}, 交地错误: {realloc}");
            }

            var real = RealLocations.Single(x => x.realloc == realloc);
            if (real.state == LocationState.BUSY) {
                SetState(realloc, LocationState.FULL, real.panelno);
            }
        }

        /// <summary>
        /// 板准备好时，设置此状态。
        /// </summary>
        /// <param name="realloc"></param>
        public void OnReady(string realloc) {
            var self = nameof(OnReady);

            var real = RealLocations.FirstOrDefault(x => x.realloc == realloc);
            if (real == null) {
                throw new Exception($"来源: {self}, 交地错误: {realloc}");
            }

            if (real.state == LocationState.FULL) {
#if DEBUG
                Thread.Sleep(1000 * 30); // 调试用，等半分钟
#endif
                //Unmap(realloc);
                SetState(realloc, LocationState.IDLE, "");
            }
        }
        #endregion
    }
}
