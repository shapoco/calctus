using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Texts {
    static class CStyleBinary {
        public static string GetPrefix(Radix radix) {
            switch (radix) {
                case Radix.Decimal: return "";
                case Radix.Hex: return "0x";
                case Radix.Bin: return "0b";
                case Radix.Oct: return "0o";
                default: throw new NotSupportedException();
            }
        }

        public static char ToChar(int digit) {
            if (digit < 0 || 16 <= digit)
                throw Log.Here().ArgException(nameof(digit));
            else if (digit < 10)
                return (char)('0' + digit);
            else
                return (char)('a' - 10 + digit);
        }
    }
}
