using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.Parsers {
    static class NumberLexer {
        public static NumberSequence Expect(StringReaderDep sr, Radix radix, bool allowUnderscore) {
            if (TryParse(sr, radix, allowUnderscore, out NumberSequence num)) {
                return num;
            }
            else {
                throw sr.ExpectFailed("Number sequence");
            }
        }

        public static bool TryParse(StringReaderDep sr, Radix radix, bool allowUnderscore, out NumberSequence num) {
            DeprecatedTextPosition pos = sr.Position;
            if (TryParseChar(sr, radix, out char c)) {
                num = new NumberSequence(radix, pos);
                num.Append(c);
                ReadFollowing(sr, num, allowUnderscore);
                return true;
            }
            else {
                num = null;
                return false;
            }
        }

        public static char ExpectChar(StringReaderDep sr, Radix radix) {
            if (TryParseChar(sr, radix, out char c)) {
                return c;
            }
            else { 
                throw sr.ExpectFailed("Number");
            }
        }

        public static bool TryParseChar(StringReaderDep sr, Radix radix, out char c) {
            bool hit;
            switch (radix) {
                case Radix.Decimal: hit = sr.ReadIf('0', '9', out c); break;
                case Radix.Hexadecimal: hit = sr.ReadIf('0', '9', out c) || sr.ReadIf('a', 'f', out c) || sr.ReadIf('A', 'F', out c); break;
                case Radix.Binary: hit = sr.ReadIf('0', '1', out c); break;
                case Radix.Octal: hit = sr.ReadIf('0', '7', out c); break;
                default: throw new NotImplementedException();
            }
            return hit;
        }

        public static void ReadFollowing(StringReaderDep sr, NumberSequence num, bool allowUnderscore) {
            var radix = (num.Radix == Radix.Hexadecimal) ? Radix.Hexadecimal : Radix.Decimal;
            char c;
            while (true) {
                if (allowUnderscore && sr.ReadIf('_')) {
                    num.Append(ExpectChar(sr, radix));
                }
                else if (TryParseChar(sr, radix, out c)) {
                    num.Append(c);
                }
                else {
                    break;
                }
            }
        }
    }
}
