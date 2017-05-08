using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
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

        public List<VirtualLoc> VirtualLocations;

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
                RealLoc.Create("B01", LocationState.IDLE, Priority.HIGH),
                RealLoc.Create("B02", LocationState.IDLE, Priority.MEDIUM),
                RealLoc.Create("B03", LocationState.IDLE, Priority.HIGH),
                RealLoc.Create("B04", LocationState.IDLE, Priority.HIGH),
                RealLoc.Create("B05", LocationState.IDLE, Priority.HIGH),
                RealLoc.Create("B06", LocationState.IDLE, Priority.MEDIUM),
                RealLoc.Create("B07", LocationState.IDLE, Priority.MEDIUM),
                RealLoc.Create("B08", LocationState.IDLE, Priority.MEDIUM),
                RealLoc.Create("B09", LocationState.IDLE, Priority.LOW),
                RealLoc.Create("B10", LocationState.IDLE, Priority.HIGH),
                RealLoc.Create("B11", LocationState.IDLE, Priority.DISABLE)
            };

            VirtualLocations = new List<VirtualLoc>() {
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
        private void SetState(string realloc, LocationState state) {
            if (!IsRealLocExists(realloc)) {
                throw new Exception($"函数: {nameof(SetState)}, 交地错误： {realloc}");
            }

            var loc = RealLocations.First(x => x.realloc == realloc);

            if (loc.priority != Priority.DISABLE) {
                loc.state = state;
            }
        }

        private void Unmap(string realloc) {
            if (string.IsNullOrEmpty(realloc)) {
                return;
            }

            var kyes = LocMap.Where(x => x.Value == realloc)
                .Select(x => x.Key)
                .ToList(); ;

            foreach (var k in kyes) {
                if (LocMap[k] == realloc) {
                    LocMap[k] = "";
                }
            }

            SetState(realloc, LocationState.IDLE);
        }

        private bool IsRealLocExists(string loc) {
            var q = RealLocations.Count(x => x.realloc == loc) == 1;
            return q;
        }

        // 交地可用
        private bool IsRealEnable(string realloc) {
            var count = RealLocations.Count(x => x.realloc == realloc && x.priority != Priority.DISABLE);
            return count == 1;
        }

        private void Map(string virtualloc, string realloc) {
            if (!LocMap.ContainsKey(virtualloc)) {
                throw new Exception($"来源: {nameof(Map)}, 名义交地错误: {virtualloc}");
            }

            if (!IsRealLocExists(realloc)) {
                throw new Exception($"函数: {nameof(Map)}, 实际交地错误： {realloc}");
            }

            if (!IsRealEnable(realloc)) {
                throw new Exception($"函数: {nameof(Map)}, 交地禁用： {realloc}");
            }

            Unmap(realloc);
            LocMap[virtualloc] = realloc;
            SetState(realloc, LocationState.BUSY);
        }

        // 找到最近的一个空板
        private string FindAvailableRealLoc() {
            // 如果有板闲置，返回该板号, 无则返回空字符串。
            var locs = RealLocations.Where(x => x.state == LocationState.IDLE && x.priority != Priority.DISABLE)
                .OrderByDescending(x => x.priority)
                .FirstOrDefault();

            if (locs == null) {
                locs = RealLocations.Where(x => x.state == LocationState.FULL && x.priority != Priority.DISABLE)
                    .OrderBy(x => x.priority)
                    .FirstOrDefault();
            }

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

        private bool automap(string virtualloc) {
            var realloc = FindAvailableRealLoc();
            if (!string.IsNullOrEmpty(realloc)) {
                Map(virtualloc, realloc);
                return true;
            } else {
                return false;
            }
        }

        //private void copyfrom(LocationHelper loc_) {
        //    foreach (var item in loc_.LocMap) {
        //        if (LocMap.Keys.Contains(item.Key)) {
        //            LocMap[item.Key] = item.Value;
        //        } else {
        //            LocMap.Add(item.Key, item.Value);
        //        }
        //    }

        //    foreach(var item in VirtualLocations) {
        //        var l = VirtualLocations.Single(x => x.virtualloc == item.virtualloc);
        //        if (l != null) {
        //            l.priority = item.priority;
        //        }
        //    }

        //    foreach(var item in loc_.RealLocations) {
        //        var l = RealLocations.Single(x => x.realloc == item.realloc);
        //        if (l != null) {
        //            l.priority = item.priority;
        //            l.state = item.state;
        //        } else {
        //            throw new Exception($"来源: {nameof(copyfrom)}, 不存在板位{item.realloc}");
        //        }
        //    }
        //}

        #endregion

        #region public_func
        // 根据erp所指的交地，换算出真实交地
        public string Convert(string virtualloc) {
            if (!LocMap.ContainsKey(virtualloc)) {
                throw new Exception($"来源: {nameof(Convert)}, 交地错误: {virtualloc}");
            }

            if (LocMap[virtualloc] == string.Empty) {
                automap(virtualloc);
            }

            return LocMap[virtualloc];
        }

        public string Current(string virtualloc) {
            if (!LocMap.ContainsKey(virtualloc)) {
                throw new Exception($"来源: {nameof(Current)}, 交地错误: {virtualloc}");
            }

            return LocMap[virtualloc];
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

        public void ResetReal() {
            for (var i = 0; i < RealLocations.Length; i++) {
                if (RealLocations[i].priority != Priority.DISABLE) {
                    RealLocations[i].state = LocationState.IDLE;
                }
            }
        }

        public void SetRealDefaultPriority() {
            var keys = LocMap.Select(x => x.Key).ToList();
            foreach (var k in keys) {
                if (IsRealEnable(k)) {
                    LocMap[k] = k;
                } else {
                    LocMap[k] = "";
                }
            }

            for (var i = 0; i < RealLocations.Length; i++) {
                if (RealLocations[i].priority != Priority.DISABLE) {
                    RealLocations[i].state = LocationState.BUSY;
                }
            }
        }

        public static LocationHelper LoadConf(string fn) {
            var jsonstr = File.ReadAllText(fn);
            return JsonConvert.DeserializeObject<LocationHelper>(jsonstr);
        }

        public void SaveConf(string fn) {
            var jsonstr = JsonConvert.SerializeObject(this);
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

            Unmap(realloc);
            SetState(realloc, LocationState.FULL);
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
                SetState(realloc, LocationState.IDLE);
            }
        }
#endregion
    }
}
