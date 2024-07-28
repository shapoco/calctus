using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Maths;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    class Bit_ByteOperationFuncs : BuiltInFuncCategory {
        private static Bit_ByteOperationFuncs _instance = null;
        public static Bit_ByteOperationFuncs Instance => _instance != null ? _instance : _instance = new Bit_ByteOperationFuncs();
        private Bit_ByteOperationFuncs() { }

        public readonly BuiltInFuncDef pack = new BuiltInFuncDef("pack(b, array[]...)",
            "Packs array values at b-bit intervals.",
            (e, a) => {
                if (a[0] is ArrayVal) {
                    return LongMath.Pack(a[0].AsIntArray, a[1].AsLongArray).ToHexVal();
                }
                else {
                    return LongMath.Pack(a[0].AsInt, a[1].AsLongArray).ToHexVal();
                }
            });

        public readonly BuiltInFuncDef unpack_2 = new BuiltInFuncDef("unpack(array, x)",
            "Divide the value of `x` into `n` elements of `b` bit width.",
            (e, a) => LongMath.Unpack(a[0].AsIntArray, a[1].AsLong).ToArrayVal(a[1].FormatHint, null));

        public readonly BuiltInFuncDef unpack_3 = new BuiltInFuncDef("unpack(b, n, x)",
            "Separate the value of `x` into `n` elements of `b` bit width.",
            (e, a) => LongMath.Unpack(a[0].AsInt, a[1].AsInt, a[2].AsLong).ToArrayVal(a[2].FormatHint, null));

        public readonly BuiltInFuncDef swapNib = new BuiltInFuncDef("swapNib(*x@)",
            "Swaps the nibble of each byte of `x`.",
            FuncDef.ArgToLong((e, a) => LongMath.SwapNibbles(a[0])));

        public readonly BuiltInFuncDef swap2 = new BuiltInFuncDef("swap2(*x@)",
            "Swaps even and odd bytes of `x`.",
            FuncDef.ArgToLong((e, a) => LongMath.Swap2(a[0])));

        public readonly BuiltInFuncDef swap4 = new BuiltInFuncDef("swap4(*x@)",
            "Reverses the order of each 4 bytes of `x`.",
            FuncDef.ArgToLong((e, a) => LongMath.Swap4(a[0])));

        public readonly BuiltInFuncDef swap8 = new BuiltInFuncDef("swap8(*x@)",
            "Reverses the byte-order of `x`.",
            FuncDef.ArgToLong((e, a) => LongMath.Swap8(a[0])));

        public readonly BuiltInFuncDef reverseBits = new BuiltInFuncDef("reverseBits(b, *x@)",
            "Reverses the lower `b` bits of `x`.",
            FuncDef.ArgToLong((e, a) => LongMath.Reverse((int)a[0], a[1])));

        public readonly BuiltInFuncDef reverseBytes = new BuiltInFuncDef("reverseBytes(b, *x@)",
            "Reverses the lower `b` bytes of `x`.",
            FuncDef.ArgToLong((e, a) => LongMath.ReverseBytes((int)a[0], a[1])));

        public readonly BuiltInFuncDef rotateL_2 = new BuiltInFuncDef("rotateL(b, *x@)",
            "Rotates the lower `b` bits of `x` to the left by 1 bit.",
            FuncDef.ArgToLong((e, a) => LongMath.RotateLeft((int)a[0], 1, a[1])));

        public readonly BuiltInFuncDef rotateL_3 = new BuiltInFuncDef("rotateL(b, n, *x@)",
            "Rotates the lower `b` bits of `x` to the left by `n` bits.",
            FuncDef.ArgToLong((e, a) => LongMath.RotateLeft((int)a[0], (int)a[1], a[2])));

        public readonly BuiltInFuncDef rotateR_2 = new BuiltInFuncDef("rotateR(b, *x@)",
            "Rotates the lower `b` bits of `x` to the right by 1 bit.",
            FuncDef.ArgToLong((e, a) => LongMath.RotateRight((int)a[0], 1, a[1])));

        public readonly BuiltInFuncDef rotateR_3 = new BuiltInFuncDef("rotateR(b, n, *x@)",
            "Rotates the lower `b` bits of `x` to the right by `n` bits.",
            FuncDef.ArgToLong((e, a) => LongMath.RotateRight((int)a[0], (int)a[1], a[2])));

        public readonly BuiltInFuncDef count1 = new BuiltInFuncDef("count1(*x)",
            "Number of bits of `x` that have the value 1.",
            FuncDef.ArgToLong((e, a) => LongMath.CountOnes(a[0])));
    }
}
