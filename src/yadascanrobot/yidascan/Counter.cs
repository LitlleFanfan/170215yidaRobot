using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yidascan {
    class Counter {
        public Dictionary<string, int> counter { get; set; }

        public Counter() { counter = new Dictionary<string, int>(); }

        public void setup(IEnumerable<string> locs) {
            counter = new Dictionary<string, int>();
            foreach (var item in locs) {
                counter.Add(item, 0);
            }
        }

        public void reset() {
            foreach (var k in counter) {
                counter[k.Key] = 0;
            }
        }

        public void inc(string loc) {
            if (counter.ContainsKey(loc)) {
                counter[loc] += 1;
            } else {
                counter.Add(loc, 1);
            }
        }

        public int total(string area) {
            if (string.IsNullOrEmpty(area)) {
                return counter.Sum(x => x.Value);
            } else {
                return counter.Where(x => x.Key.StartsWith(area.ToUpper())).Sum(x => x.Value);
            }
        }

        public const int LOW = 0;
        public const int MEDIUM = 1;
        public const int HIGH = 2;

        public int hotareab(string loc) {
            if (!loc.ToUpper().StartsWith(Area.B)) {
                return -1;
            }

            var limit = (int)Math.Ceiling(counter.Count(x => x.Key.StartsWith(Area.B)) * .33);
            
            var q = counter.Where(x => x.Key.StartsWith(Area.B))
                .OrderBy(x => x.Value)
                .Select(x => x.Key);
            var qlow = q.Take(limit);
            var qmedium = q.Skip(limit).Take(limit);

            if (qlow.Contains(loc)) { return LOW; }
            else if (qmedium.Contains(loc)) { return MEDIUM; }
            else { return HIGH; }
        }
    }
}
