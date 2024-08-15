using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Shapoco.Maths {
    static class DecMath {
        public static readonly Regex Pattern = new Regex(@"^(?<frac>-?([1-9][0-9]*(_[0-9]+)*|0)*(\.[0-9]+(_[0-9]+)*)?)(?<exppart>(?<echar>e|E)(?<exp>[+\-]?[0-9]+(_[0-9]+)*))?$");
        public const decimal PI = 3.1415926535897932384626433833m;
        public const decimal E = 2.7182818284590452353602874714m;
        public const decimal LogE2 = 0.6931471805599453094172321215m;
        public const decimal LogE10 = 2.3025850929940456840179914547m;

        public static bool TryParse(string str, out decimal frac, out char eChar, out int exp) {
            frac = 0;
            eChar = '\0';
            exp = 0;

            var m = Pattern.Match(str);
            if (!m.Success) {
                return false;
            }
            frac = decimal.Parse(m.Groups["frac"].Value.Replace("_", ""), CultureInfo.InvariantCulture);
            if (m.Groups["exppart"].Success) {
                eChar = m.Groups["echar"].Value[0];
                exp = int.Parse(m.Groups["exp"].Value.Replace("_", ""), CultureInfo.InvariantCulture);
            }
            return true;
        }

        public static void Parse(string str, out decimal frac, out char eChar, out int exp) {
            if (!TryParse(str, out frac, out eChar, out exp)) {
                throw new Exception("Invalid number format.");
            }
        }

        public static decimal Parse(string str) {
            Parse(str, out decimal frac, out _, out int exp);
            if (exp >= 0) {
                return frac * Math.Round((decimal)Math.Pow(10, exp));
            }
            else {
                return frac / Math.Round((decimal)Math.Pow(10, -exp));
            }
        }

        public static Int64 ToInt64(this decimal val, CastOptions opts = CastOptions.Strict)
            => (Int64)roundClip(val, opts, Int64.MinValue, Int64.MaxValue);

        public static Int32 ToInt32(this decimal val, CastOptions opts = CastOptions.Strict)
            => (Int32)roundClip(val, opts, Int32.MinValue, Int32.MaxValue);

        public static char ToChar(this decimal val, CastOptions opts = CastOptions.Strict)
            => (char)roundClip(val, opts, char.MinValue, char.MaxValue);

        public static byte ToByte(this decimal val, CastOptions opts = CastOptions.Strict) 
            =>(byte)roundClip(val, opts, byte.MinValue, byte.MaxValue);

        private static decimal roundClip(decimal val, CastOptions opts, decimal min, decimal max) {
            if (!opts.HasFlag(CastOptions.AllowDegrade) && !val.IsInteger()) {
                throw Log.Here().E(new InvalidCastException());
            }

            if (opts.HasFlag(CastOptions.Round)) {
                val = Math.Round(val);
            }
            else if (opts.HasFlag(CastOptions.Floor)) {
                val = Math.Floor(val);
            }
            else {
                val = Math.Truncate(val);
            }

            if (val < min || max < val) {
                if (!opts.HasFlag(CastOptions.AllowOverflow)) {
                    throw Log.Here().E(new OverflowException());
                }
                else if (opts.HasFlag(CastOptions.Clip)) {
                    val = MathEx.Clip(min, max, val);
                }
            }

            return val;
        }
    }

}
