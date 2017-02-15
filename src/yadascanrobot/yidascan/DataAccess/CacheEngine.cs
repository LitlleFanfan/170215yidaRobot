using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yidascan.DataAccess {
    public class CacheEngine {
        public LableCode CurrentLable;
        public LableCode CachedLable;
        public CacheState State;
        public FloorPerformance FloorFlag;
        public bool ToSwap;
    }
}
