using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Mathematics;

namespace Shapoco.Calctus.Model.Standards {
    static class UnixTime {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static real FromLocalTime(DateTime dt)
            => (real)(dt.ToUniversalTime() - Epoch).TotalSeconds;

        public static real FromLocalTime(int year, int mon, int day, int hour, int min, real sec) {
            int secInt = (int)RMath.Floor(sec);
            return FromLocalTime(new DateTime(year, mon, day, hour, min, secInt)) + (sec - secInt);
        }

        public static DateTime ToLocalTime(real unixTime)
            => Epoch.AddSeconds((double)unixTime).ToLocalTime();

        public static real Now => FromLocalTime(DateTime.Now);
        public static real Today => FromLocalTime(DateTime.Today);
    }
}
