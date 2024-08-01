using System;
using System.Text.RegularExpressions;
using Shapoco.Calctus.Model.Maths;
using Shapoco.Calctus.Model.Formats;

namespace Shapoco.Calctus.Model.Standards {
    static class BinaryPrefix {
        public static readonly char[] Chars = new char[] {
            '_', 'k', 'M', 'G', 'T', 'P', 'E', 'Z', 'Y', 'R'
        };
        public static readonly Regex Regex
            = new Regex(@"(?<frac>(\d+(_\d+)*)(\.\d+(_\d+)*)?|(\.\d+(_\d+)*))(?<prefix>[" + String.Join("", Chars) + "])i");

        public const int MinExp = 0;
        public const int MaxExp = 9;

        public static bool TryCharToExp(int prefix, out int exp) {
            for (int i = 0; i < Chars.Length; i++) {
                if (Chars[i] == prefix) {
                    exp = i;
                    return true;
                }
            }
            exp = MinExp - 1;
            return false;
        }

        public static int CharToExp(int prefix) {
            if (!TryCharToExp(prefix, out int exp)) {
                throw new CalctusArgError(nameof(CharToExp), "Invalid SI prefix: " + CalctusUtils.ToString(prefix) + ".");
            }
            return exp;
        }

        public static string ToString(decimal val, ToStringArgs args) {
            int prefixIndex = 0;
            if (val != 0) {
                prefixIndex = (int)Math.Floor(MathEx.Log2(Math.Abs(val), highAccuracy: true) / 10);
            }
            prefixIndex = MathEx.Clip(MinExp, MaxExp, prefixIndex);
            var exp = prefixIndex * 10;
            var frac = val / Math.Truncate((decimal)Math.Pow(2, exp));
            if (prefixIndex == 0) {
                return Formatter.DecimalToCStyleDecimalLiteral(frac, args, false);
            }
            else {
                return Formatter.DecimalToCStyleDecimalLiteral(frac, args, false) + GetPrefixString(prefixIndex);
            }
        }

        public static string GetPrefixString(int prefixIndex) => Chars[prefixIndex] + "i";

        public static bool TryParse(string str, out decimal frac, out int prefixIndex) {
            frac = 0;
            prefixIndex = 0;
            var m = Regex.Match(str);
            if (m.Success && m.Index == 0 && m.Length == str.Length) {
                try {
                    frac = DecMath.Parse(m.Groups["frac"].Value);
                    prefixIndex = CharToExp(m.Groups["prefix"].Value[0]);
                    return true;
                }
                catch (Exception ex) {
                    Console.WriteLine("*WARNING: " + nameof(BinaryPrefix) + "." + nameof(TryParse) + "(): " + ex.Message);
                    return false;
                }
            }
            else {
                return false;
            }
        }
    }
}
