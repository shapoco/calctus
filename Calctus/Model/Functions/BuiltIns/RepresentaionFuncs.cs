using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Formats;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    class RepresentaionFuncs : BuiltInFuncCategory {
        private static RepresentaionFuncs _instance = null;
        public static RepresentaionFuncs Instance => _instance != null ? _instance : _instance = new RepresentaionFuncs();
        private RepresentaionFuncs() { }

        public readonly BuiltInFuncDef dec = new BuiltInFuncDef("dec(*x)",
            "Converts `x` to decimal representation.",
            (e, a) => a[0].Format(FormatHint.Default));

        public readonly BuiltInFuncDef hex = new BuiltInFuncDef("hex(*x)",
            "Converts `x` to hexdecimal representation.",
            (e, a) => a[0].Format(FormatHint.Hexadecimal));

        public readonly BuiltInFuncDef bin = new BuiltInFuncDef("bin(*x)",
            "Converts `x` to binary representation.",
            (e, a) => a[0].Format(FormatHint.Binary));

        public readonly BuiltInFuncDef oct = new BuiltInFuncDef("oct(*x)",
            "Converts `x` to octal representation.",
            (e, a) => a[0].Format(FormatHint.Octal));

        public readonly BuiltInFuncDef si = new BuiltInFuncDef("si(*x)",
            "Converts `x` to SI prefixed representation.",
            (e, a) => a[0].Format(FormatHint.SiPrefixed));

        public readonly BuiltInFuncDef kibi = new BuiltInFuncDef("kibi(*x)",
            "Converts `x` to binary prefixed representation.",
            (e, a) => a[0].Format(FormatHint.BinaryPrefixed));

        public readonly BuiltInFuncDef char_1 = new BuiltInFuncDef("char(*x)",
            "Converts `x` to character representation.",
            (e, a) => a[0].Format(FormatHint.Character));

        public readonly BuiltInFuncDef datetime = new BuiltInFuncDef("datetime(*x)",
            "Converts UNIX time `x` to datetime representation.",
            (e, a) => a[0].Format(FormatHint.DateTime));
    }
}
