using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.Syntax {
    class IntFormatter : NumberFormatter {
        public readonly int Radix;
        public string Prefix;

        public IntFormatter(int radix, string prefix, Regex regex, int groupIndex) : base(regex, groupIndex) {
            this.Radix = radix;
            this.Prefix = prefix;
        }

        public override Val Parse(Match m) {
            System.Diagnostics.Debug.Assert(m.Groups[CaptureGroupIndex].Length > 0);
            var tok = m.Groups[CaptureGroupIndex].Value;
            if (Radix == 10) {
                return new RealVal(real.Parse(tok), new ValFormatHint(this));
            }
            else {
                return new RealVal(Convert.ToInt64(tok, Radix), new ValFormatHint(this));
            }
        }

        protected override string OnFormat(Val val, EvalContext e) {
            if (val is RealVal) {
                var fval = val.AsReal;
                var ival = RMath.Truncate(fval);

                // 10進表記、かつ指数表記対象に該当する場合はデフォルトの数値表現を使う
                int exp = RMath.FLog10(val.AsReal);
                var s = e.Settings;
                bool enotation =
                    Radix == 10 &&
                    s.ENotationEnabled &&
                    (exp >= s.ENotationExpPositiveMin || exp <= s.ENotationExpNegativeMax);

                if (fval != ival || ival < long.MinValue || long.MaxValue < ival || enotation) {
                    // デフォルトの数値表現
                    return base.OnFormat(val, e);
                }
                else if (Radix == 10) {
                    // 10進表現
                    var abs64val = ival >= 0 ? (decimal)ival : -(decimal)ival;
                    var ret = Convert.ToString(abs64val);
                    ret = Prefix + ret;
                    if (ival < 0) ret = "-" + ret;
                    return ret;
                }
                else {
                    // 10進以外
                    return Prefix + Convert.ToString((Int64)ival, Radix);
                }
            }
            else {
                return base.OnFormat(val, e);
            }
        }
    }
}
