using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model {
    enum VariadicMode {
        None,
        Array,
        Flatten,
    }

    enum OpType {
        None,
        Unary,
        Binary,
    }

    enum OpPriorityDir {
        Left,
        Right
    }

    enum Radix {
        Decimal = 0,
        Hexadecimal = 1,
        Binary = 2,
        Octal = 3,
    }

    static class RadixExtensions {
        public static int GetBaseNumber(this Radix radix) {
            switch (radix) {
                case Radix.Decimal: return 10;
                case Radix.Hexadecimal: return 16;
                case Radix.Binary: return 2;
                case Radix.Octal: return 8;
                default: throw new NotSupportedException();
            }
        }

        public static string GetCStylePrefix(this Radix radix) {
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
