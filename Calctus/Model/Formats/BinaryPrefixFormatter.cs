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
        private static readonly Regex patternRegex = new Regex(@"(?<frac>([1-9][0-9]*(_[0-9]+)*|0)(\.[0-9]+(_[0-9]+)*)?|(\.[0-9]+(_[0-9]+)*))(?<prefix>[" + Prefixes + "])i");
        public const int MinPrefixIndex = 0;
        public const int MaxPrefixIndex = 9;

        public BinaryPrefixFormatter() : base(patternRegex, FormatPriority.Strong) { }

        private static void extractMatch(Match m, out decimal frac, out int prefixIndex) {
            frac = real.Parse(m.Groups["frac"].Value);
            prefixIndex = Prefixes.IndexOf(m.Groups["prefix"].Value);
            System.Diagnostics.Debug.Assert(prefixIndex >= 0);
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

        public override Val Parse(Match m) {
            extractMatch(m, out var frac, out var prefixIndex);
            var exp = prefixIndex * 10;
            return new RealVal(frac * Math.Truncate((decimal)Math.Pow(2, exp)), new FormatHint(this));
        }

        protected override string OnFormat(Val val, FormatSettingss fs) {
            if (val is RealVal) {
                var r = val.AsReal;
                int prefixIndex = 0;
                if (r != 0) {
                    prefixIndex = (int)RMath.Floor(RMath.Log2(RMath.Abs(r)) / 10);
                }
                if (prefixIndex < MinPrefixIndex) {
                    prefixIndex = MinPrefixIndex;
                }
                else if (prefixIndex > MaxPrefixIndex) {
                    prefixIndex = MaxPrefixIndex;
                }
                var exp = prefixIndex * 10;
                var frac = r / Math.Truncate((decimal)Math.Pow(2, exp));
                if (prefixIndex == 0) {
                    return RealToString(frac, fs, false);
                }
                else {
                    return RealToString(frac, fs, false) + GetPrefixString(prefixIndex);
                }
            }
            else {
                return base.OnFormat(val, fs);
            }
        }

        public static string GetPrefixString(int prefixIndex) {
            return Prefixes[prefixIndex] + "i";
        }
    }
}
