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
using Shapoco.Calctus.Model.Parsers;

namespace Shapoco.Calctus.Model.Formats {
    class BinaryPrefixFormat : ValFormat {
        public static readonly string Prefixes = "_kMGTPEZYR";
        public const int MinPrefixIndex = 0;
        public const int MaxPrefixIndex = 9;
        private static readonly Regex pattern
            = new Regex(@"(?<frac>([1-9][0-9]*(_[0-9]+)*|0)(\.[0-9]+(_[0-9]+)*)?|(\.[0-9]+(_[0-9]+)*))(?<prefix>[" + Prefixes + "])i");

        private static BinaryPrefixFormat _instance;
        public static BinaryPrefixFormat Instance => (_instance != null) ? _instance : (_instance = new BinaryPrefixFormat());

        public static string GetPrefixString(int prefixIndex) {
            return Prefixes[prefixIndex] + "i";
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
            prefixIndex = Prefixes.IndexOf(m.Groups["prefix"].Value);
            System.Diagnostics.Debug.Assert(prefixIndex >= 0);
        }

        private BinaryPrefixFormat() : base(TokenType.NumericLiteral, pattern, FormatPriority.Strong) { }

        protected override Val OnParse(Match m) {
            extractMatch(m, out var frac, out var prefixIndex);
            return new RealVal(frac * Math.Truncate((decimal)Math.Pow(2, prefixIndex * 10)), new FormatHint(this));
        }

        protected override string OnFormat(Val val, FormatSettings fs) {
            if (val is RealVal) {
                var r = val.AsDecimal;
                int prefixIndex = 0;
                if (r != 0) {
                    prefixIndex = (int)Math.Floor(DMath.Log2(Math.Abs(r), highAccuracy: true) / 10);
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
                    return RealFormat.RealToString(frac, fs, false);
                }
                else {
                    return RealFormat.RealToString(frac, fs, false) + GetPrefixString(prefixIndex);
                }
            }
            else {
                return base.OnFormat(val, fs);
            }
        }
    }
}

