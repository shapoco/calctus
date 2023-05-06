using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.Standard {
    static class UnixTime {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        public static real FromLocalTime(DateTime dt) => (real)(dt.ToUniversalTime() - Epoch).TotalSeconds;
        public static DateTime ToLocalTime(real unixTime) => Epoch.AddSeconds((double)unixTime).ToLocalTime();
    }
}
