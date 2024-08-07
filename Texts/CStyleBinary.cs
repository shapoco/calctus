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
                case Radix.Hexadecimal: return "0x";
                case Radix.Binary: return "0b";
                case Radix.Octal: return "0o";
                default: throw new NotSupportedException();
            }
        }
    }
}
