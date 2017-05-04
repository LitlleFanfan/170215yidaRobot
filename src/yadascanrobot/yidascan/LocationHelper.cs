using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yidascan {
    public enum LocationState {
        IDLE = 0,
        BUSY = 1,
        FULL = 2
    }

    public enum Priority {
        LOW = 0,
        MEDIUM = 1,
        HIGH = 2
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
        public string readloc { get; set; }
        public LocationState state { get; set; }

        public RealLoc(string loc, LocationState s) {
            this.readloc = loc;
            this.state = s;
        }

        public static RealLoc Create(string loc, LocationState s) {
            return new RealLoc(loc, s);
        }
    }

    public class LocationHelper {
        // 虚拟板号，key是虚拟板号，value是真实板号。
        public Dictionary<string, string> LocMap;

        // key是真实板号，value是板状态。
        // 板满时，value是false，空时，value是true。
        public List<RealLoc> RealLocations;

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

            RealLocations = new List<RealLoc>() {
                RealLoc.Create("B01", LocationState.IDLE),
                RealLoc.Create("B02", LocationState.IDLE),
                RealLoc.Create("B03", LocationState.IDLE),
                RealLoc.Create("B04", LocationState.IDLE),
                RealLoc.Create("B05", LocationState.IDLE),
                RealLoc.Create("B06", LocationState.IDLE),
                RealLoc.Create("B07", LocationState.IDLE),
                RealLoc.Create("B08", LocationState.IDLE),
                RealLoc.Create("B09", LocationState.IDLE),
                RealLoc.Create("B10", LocationState.IDLE),
                RealLoc.Create("B11", LocationState.IDLE)
            };

            VirtualLocations = new List<VirtualLoc>() {
                VirtualLoc.Create("", Priority.MEDIUM),
                VirtualLoc.Create("", Priority.MEDIUM),
                VirtualLoc.Create("", Priority.MEDIUM),
                VirtualLoc.Create("", Priority.MEDIUM),
                VirtualLoc.Create("", Priority.MEDIUM),
                VirtualLoc.Create("", Priority.MEDIUM),
                VirtualLoc.Create("", Priority.MEDIUM),
                VirtualLoc.Create("", Priority.MEDIUM),
                VirtualLoc.Create("", Priority.MEDIUM)
            };
        }

        private bool IsRealLocUsing(string realloc) {
            var count = LocMap.Count(x => x.Value == realloc);
            return count > 0;
        }


        // 根据erp所指的交地，换算出真实交地
        public string GetReal(string virtualloc) {
            if (!LocMap.ContainsKey(virtualloc)) {
                throw new Exception($"来源: {nameof(GetReal)}, 交地错误: {virtualloc}");
            }

            return LocMap[virtualloc];
        }

        private void SetState(string realloc, LocationState state) {
            if (!IsRealLocExists(realloc)) {
                throw new Exception($"函数: {nameof(SetState)}, 交地错误： {realloc}");
            }

            var loc = RealLocations.First(x => x.readloc == realloc);
            loc.state = state;
        }

        private void Unmap(string realloc) {
            if (!IsRealLocExists(realloc)) {
                throw new Exception($"函数: {nameof(Unmap)}, 交地错误： {realloc}");
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
            return RealLocations.Exists(x => x.readloc == loc);
        }

        public void Map(string virtualloc, string realloc) {
            if (!LocMap.ContainsKey(virtualloc)) {
                throw new Exception($"来源: {nameof(Map)}, 交地错误: {virtualloc}");
            }

            if (!IsRealLocExists(realloc)) {
                throw new Exception($"函数: {nameof(Map)}, 交地错误： {realloc}");
            }

            Unmap(realloc);
            LocMap[virtualloc] = realloc;
            SetState(realloc, LocationState.BUSY);
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
            foreach (var k in RealLocations) {
                k.state = LocationState.IDLE;
            }
        }

        // 找到最近的一个空板
        private string FindEmptyRealLoc() {
            // 如果有板闲置，返回该板号, 无则返回空字符串。
            var locs = RealLocations.FirstOrDefault(x => x.state == LocationState.IDLE);
            
            if (locs != null) {
                return locs.readloc;
            } else {
                return "";
            }
        }

        /// <summary>
        /// 当板满的时候，调用此函数。
        /// </summary>
        /// <param name="virtualloc"></param>
        public void OnFull(string virtualloc) {
            if (!LocMap.ContainsKey(virtualloc)) {
                throw new Exception($"来源: {nameof(OnFull)}, 交地错误: {virtualloc}");
            }

            var real = LocMap[virtualloc];
            if (!string.IsNullOrEmpty(real)) {
                SetState(real, LocationState.FULL);
            }

            var newrealloc = FindEmptyRealLoc();
            Map(virtualloc, newrealloc);
        }

        /// <summary>
        /// 板准备好时，设置此状态。
        /// </summary>
        /// <param name="realloc"></param>
        public void OnReady(string realloc) {
            if (!IsRealLocExists(realloc)) {
                throw new Exception($"来源: {nameof(OnReady)}, 交地错误: {realloc}");
            }

            var loc = RealLocations.Single(x => x.readloc == realloc);
            loc.state = LocationState.IDLE;
        }

        // 判断真实板号是否可用。
        public bool IsRealAvailable(string realloc) {
            if (!IsRealLocExists(realloc)) {
                throw new Exception($"来源: {nameof(IsRealAvailable)}, 交地错误: {realloc}");
            }

            var loc = RealLocations.Single(x => x.readloc == realloc);
            return loc.state == LocationState.IDLE;
        }

        public override string ToString() {
            var s = new StringBuilder();
            foreach (var item in LocMap) {
                s.Append($"{item.Key} => {item.Value}");
            }
            return s.ToString();
        }

        public static string LocationState_s(LocationState s) {
            var v = "";
            switch(s) {
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
    }
}
