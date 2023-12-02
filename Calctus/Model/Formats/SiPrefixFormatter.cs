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
        private static readonly Regex patternRegex = new Regex(@"(?<frac>([1-9][0-9]*(_[0-9]+)*|0)(\.[0-9]+(_[0-9]+)*)?|(\.[0-9]+(_[0-9]+)*))(?<prefix>[" + Prefixes + "])");
        private const int PrefixIndexOffset = 9;
        public const int MinPrefixIndex = -PrefixIndexOffset;
        public const int MaxPrefixIndex = PrefixIndexOffset;

        public SiPrefixFormatter() : base(patternRegex, FormatPriority.Strong) { }

        private static void extractMatch(Match m, out decimal frac, out int prefixIndex) {
            frac = real.Parse(m.Groups["frac"].Value);
            int i = Prefixes.IndexOf(m.Groups["prefix"].Value);
            System.Diagnostics.Debug.Assert(i >= 0);
            prefixIndex = i - PrefixIndexOffset;
        }

        public static bool TryParse(string str, out decimal frac, out int prefixIndex) {
            frac = 0;
            prefixIndex = 0;
            var m = patternRegex.Match(str);
            if (m.Success && m.Index == 0 && m.Length == str.Length) {
                extractMatch(m, out frac, out prefixIndex);
                return true;
            }
            else {
                return false;
            }
        }

        public static void Parse(string str, out decimal frac, out int prefixIndex) {
            if (!TryParse(str, out frac, out prefixIndex)) {
                throw new CalctusError("Invalid SI prefixed format");
            }
        }

        public override Val Parse(Match m) {
            extractMatch(m, out var frac, out var prefixIndex);
            var exp = prefixIndex * 3;
            return new RealVal(frac * RMath.Pow10(exp) , new FormatHint(this));
        }

        protected override string OnFormat(Val val, FormatSettingss fs) {
            if (val is RealVal) {
                var r = val.AsReal;
                int prefixIndex = 0;
                if (r != 0) {
                    prefixIndex = (int)RMath.Floor(RMath.Log10(RMath.Abs(r)) / 3);
                }
                if (prefixIndex < MinPrefixIndex) {
                    prefixIndex = MinPrefixIndex;
                }
                else if (prefixIndex > MaxPrefixIndex) {
                    prefixIndex = MaxPrefixIndex;
                }
                var exp = prefixIndex * 3;
                var frac = r / RMath.Pow10(exp);
                if (prefixIndex == 0) {
                    return RealToString(frac, fs, false);
                }
                else {
                    return RealToString(frac, fs, false) + GetPrefixChar(prefixIndex);
                }
            }
            else {
                return base.OnFormat(val, fs);
            }
        }

        public static char GetPrefixChar(int prefixIndex) {
            return Prefixes[prefixIndex + PrefixIndexOffset];
        }
    }
}
