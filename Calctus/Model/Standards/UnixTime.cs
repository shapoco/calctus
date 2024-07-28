using System;

namespace Shapoco.Calctus.Model.Standards {
    static class UnixTime {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static decimal FromLocalTime(DateTime dt)
            => (decimal)(dt.ToUniversalTime() - Epoch).TotalSeconds;

        public static decimal FromLocalTime(int year, int mon, int day, int hour, int min, decimal sec) {
            int secInt = (int)Math.Floor(sec);
            return FromLocalTime(new DateTime(year, mon, day, hour, min, secInt)) + (sec - secInt);
        }

        public static DateTime ToLocalTime(decimal unixTime)
            => Epoch.AddSeconds((double)unixTime).ToLocalTime();

        public static decimal Now => FromLocalTime(DateTime.Now);
        public static decimal Today => FromLocalTime(DateTime.Today);
    }
}
