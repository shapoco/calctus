using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Standard;
using System.Globalization;

namespace Shapoco.Calctus.Model.Syntax {
    class DateTimeFormatter : NumberFormatter {
        public DateTimeFormatter() : base(new Regex(@"#(\d+/\d+/\d+|\d+:\d+:\d+(\.\d+)?|\d+/\d+/\d+ \d+:\d+:\d+(\.\d+)?)#")) { }

        public override Val Parse(Match m) {
            var tok = m.Groups[1].Value;
            var unixTime = UnixTime.FromLocalTime(System.DateTime.Parse(tok));
            return new RealVal(unixTime, new ValFormatHint(this));
        }

        protected override string OnFormat(Val val, EvalContext e) {
            if (!(val is RealVal)) {
                return base.OnFormat(val, e);
            }

            var fval = val.AsReal;
            if (fval < ulong.MinValue || ulong.MaxValue < fval) {
                // ulongの範囲外の値はデフォルトの数値表現を使用
                return base.OnFormat(val, e);
            }
            else {
                // Unix Time からローカル時刻に変換
                return ToString(UnixTime.ToLocalTime(fval));
            }
        }

        public static string ToString(DateTime t) => "#" + t.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture) + "#";
    }
}
