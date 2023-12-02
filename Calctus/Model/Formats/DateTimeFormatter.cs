using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Globalization;
using Shapoco.Calctus.Model.Standards;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Formats {
    class DateTimeFormatter : NumberFormatter {
        public DateTimeFormatter() : base(new Regex(@"#(?<datetime>\d+/\d+/\d+|\d+:\d+:\d+(\.\d+)?|\d+/\d+/\d+ \d+:\d+:\d+(\.\d+)?)#"), FormatPriority.AlwaysLeft) { }

        public override Val Parse(Match m) {
            var tok = m.Groups["datetime"].Value;
            var unixTime = UnixTime.FromLocalTime(System.DateTime.Parse(tok));
            return new RealVal(unixTime, new FormatHint(this));
        }

        protected override string OnFormat(Val val, FormatSettingss fs) {
            if (!(val is RealVal)) {
                return base.OnFormat(val, fs);
            }

            var fval = val.AsReal;
            if (fval < ulong.MinValue || ulong.MaxValue < fval) {
                // ulongの範囲外の値はデフォルトの数値表現を使用
                return base.OnFormat(val, fs);
            }
            else {
                // Unix Time からローカル時刻に変換
                return ToString(UnixTime.ToLocalTime(fval));
            }
        }

        public static string ToString(DateTime t) => "#" + t.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture) + "#";
    }
}
