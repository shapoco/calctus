using System;
using System.Text.RegularExpressions;
using Shapoco.Texts;
using Shapoco.Maths;
using Shapoco.Calctus.Model.Formats;

namespace Shapoco.Calctus.Model.Standards {
    static class SiPrefix {
        public static readonly char[] Chars = new char[] {
            'r', 'y', 'z', 'a', 'f', 'p', 'n', 'u', 'm', '_',
            'k', 'M', 'G', 'T', 'P', 'E', 'Z', 'Y', 'R'
        };
        public static readonly Regex Regex
            = new Regex(@"(?<frac>(\d+(_\d+)*)(\.\d+(_\d+)*)?|(\.\d+(_\d+)*))(?<prefix>[" + String.Join("", Chars) + "])");
            
        public const int IndexOffset = 9;
        public const int MinExp = -IndexOffset;
        public const int MaxExp = IndexOffset;

        public static bool TryCharToExp(char prefix, out int exp) {
            for (int i = 0; i < Chars.Length; i++) {
                if (Chars[i] == prefix) {
                    exp = i - IndexOffset;
                    return true;
                }
            }
            exp = MinExp - 1;
            return false;
        }

        public static int CharToExp(char prefix) {
            if (!TryCharToExp(prefix, out int exp)) {
                throw new CalctusArgError(nameof(CharToExp), "Invalid SI prefix: " + CStyleEscaping.EscapeAndQuote(prefix) + ".");
            }
            return exp;
        }

        public static char GetPrefixChar(int expDiv3) => Chars[expDiv3 + IndexOffset];

        public static string ToString(decimal val, ToStringArgs args) {
            int prefixIndex = 0;
            if (val != 0) {
                prefixIndex = (int)Math.Floor(MathEx.Log10(Math.Abs(val)) / 3);
            }
            prefixIndex = MathEx.Clip(MinExp, MaxExp, prefixIndex);
            var exp = prefixIndex * 3;
            var frac = val / MathEx.Pow10(exp);
            if (prefixIndex == 0) {
                return Formatter.DecimalToCStyleDecimalLiteral(frac, args, false);
            }
            else {
                return Formatter.DecimalToCStyleDecimalLiteral(frac, args, false) + GetPrefixChar(prefixIndex);
            }
        }

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
                catch(Exception ex) {
                    Log.Here().I(ex);
                    return false;
                }
            }
            else {
                return false;
            }
        }
    }
}
