using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Mathematics;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    static class Parity_EccFuncs {
        public static readonly BuiltInFuncDef xorReduce = new BuiltInFuncDef("xorReduce(*x)", (e, a) => new RealVal(LMath.XorReduce(a[0].AsLong)).FormatInt(), "Reduction XOR of `x` (Same as even parity).");
        public static readonly BuiltInFuncDef oddParity = new BuiltInFuncDef("oddParity(*x)", (e, a) => new RealVal(LMath.OddParity(a[0].AsLong)).FormatInt(), "Odd parity of `x`.");
        public static readonly BuiltInFuncDef eccWidth = new BuiltInFuncDef("eccWidth(*b)", (e, a) => new RealVal(LMath.EccWidth(a[0].AsInt)).FormatInt(), "Width of ECC for `b`-bit data.");
        public static readonly BuiltInFuncDef eccEnc = new BuiltInFuncDef("eccEnc(b, *x)", (e, a) => new RealVal(LMath.EccEncode(a[0].AsInt, a[1].AsLong)).FormatHex(), "Generates ECC code (`b`: data width, `x`: data).");
        public static readonly BuiltInFuncDef eccDec = new BuiltInFuncDef("eccDec(b, ecc, x)", (e, a) => new RealVal(LMath.EccDecode(a[0].AsInt, a[1].AsInt, a[2].AsLong)).FormatInt(), 
            "Checks ECC code (`b`: data width, `ecc`: ECC code, `x`: data). Returns: 0 = no error, positive value = position of 1-bit error, negative value = 2-bit error.");
    }
}
