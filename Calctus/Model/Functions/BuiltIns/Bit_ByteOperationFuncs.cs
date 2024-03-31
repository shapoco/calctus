using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Mathematics;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    static class Bit_ByteOperationFuncs {
        public static readonly BuiltInFuncDef pack = new BuiltInFuncDef("pack(b, array[]...)", (e, a) => {
            if (a[0] is ArrayVal) {
                return new RealVal(LMath.Pack(a[0].AsIntArray, a[1].AsLongArray)).FormatHex();
            }
            else {
                return new RealVal(LMath.Pack(a[0].AsInt, a[1].AsLongArray)).FormatHex();
            }
        }, "Packs array values at b-bit intervals.");
        public static readonly BuiltInFuncDef unpack_2 = new BuiltInFuncDef("unpack(array, x)", (e, a) => new ArrayVal(LMath.Unpack(a[0].AsIntArray, a[1].AsLong)).FormatInt(), "Divide the value of `x` into `n` elements of `b` bit width.");
        public static readonly BuiltInFuncDef unpack_3 = new BuiltInFuncDef("unpack(b, n, x)", (e, a) => new ArrayVal(LMath.Unpack(a[0].AsInt, a[1].AsInt, a[2].AsLong)).FormatInt(), "Separate the value of `x` into `n` elements of `b` bit width.");
        public static readonly BuiltInFuncDef swapNib = new BuiltInFuncDef("swapNib(*x)", (e, a) => new RealVal(LMath.SwapNibbles(a[0].AsLong), a[0].FormatHint), "Swaps the nibble of each byte of `x`.");
        public static readonly BuiltInFuncDef swap2 = new BuiltInFuncDef("swap2(*x)", (e, a) => new RealVal(LMath.Swap2(a[0].AsLong), a[0].FormatHint), "Swaps even and odd bytes of `x`.");
        public static readonly BuiltInFuncDef swap4 = new BuiltInFuncDef("swap4(*x)", (e, a) => new RealVal(LMath.Swap4(a[0].AsLong), a[0].FormatHint), "Reverses the order of each 4 bytes of `x`.");
        public static readonly BuiltInFuncDef swap8 = new BuiltInFuncDef("swap8(*x)", (e, a) => new RealVal(LMath.Swap8(a[0].AsLong), a[0].FormatHint), "Reverses the byte-order of `x`.");
        public static readonly BuiltInFuncDef reverseBits = new BuiltInFuncDef("reverseBits(b, *x)", (e, a) => new RealVal(LMath.Reverse(a[0].AsInt, a[1].AsLong), a[1].FormatHint), "Reverses the lower `b` bits of `x`.");
        public static readonly BuiltInFuncDef reverseBytes = new BuiltInFuncDef("reverseBytes(b, *x)", (e, a) => new RealVal(LMath.ReverseBytes(a[0].AsInt, a[1].AsLong), a[1].FormatHint), "Reverses the lower `b` bytes of `x`.");
        public static readonly BuiltInFuncDef rotateL_2 = new BuiltInFuncDef("rotateL(b, *x)", (e, a) => new RealVal(LMath.RotateLeft(a[0].AsInt, 1, a[1].AsLong), a[1].FormatHint), "Rotates the lower `b` bits of `x` to the left by 1 bit.");
        public static readonly BuiltInFuncDef rotateL_3 = new BuiltInFuncDef("rotateL(b, n, *x)", (e, a) => new RealVal(LMath.RotateLeft(a[0].AsInt, a[1].AsInt, a[2].AsLong), a[2].FormatHint), "Rotates the lower `b` bits of `x` to the left by `n` bits.");
        public static readonly BuiltInFuncDef rotateR_2 = new BuiltInFuncDef("rotateR(b, *x)", (e, a) => new RealVal(LMath.RotateRight(a[0].AsInt, 1, a[1].AsLong), a[1].FormatHint), "Rotates the lower `b` bits of `x` to the right by 1 bit.");
        public static readonly BuiltInFuncDef rotateR_3 = new BuiltInFuncDef("rotateR(b, n, *x)", (e, a) => new RealVal(LMath.RotateRight(a[0].AsInt, a[1].AsInt, a[2].AsLong), a[2].FormatHint), "Rotates the lower `b` bits of `x` to the right by `n` bits.");
        public static readonly BuiltInFuncDef count1 = new BuiltInFuncDef("count1(*x)", (e, a) => new RealVal(LMath.CountOnes(a[0].AsLong)).FormatInt(), "Number of bits of `x` that have the value 1.");
    }
}
