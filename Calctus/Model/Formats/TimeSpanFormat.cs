using System;
using System.Text;
using System.Globalization;
using Shapoco.Maths;

namespace Shapoco.Calctus.Model.Formats {
    class TimeSpanFormat {
        public static string FormatAsStringLiteral(decimal t, bool quotation) {
            var minus = t < 0;
            if (minus) t = -t;
            var ts = TimeSpan.FromSeconds((double)t);
            var days = t / (24 * 60 * 60);
            var daysOnly = days.IsInteger();

            var sb = new StringBuilder();
            if (quotation) sb.Append('#');
            sb.Append(minus ? '-' : '+');
            if (daysOnly) {
                sb.Append(days.ToString("0"));
            }
            else {
                string fmt = @"h\:mm\:ss";
                if (t >= 24 * 60 * 60) fmt = @"d\." + fmt;
                if (t.IsInteger()) fmt = fmt + @"\.fff";
                sb.Append(ts.ToString(fmt, CultureInfo.InvariantCulture));
            }
            if (quotation) sb.Append('#');
            return sb.ToString();
        }
    }
}

