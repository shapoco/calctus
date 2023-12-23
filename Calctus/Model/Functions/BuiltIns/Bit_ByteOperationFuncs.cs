using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Mathematics;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    static class Bit_ByteOperationFuncs {
        public static readonly BuiltInFuncDef pack = new BuiltInFuncDef("pack(b, array[]...)", (e, a) => new RealVal(LMath.Pack(a[0].AsInt, a[1].AsLongArray)).FormatHex(), "Packs array values at b-bit intervals.");
        public static readonly BuiltInFuncDef unpack = new BuiltInFuncDef("unpack(b, x)", (e, a) => new ArrayVal(LMath.Unpack(a[0].AsInt, a[1].AsLong)).FormatInt(), "Returns an array of `x` values divided into `b` bits.");
        public static readonly BuiltInFuncDef swapNib = new BuiltInFuncDef("swapNib(*x)", (e, a) => new RealVal(LMath.SwapNibbles(a[0].AsLong), a[0].FormatHint), "Swaps the nibble of each byte of `x`.");
        public static readonly BuiltInFuncDef swap2 = new BuiltInFuncDef("swap2(*x)", (e, a) => new RealVal(LMath.Swap2(a[0].AsLong), a[0].FormatHint), "Swaps even and odd bytes of `x`.");
        public static readonly BuiltInFuncDef swap4 = new BuiltInFuncDef("swap4(*x)", (e, a) => new RealVal(LMath.Swap4(a[0].AsLong), a[0].FormatHint), "Reverses the order of each 4 bytes of `x`.");
        public static readonly BuiltInFuncDef swap8 = new BuiltInFuncDef("swap8(*x)", (e, a) => new RealVal(LMath.Swap8(a[0].AsLong), a[0].FormatHint), "Reverses the order of each 8 bytes of `x`.");
        public static readonly BuiltInFuncDef reverseBits = new BuiltInFuncDef("reverseBits(b, *x)", (e, a) => new RealVal(LMath.Reverse(a[0].AsInt, a[1].AsLong), a[1].FormatHint), "Reverses the lower `b` bits of `x`.");
        public static readonly BuiltInFuncDef reverseBytewise = new BuiltInFuncDef("reverseBytewise(*x)", (e, a) => new RealVal(LMath.ReverseBytes(a[0].AsLong), a[0].FormatHint), "Reverses the order of bits of each byte of `x`.");
        public static readonly BuiltInFuncDef rotateL = new BuiltInFuncDef("rotateL(b, *x)", (e, a) => new RealVal(LMath.RotateLeft(a[0].AsInt, a[1].AsLong), a[1].FormatHint), "Rotates left the lower `b` bits of `x`.");
        public static readonly BuiltInFuncDef rotateR = new BuiltInFuncDef("rotateR(b, *x)", (e, a) => new RealVal(LMath.RotateRight(a[0].AsInt, a[1].AsLong), a[1].FormatHint), "Rotates right the lower `b` bits of `x`.");
        public static readonly BuiltInFuncDef count1 = new BuiltInFuncDef("count1(*x)", (e, a) => new RealVal(LMath.CountOnes(a[0].AsLong)).FormatInt(), "Number of bits of `x` that have the value 1.");
    }
}
