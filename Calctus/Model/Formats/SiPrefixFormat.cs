using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Maths;
using Shapoco.Calctus.Model.Maths.Types;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Formats {
    class SiPrefixFormat : ValFormat {
        public static readonly string Prefixes = "ryzafpnum_kMGTPEZYR";
        private const int PrefixIndexOffset = 9;
        public const int MinPrefixIndex = -PrefixIndexOffset;
        public const int MaxPrefixIndex = PrefixIndexOffset;
        private static readonly Regex pattern
            = new Regex(@"(?<frac>([1-9][0-9]*(_[0-9]+)*|0)(\.[0-9]+(_[0-9]+)*)?|(\.[0-9]+(_[0-9]+)*))(?<prefix>[" + Prefixes + "])");

        private static SiPrefixFormat _instance;
        public static SiPrefixFormat Instance => (_instance != null) ? _instance : (_instance = new SiPrefixFormat());

        public static char GetPrefixChar(int prefixIndex) {
            return Prefixes[prefixIndex + PrefixIndexOffset];
        }

        public static bool TryParse(string str, out decimal frac, out int prefixIndex) {
            frac = 0;
            prefixIndex = 0;
            var m = pattern.Match(str);
            if (m.Success && m.Index == 0 && m.Length == str.Length) {
                extractMatch(m, out frac, out prefixIndex);
                return true;
            }
            else {
                return false;
            }
        }

        private static void extractMatch(Match m, out decimal frac, out int prefixIndex) {
            frac = DMath.Parse(m.Groups["frac"].Value);
            int i = Prefixes.IndexOf(m.Groups["prefix"].Value);
            System.Diagnostics.Debug.Assert(i >= 0);
            prefixIndex = i - PrefixIndexOffset;
        }

        private SiPrefixFormat() : base(Parsers.TokenType.NumericLiteral, pattern, FormatPriority.Strong) { }

        public static void Parse(string str, out decimal frac, out int prefixIndex) {
            if (!TryParse(str, out frac, out prefixIndex)) {
                throw new CalctusError("Invalid SI prefixed format");
            }
        }

        protected override Val OnParse(Match m) {
            extractMatch(m, out var frac, out var prefixIndex);
            var exp = prefixIndex * 3;
            return new RealVal(frac * DMath.Pow10(exp) , new FormatHint(this));
        }

        protected override string OnFormat(Val val, FormatSettings fs) {
            if (val is RealVal) {
                var r = val.AsDecimal;
                int prefixIndex = 0;
                if (r != 0) {
                    prefixIndex = (int)Math.Floor(DMath.Log10(Math.Abs(r)) / 3);
                }
                if (prefixIndex < MinPrefixIndex) {
                    prefixIndex = MinPrefixIndex;
                }
                else if (prefixIndex > MaxPrefixIndex) {
                    prefixIndex = MaxPrefixIndex;
                }
                var exp = prefixIndex * 3;
                var frac = r / DMath.Pow10(exp);
                if (prefixIndex == 0) {
                    return RealFormat.RealToString(frac, fs, false);
                }
                else {
                    return RealFormat.RealToString(frac, fs, false) + GetPrefixChar(prefixIndex);
                }
            }
            else {
                return base.OnFormat(val, fs);
            }
        }

    }
}
