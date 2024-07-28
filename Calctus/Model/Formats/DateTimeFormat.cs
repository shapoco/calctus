using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Globalization;
using Shapoco.Calctus.Model.Standards;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Maths;
using Shapoco.Calctus.Model.Maths.Types;

namespace Shapoco.Calctus.Model.Formats {
    class DateTimeFormat : ValFormat {
        private static readonly Regex pattern = new Regex(@"#(?<datetime>\d+/\d+/\d+|\d+:\d+:\d+(\.\d+)?|\d+/\d+/\d+ \d+:\d+:\d+(\.\d+)?)#");

        private static DateTimeFormat _instance;
        public static DateTimeFormat Instance => (_instance != null) ? _instance : (_instance = new DateTimeFormat());

        private DateTimeFormat() : base(TokenType.SpecialLiteral, pattern, FormatPriority.AlwaysLeft) { }

        protected override Val OnParse(Match m) {
            var tok = m.Groups["datetime"].Value;
            var unixTime = UnixTime.FromLocalTime(System.DateTime.Parse(tok));
            return new RealVal(unixTime, new FormatHint(this));
        }

        protected override string OnFormat(Val val, FormatSettings fs) {
            if (!(val is RealVal)) {
                return base.OnFormat(val, fs);
            }

            var fval = val.AsDecimal;
            if (fval < ulong.MinValue || ulong.MaxValue < fval) {
                // ulongの範囲外の値はデフォルトの数値表現を使用
                return base.OnFormat(val, fs);
            }
            else {
                // Unix Time からローカル時刻に変換
                return FormatAsStringLiteral(fval);
            }
        }

        public static string FormatAsStringLiteral(decimal unixTime) {
            return FormatAsStringLiteral(UnixTime.ToLocalTime(unixTime));
        }
        
        public static string FormatAsStringLiteral(DateTime dateTime) {
            if (dateTime.Hour == 0 && dateTime.Minute == 0 && dateTime.Second == 0 && dateTime.Millisecond == 0) {
                return "#" + dateTime.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture) + "#";
            }
            else {
                return "#" + dateTime.ToString("yyyy/MM/dd HH:mm:ss.FFF", CultureInfo.InvariantCulture) + "#";
            }
        }
    }
}
