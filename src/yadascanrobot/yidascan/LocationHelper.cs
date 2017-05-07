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
                RealLoc.Create("B01", LocationState.IDLE, Priority.HIGH),
                RealLoc.Create("B02", LocationState.IDLE, Priority.MEDIUM),
                RealLoc.Create("B03", LocationState.IDLE, Priority.MEDIUM),
                RealLoc.Create("B04", LocationState.IDLE, Priority.HIGH),
                RealLoc.Create("B05", LocationState.IDLE, Priority.MEDIUM),
                RealLoc.Create("B06", LocationState.IDLE, Priority.MEDIUM),
                RealLoc.Create("B07", LocationState.IDLE, Priority.LOW),
                RealLoc.Create("B08", LocationState.IDLE, Priority.LOW),
                RealLoc.Create("B09", LocationState.IDLE, Priority.LOW),
                RealLoc.Create("B10", LocationState.IDLE, Priority.LOW),
                RealLoc.Create("B11", LocationState.IDLE, Priority.LOW)
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

        public string Current(string virtualloc) {
            if (!LocMap.ContainsKey(virtualloc)) {
                throw new Exception($"来源: {nameof(Current)}, 交地错误: {virtualloc}");
            }

            return LocMap[virtualloc];
        }

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

        private void SetState(string realloc, LocationState state) {
            if (!IsRealLocExists(realloc)) {
                throw new Exception($"函数: {nameof(SetState)}, 交地错误： {realloc}");
            }

            var loc = RealLocations.First(x => x.realloc == realloc);
            loc.state = state;
        }

        public void Unmap(string realloc) {
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
            return RealLocations.Exists(x => x.realloc == loc);
        }

        private void Map(string virtualloc, string realloc) {
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
        private string FindAvailableRealLoc() {
            // 如果有板闲置，返回该板号, 无则返回空字符串。
            var locs = RealLocations.Where(x => x.state == LocationState.IDLE)
                .OrderByDescending(x => x.priority)
                .FirstOrDefault();

            if (locs != null) {
                locs = RealLocations.Where(x => x.state == LocationState.FULL)
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
            return LocMap.Where(x => x.Value == realloc).Select(x => x.Key).FirstOrDefault();
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
            if (!IsRealLocExists(realloc)) {
                throw new Exception($"来源: {nameof(OnReady)}, 交地错误: {realloc}");
            }

            var virtualloc = FindInMapByRealLoc(realloc);
            if (!string.IsNullOrEmpty(virtualloc)) {
                throw new Exception($"来源: {nameof(OnReady)}, 交地忙: {realloc}");
            }

            var loc = RealLocations.Single(x => x.realloc == realloc);
            loc.state = LocationState.IDLE;
        }

        // 判断真实板号是否可用。
        private bool IsRealAvailable(string realloc) {
            if (!IsRealLocExists(realloc)) {
                throw new Exception($"来源: {nameof(IsRealAvailable)}, 交地错误: {realloc}");
            }

            var loc = RealLocations.Single(x => x.realloc == realloc);
            return loc.state == LocationState.IDLE;
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

        public static string priority_s(Priority p) {
            var v = "";
            switch(p) {
                case Priority.HIGH:
                    v = "高";
                    break;
                case Priority.MEDIUM:
                    v = "中";
                    break;
                case Priority.LOW:
                    v = "低";
                    break;
            }
            return v;
        }

        public void panelCheckLoop(IRobotJob robot) {
            Task.Run(() => {
                foreach(var item in RealLocations) {
                    if (item.state == LocationState.FULL) {
                        if (robot.PanelAvailable(item.realloc)) {
                            item.state = LocationState.IDLE;
                        }
                    }
                }

                Task.Delay(500);
            });
        }
    }
}
