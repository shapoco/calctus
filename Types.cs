using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco {
    enum Radix {
        Decimal = 0,
        Hex = 1,
        Bin = 2,
        Oct = 3,
    }

    static class RadixExtensions {
        public static int ToBaseNumber(this Radix radix) {
            switch (radix) {
                case Radix.Decimal: return 10;
                case Radix.Hex: return 16;
                case Radix.Bin: return 2;
                case Radix.Oct: return 8;
                default: throw new NotSupportedException();
            }
        }

        public static int ToBinaryDigitBits(this Radix radix) {
            switch (radix) {
                case Radix.Decimal: throw new ArgumentException(radix.ToString() + " is not binary radix.", nameof(radix));
                case Radix.Hex: return 4;
                case Radix.Bin: return 1;
                case Radix.Oct: return 3;
                default: throw new NotSupportedException();
            }
        }
        
        public static string ToRustStylePrefix(this Radix radix) {
            switch (radix) {
                case Radix.Decimal: return "";
                case Radix.Hex: return "0x";
                case Radix.Bin: return "0b";
                case Radix.Oct: return "0o";
                default: throw new NotSupportedException();
            }
        }
    }
}
