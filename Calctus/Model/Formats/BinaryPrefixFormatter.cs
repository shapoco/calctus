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
    class BinaryPrefixFormatter : NumberFormatter {
        private static readonly string Prefixes = "_kMGTPEZYR";

        public BinaryPrefixFormatter() : base(new Regex(@"(([1-9][0-9]*|0?)(\.[0-9]+)?)([" + Prefixes + "])i"), FormatPriority.NextPriority) { }

        public override Val Parse(Match m) {
            var frac = decimal.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);
            var prefixIndex = Prefixes.IndexOf(m.Groups[4].Value);
            var exp = prefixIndex * 10;
            return new RealVal(frac * Math.Truncate((decimal)Math.Pow(2, exp)), new FormatHint(this));
        }

        protected override string OnFormat(Val val, EvalContext e) {
            if (val is RealVal) {
                var r = val.AsReal;
                int prefixIndex = 0;
                if (r != 0) {
                    prefixIndex = (int)RMath.Floor(RMath.Log2(RMath.Abs(r)) / 10);
                }
                if (prefixIndex < 0) {
                    prefixIndex = 0;
                }
                else if (prefixIndex >= Prefixes.Length) {
                    prefixIndex = Prefixes.Length - 1;
                }
                var exp = prefixIndex * 10;
                var frac = r / Math.Truncate((decimal)Math.Pow(2, exp));
                if (prefixIndex == 0) {
                    return RealToString(frac, e, false);
                }
                else {
                    return RealToString(frac, e, false) + Prefixes[prefixIndex] + "i";
                }
            }
            else {
                return base.OnFormat(val, e);
            }
        }
    }
}
