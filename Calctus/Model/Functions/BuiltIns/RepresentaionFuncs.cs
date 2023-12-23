using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    internal class RepresentaionFuncs {
        public static readonly BuiltInFuncDef dec = new BuiltInFuncDef("dec(*x)", (e, a) => a[0].FormatInt(), "Converts `x` to decimal representation.");
        public static readonly BuiltInFuncDef hex = new BuiltInFuncDef("hex(*x)", (e, a) => a[0].FormatHex(), "Converts `x` to hexdecimal representation.");
        public static readonly BuiltInFuncDef bin = new BuiltInFuncDef("bin(*x)", (e, a) => a[0].FormatBin(), "Converts `x` to binary representation.");
        public static readonly BuiltInFuncDef oct = new BuiltInFuncDef("oct(*x)", (e, a) => a[0].FormatOct(), "Converts `x` to octal representation.");
        public static readonly BuiltInFuncDef si = new BuiltInFuncDef("si(*x)", (e, a) => a[0].FormatSiPrefix(), "Converts `x` to SI prefixed representation.");
        public static readonly BuiltInFuncDef kibi = new BuiltInFuncDef("kibi(*x)", (e, a) => a[0].FormatBinaryPrefix(), "Converts `x` to binary prefixed representation.");
        public static readonly BuiltInFuncDef char_1 = new BuiltInFuncDef("char(*x)", (e, a) => a[0].FormatChar(), "Converts `x` to character representation.");
        public static readonly BuiltInFuncDef datetime = new BuiltInFuncDef("datetime(*x)", (e, a) => a[0].FormatDateTime(), "Converts `x` to datetime representation.");
    }
}
