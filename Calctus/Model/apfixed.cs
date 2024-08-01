#if false
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model {
    struct apfixed {
        private const int Stride = 32;
        public readonly bool Signed;
        public readonly int IWidth;
        public readonly int FWidth;
        public int Width => IWidth + FWidth;

        private uint[] _seg;

        public apfixed(bool signed, int iWidth, int fWidth, uint[] seg, int offsetBits) {
#if DEBUG
            System.Diagnostics.Debug.Assert(iWidth >= 0);
            System.Diagnostics.Debug.Assert(fWidth >= 0);
#endif
            this.Signed = signed;
            this.IWidth = iWidth;
            this.FWidth = fWidth;
            int width = iWidth + fWidth;
            int numSeg = DMath.CDiv(width, Stride);
            _seg = new uint[numSeg];

            int iSrc = offsetBits / Stride;
            int bSrc = 0;
            for (int i = 0; i < width; i++) {

            }

            for (int i = 0; i < N; i++) {
                _seg[i] = seg[offset + i];
            }
        }


        //public apfixed(uint e, uint d, uint c, uint b, uint a) : this(new uint[] { a, b, c, d, e }) { }
        //
        //public uint Msb => _seg[N - 1];
        //
        //public ulong Lower64Bits =>
        //    (ulong)_seg[0] | ((ulong)_seg[1] << 28) | ((ulong)_seg[2] << 56);
        //
        //public override bool Equals(object obj) {
        //    if (obj is apfixed objB) {
        //        return this == objB;
        //    }
        //    else {
        //        return false;
        //    }
        //}
        //
        //public override int GetHashCode() {
        //    uint hash = 0;
        //    for (int i = 0; i < N; i++) {
        //        hash ^= _seg[i];
        //    }
        //    return (int)hash;
        //}
        //
        //public override string ToString() {
        //    var sb = new StringBuilder();
        //    sb.Append(Convert.ToString(_seg[N - 1], 16));
        //    for (int i = N - 2; i >= 0; i--) {
        //        sb.Append(" ");
        //        var tmp = ("0000000" + Convert.ToString(_seg[i], 16));
        //        sb.Append(tmp.Substring(tmp.Length - 7));
        //    }
        //    return sb.ToString();
        //}
        //
        //public apfixed Add(apfixed b, out uint carryOut) {
        //    var a = this;
        //    var accum = new uint[N];
        //    uint tmp = 0;
        //    for (int i = 0; i < N; i++) {
        //        tmp += a._seg[i] + b._seg[i];
        //        accum[i] = tmp & 0xfffffffu;
        //        tmp >>= 28;
        //    }
        //    carryOut = (accum[N - 1] >> 1) & 1u;
        //    accum[N - 1] &= 1u;
        //    return new apfixed(accum);
        //}
        //
        //public apfixed Sub(apfixed b, out uint carryOut) {
        //    carryOut = this < b ? 1u : 0u;
        //    return Add(-b, out _);
        //}
        //
        //public apfixed Mul(apfixed b, out uint carryOut) {
        //    var a = this;
        //    var accum = new ulong[2 * N];
        //    for (int i = 0; i < N; i++) {
        //        for (int j = 0; j < N; j++) {
        //            accum[i + j] += (ulong)a._seg[j] * (ulong)b._seg[i];
        //        }
        //    }
        //
        //    var carry = 0ul;
        //    for (int i = 0; i < 2 * N; i++) {
        //        accum[i] += carry;
        //        carry = accum[i] >> 28;
        //        accum[i] &= 0xfffffffu;
        //    }
        //
        //    var w = new uint[N];
        //    for (int i = 0; i < N; i++) {
        //        w[i] = (uint)accum[N + i - 1];
        //    }
        //
        //    carryOut = (w[N - 1] >> 1) & 1u;
        //    w[N - 1] &= 1u;
        //    return new apfixed(w);
        //}
        //
        //public apfixed Div(apfixed b, out apfixed q) {
        //    var a = this;
        //    if (b == Zero) throw new DivideByZeroException();
        //    a = a.Align(out int aShift);
        //    b = b.Align(out int bShift);
        //    int shiftRight = aShift - bShift;
        //
        //    q = Zero;
        //    for (int i = 0; i < NumBits; i++) {
        //        if (a >= b) {
        //            a -= b;
        //            q = q.SingleShiftLeft(1u);
        //        }
        //        else {
        //            q = q.SingleShiftLeft(0u);
        //        }
        //        a <<= 1;
        //    }
        //
        //    if (shiftRight >= 0) {
        //        q = q.LogicShiftRight(shiftRight);
        //    }
        //    else {
        //        q <<= shiftRight;
        //    }
        //
        //    return q;
        //}
        //
        //public apfixed Align(out int shift) {
        //    var a = this;
        //    shift = 0;
        //    if (a != Zero) {
        //        while (a.Msb == 0u) {
        //            a <<= 1;
        //            shift++;
        //        }
        //    }
        //    return a;
        //}
        //
        //public apfixed ArithInvert(out uint carry) {
        //    return (~this).Add(new apfixed(0u, 0u, 0u, 0u, 1u), out carry);
        //}
        //
        //public apfixed SingleShiftLeft(uint carry) {
        //    var w = (uint[])_seg.Clone();
        //    uint msb;
        //    for (int i = 0; i < N - 1; i++) {
        //        msb = (w[i] >> 27) & 1u;
        //        w[i] = ((w[i] << 1) & 0xffffffeu) | (carry);
        //        carry = msb;
        //    }
        //    w[N - 1] = carry;
        //    return new apfixed(w);
        //}
        //
        //public apfixed SingleShiftRight(uint carry) {
        //    var w = (uint[])_seg.Clone();
        //    uint lsb = w[N - 1] & 1u;
        //    w[N - 1] = carry;
        //    carry = lsb;
        //    for (int i = N - 2; i >= 0; i--) {
        //        lsb = w[i] & 1u;
        //        w[i] = (carry << 27) | ((w[i] >> 1) & 0x7ffffffu);
        //        carry = lsb;
        //    }
        //    return new apfixed(w);
        //}
        //
        //public apfixed LogicShiftLeft(int n) {
        //    if (n <= 0) throw new ArgumentException();
        //    var a = this;
        //    for (int i = 0; i < n; i++) {
        //        a = a.SingleShiftLeft(0u);
        //    }
        //    return a;
        //}
        //
        //public apfixed LogicShiftRight(int n) {
        //    if (n < 0) throw new ArgumentException();
        //    var a = this;
        //    for (int i = 0; i < n; i++) {
        //        a = a.SingleShiftRight(0u);
        //    }
        //    return a;
        //}
        //
        //public apfixed ArithShiftRight(int n) {
        //    if (n < 0) throw new ArgumentException();
        //    uint msb = Msb;
        //    var a = this;
        //    for (int i = 0; i < n; i++) {
        //        a = a.SingleShiftRight(msb);
        //    }
        //    return a;
        //}
        //
        //public apfixed TruncateRight(int n) {
        //    var w = (uint[])_seg.Clone();
        //    int i = 0;
        //    while (n >= 28 && i < N) {
        //        w[i] = 0;
        //        i++;
        //        n -= 28;
        //    }
        //    if (n > 0 && i < N) {
        //        w[i] &= ~((1u << n) - 1u);
        //    }
        //    return new apfixed(w);
        //}
        //
        //public static implicit operator apfixed(int i) {
        //    if (i == 0) return Zero;
        //    if (i == 1) return One;
        //    throw new OverflowException();
        //}
        //
        //public static implicit operator apfixed(double d) {
        //    if (d < 0 || d >= 2) throw new OverflowException();
        //    apfixed ret = Zero;
        //    for (int i = 0; i < NumBits; i++) {
        //        var tmp = (uint)Math.Floor(d);
        //        ret = ret.SingleShiftLeft(tmp);
        //        d -= tmp;
        //        d *= 2;
        //    }
        //    return ret;
        //}
        //
        //public static apfixed operator ~(apfixed a) {
        //    var w = (uint[])a._seg.Clone();
        //    for (int i = 0; i < N; i++) {
        //        w[i] ^= 0xfffffffu;
        //    }
        //    w[N - 1] &= 1u;
        //    return new apfixed(w);
        //}
        //
        //public static apfixed operator -(apfixed a) => a.ArithInvert(out _);
        //
        //public static apfixed operator +(apfixed a, apfixed b) => a.Add(b, out _);
        //public static apfixed operator -(apfixed a, apfixed b) => a.Sub(b, out _);
        //public static apfixed operator *(apfixed a, apfixed b) => a.Mul(b, out _);
        //public static apfixed operator /(apfixed a, apfixed b) { a.Div(b, out apfixed q); return q; }
        //
        //public static apfixed operator <<(apfixed a, int n) => a.LogicShiftLeft(n);
        //public static apfixed operator >>(apfixed a, int n) => a.ArithShiftRight(n);
        //
        //public static bool operator ==(apfixed a, apfixed b) {
        //    for (int i = 0; i < N; i++) {
        //        if (a._seg[i] != b._seg[i]) return false;
        //    }
        //    return true;
        //}
        //public static bool operator !=(apfixed a, apfixed b) => !(a == b);
        //public static bool operator >(apfixed a, apfixed b) {
        //    for (int i = N - 1; i >= 0; i--) {
        //        if (a._seg[i] > b._seg[i]) return true;
        //        if (a._seg[i] < b._seg[i]) return false;
        //    }
        //    return false;
        //}
        //public static bool operator <(apfixed a, apfixed b) => b > a;
        //public static bool operator >=(apfixed a, apfixed b) => (a > b) || (a == b);
        //public static bool operator <=(apfixed a, apfixed b) => (a < b) || (a == b);
        //
        //public static void Test() {
        //    AssertAdd(0.5, 0.5, 1, 0u);
        //    AssertAdd(1, 1, 0, 1u);
        //    AssertSub(1, 1, 0, 0u);
        //}
        //
        //public static void AssertAdd(double a, double b, double q, uint carry) {
        //    Assert.Equal("[" + nameof(apfixed) + "] " + a + "+" + b, ((apfixed)a).Add((apfixed)b, out uint c), (apfixed)q);
        //    Assert.Equal("[" + nameof(apfixed) + "] carry(" + a + "+" + b + ")", carry, c);
        //}
        //
        //public static void AssertSub(double a, double b, double q, uint carry) {
        //    Assert.Equal("[" + nameof(apfixed) + "] " + a + "-" + b, ((apfixed)a).Sub((apfixed)b, out uint c), (apfixed)q);
        //    Assert.Equal("[" + nameof(apfixed) + "] carry(" + a + "-" + b + ")", carry, c);
        //}
    }
}
#endif
