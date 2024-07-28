using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Globalization;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Maths;
using Shapoco.Calctus.Model.Maths.Types;

namespace Shapoco.Calctus.Model.Formats {
    class RelativeTimeFormat : ValFormat {
        private static readonly Regex pattern = new Regex(@"#(?<time>[+-]\d+(\.\d+:\d+(:\d+(\.\d+)?)?|:\d+(:\d+(\.\d+)?)?)?)#");

        private static RelativeTimeFormat _instance;
        public static RelativeTimeFormat Instance => (_instance != null) ? _instance : (_instance = new RelativeTimeFormat());

        private RelativeTimeFormat() : base(TokenType.SpecialLiteral, pattern, FormatPriority.AlwaysLeft) { }

        protected override Val OnParse(Match m) {
            var tok = m.Groups["time"].Value;
            if (tok[0] == '+') tok = tok.Substring(1);
            var timeSpan = (decimal)TimeSpan.Parse(tok).TotalSeconds;
            return new RealVal(timeSpan, new FormatHint(this));
        }

        protected override string OnFormat(Val val, FormatSettings fs) {
            if (!(val is RealVal)) {
                return base.OnFormat(val, fs);
            }
            return FormatAsStringLiteral(val.AsDecimal);
        }

        public static string FormatAsStringLiteral(decimal t) {
            var minus = t < 0;
            if (minus) t = -t;
            var ts = TimeSpan.FromSeconds((double)t);
            var days = t / (24 * 60 * 60);
            var daysOnly = days.IsInteger();

            var sb = new StringBuilder("#");
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
            sb.Append('#');
            return sb.ToString();
        }
    }
}

