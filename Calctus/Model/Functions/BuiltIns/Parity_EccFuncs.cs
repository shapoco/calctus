using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Maths;
using Shapoco.Calctus.Model.Formats;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    class Parity_EccFuncs : BuiltInFuncCategory {
        private static Parity_EccFuncs _instance = null;
        public static Parity_EccFuncs Instance => _instance != null ? _instance : _instance = new Parity_EccFuncs();
        private Parity_EccFuncs() { }

        public readonly BuiltInFuncDef xorReduce = new BuiltInFuncDef("xorReduce(*x)",
                "Reduction XOR of `x` (Same as even parity).",
                FuncDef.ArgToLong((e, a) => LongMath.XorReduce(a[0])));

        public readonly BuiltInFuncDef oddParity = new BuiltInFuncDef("oddParity(*x)",
            "Odd parity of `x`.",
            FuncDef.ArgToLong((e, a) => LongMath.OddParity(a[0])));

        public readonly BuiltInFuncDef eccWidth = new BuiltInFuncDef("eccWidth(*b)",
            "Width of ECC for `b`-bit data.",
            FuncDef.ArgToLong((e, a) => LongMath.EccWidth((int)a[0])));

        public readonly BuiltInFuncDef eccEnc = new BuiltInFuncDef("eccEnc(b, *x@)",
            "Generates ECC code (`b`: data width, `x`: data).",
            FuncDef.ArgToLong((e, a) => LongMath.EccEncode((int)a[0], a[1])));

        public readonly BuiltInFuncDef eccDec = new BuiltInFuncDef("eccDec(b, ecc, x)",
            "Checks ECC code (`b`: data width, `ecc`: ECC code, `x`: data). " +
            "Returns: 0 = no error, positive value = position of 1-bit error, negative value = 2-bit error.",
            FuncDef.ArgToLong((e, a) => LongMath.EccDecode((int)a[0], (int)a[1], a[2])));
    }
}
