using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Mathematics;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Formats {
    class SiPrefixFormatter : NumberFormatter {
        private static readonly string Prefixes = "ryzafpnum_kMGTPEZYR";
        private const int PrefixOffset = 9;

        public SiPrefixFormatter() : base(new Regex(@"(([1-9][0-9]*|0)(\.[0-9]+)?|(\.[0-9]+))([" + Prefixes + "])"), FormatPriority.Strong) { }

        public override Val Parse(Match m) {
            var frac = decimal.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);
            var prefixIndex = Prefixes.IndexOf(m.Groups[5].Value);
            var exp = (prefixIndex - PrefixOffset) * 3;
            return new RealVal(frac * RMath.Pow10(exp) , new FormatHint(this));
        }

        protected override string OnFormat(Val val, EvalContext e) {
            if (val is RealVal) {
                var r = val.AsReal;
                int prefixIndex = PrefixOffset;
                if (r != 0) {
                    prefixIndex = (int)RMath.Floor(RMath.Log10(RMath.Abs(r)) / 3) + PrefixOffset;
                }
                if (prefixIndex < 0) {
                    prefixIndex = 0;
                }
                else if (prefixIndex >= Prefixes.Length) {
                    prefixIndex = Prefixes.Length - 1;
                }
                var exp = (prefixIndex - PrefixOffset) * 3;
                var frac = r / RMath.Pow10(exp);
                if (prefixIndex == PrefixOffset) {
                    return RealToString(frac, e, false);
                }
                else {
                    return RealToString(frac, e, false) + Prefixes[prefixIndex];
                }
            }
            else {
                return base.OnFormat(val, e);
            }
        }
    }
}
