using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.Maths.Types {
    // todo Delete
    [Obsolete("Deprecated", false)]
    struct ufixed113 {
        public static readonly ufixed113 Zero = new ufixed113(0u, 0u, 0u, 0u, 0u);
        public static readonly ufixed113 One = new ufixed113(1u, 0u, 0u, 0u, 0u);
        public const int NumBits = 113;

        private const int N = 5;
        private uint[] _words;

        public ufixed113(uint[] a, int offset = 0) {
            if (a.Length != N) throw new ArgumentException();
            _words = new uint[N];
            for (int i = 0; i < N; i++) {
                _words[i] = a[offset + i];
            }
        }

        public ufixed113(uint e, uint d, uint c, uint b, uint a) : this(new uint[] { a, b, c, d, e }) { }

        public uint Msb => _words[N - 1];

        public ulong Lower64Bits =>
            (ulong)_words[0] | ((ulong)_words[1] << 28) | ((ulong)_words[2] << 56);

        public override bool Equals(object obj) {
            if (obj is ufixed113 objB) {
                return this == objB;
            }
            else {
                return false;
            }
        }

        public override int GetHashCode() {
            uint hash = 0;
            for (int i = 0; i < N; i++) {
                hash ^= _words[i];
            }
            return (int)hash;
        }

        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append(Convert.ToString(_words[N - 1], 16));
            for (int i = N - 2; i >= 0; i--) {
                sb.Append(" ");
                var tmp = ("0000000" + Convert.ToString(_words[i], 16));
                sb.Append(tmp.Substring(tmp.Length - 7));
            }
            return sb.ToString();
        }

        public ufixed113 Add(ufixed113 b, out uint carryOut) {
            var a = this;
            var accum = new uint[N];
            uint tmp = 0;
            for (int i = 0; i < N; i++) {
                tmp += a._words[i] + b._words[i];
                accum[i] = tmp & 0xfffffffu;
                tmp >>= 28;
            }
            carryOut = (accum[N - 1] >> 1) & 1u;
            accum[N - 1] &= 1u;
            return new ufixed113(accum);
        }

        public ufixed113 Sub(ufixed113 b, out uint carryOut) {
            carryOut = this < b ? 1u : 0u;
            return Add(-b, out _);
        }

        public ufixed113 Mul(ufixed113 b, out uint carryOut) {
            var a = this;
            var accum = new ulong[2 * N];
            for (int i = 0; i < N; i++) {
                for (int j = 0; j < N; j++) {
                    accum[i + j] += (ulong)a._words[j] * (ulong)b._words[i];
                }
            }

            var carry = 0ul;
            for (int i = 0; i < 2 * N; i++) {
                accum[i] += carry;
                carry = accum[i] >> 28;
                accum[i] &= 0xfffffffu;
            }

            var w = new uint[N];
            for (int i = 0; i < N; i++) {
                w[i] = (uint)accum[N + i - 1];
            }

            carryOut = (w[N - 1] >> 1) & 1u;
            w[N - 1] &= 1u;
            return new ufixed113(w);
        }

        public ufixed113 Div(ufixed113 b, out ufixed113 q) {
            var a = this;
            if (b == Zero) throw new DivideByZeroException();
            a = a.Align(out int aShift);
            b = b.Align(out int bShift);
            int shiftRight = aShift - bShift;

            q = Zero;
            for (int i = 0; i < NumBits; i++) {
                if (a >= b) {
                    a -= b;
                    q = q.SingleShiftLeft(1u);
                }
                else {
                    q = q.SingleShiftLeft(0u);
                }
                a <<= 1;
            }

            if (shiftRight >= 0) {
                q = q.LogicShiftRight(shiftRight);
            }
            else {
                q <<= shiftRight;
            }

            return q;
        }

        public ufixed113 Align(out int shift) {
            var a = this;
            shift = 0;
            if (a != Zero) {
                while (a.Msb == 0u) {
                    a <<= 1;
                    shift++;
                }
            }
            return a;
        }

        public ufixed113 ArithInvert(out uint carry) {
            return (~this).Add(new ufixed113(0u, 0u, 0u, 0u, 1u), out carry);
        }

        public ufixed113 SingleShiftLeft(uint carry) {
            var w = (uint[])_words.Clone();
            uint msb;
            for (int i = 0; i < N - 1; i++) {
                msb = (w[i] >> 27) & 1u;
                w[i] = ((w[i] << 1) & 0xffffffeu) | (carry);
                carry = msb;
            }
            w[N - 1] = carry;
            return new ufixed113(w);
        }

        public ufixed113 SingleShiftRight(uint carry) {
            var w = (uint[])_words.Clone();
            uint lsb = w[N - 1] & 1u;
            w[N - 1] = carry;
            carry = lsb;
            for (int i = N - 2; i >= 0; i--) {
                lsb = w[i] & 1u; 
                w[i] = (carry << 27) | ((w[i] >> 1) & 0x7ffffffu);
                carry = lsb;
            }
            return new ufixed113(w);
        }

        public ufixed113 LogicShiftLeft(int n) {
            if (n <= 0) throw new ArgumentException();
            var a = this;
            for (int i = 0; i < n; i++) {
                a = a.SingleShiftLeft(0u);
            }
            return a;
        }

        public ufixed113 LogicShiftRight(int n) {
            if (n < 0) throw new ArgumentException();
            var a = this;
            for (int i = 0; i < n; i++) {
                a = a.SingleShiftRight(0u);
            }
            return a;
        }

        public ufixed113 ArithShiftRight(int n) {
            if (n < 0) throw new ArgumentException();
            uint msb = Msb;
            var a = this;
            for (int i = 0; i < n; i++) {
                a = a.SingleShiftRight(msb);
            }
            return a;
        }

        public ufixed113 TruncateRight(int n) {
            var w = (uint[])_words.Clone();
            int i = 0;
            while (n >= 28 && i < N) {
                w[i] = 0;
                i++;
                n -= 28;
            }
            if (n > 0 && i < N) {
                w[i] &= ~((1u << n) - 1u);
            }
            return new ufixed113(w);
        }

        public static implicit operator ufixed113(int i) {
            if (i == 0) return Zero;
            if (i == 1) return One;
            throw new OverflowException();
        }

        public static implicit operator ufixed113(double d) {
            if (d < 0 || d >= 2) throw new OverflowException();
            ufixed113 ret = Zero;
            for (int i = 0; i < NumBits; i++) {
                var tmp = (uint)Math.Floor(d);
                ret = ret.SingleShiftLeft(tmp);
                d -= tmp;
                d *= 2;
            }
            return ret;
        }

        public static ufixed113 operator ~(ufixed113 a) {
            var w = (uint[])a._words.Clone();
            for (int i = 0; i < N; i++) {
                w[i] ^= 0xfffffffu;
            }
            w[N - 1] &= 1u;
            return new ufixed113(w);
        }

        public static ufixed113 operator -(ufixed113 a) => a.ArithInvert(out _);

        public static ufixed113 operator +(ufixed113 a, ufixed113 b) => a.Add(b, out _);
        public static ufixed113 operator -(ufixed113 a, ufixed113 b) => a.Sub(b, out _);
        public static ufixed113 operator *(ufixed113 a, ufixed113 b) => a.Mul(b, out _);
        public static ufixed113 operator /(ufixed113 a, ufixed113 b) { a.Div(b, out ufixed113 q); return q; }

        public static ufixed113 operator <<(ufixed113 a, int n) => a.LogicShiftLeft(n);
        public static ufixed113 operator >>(ufixed113 a, int n) => a.ArithShiftRight(n);

        public static bool operator ==(ufixed113 a, ufixed113 b) {
            for (int i = 0; i < N; i++) {
                if (a._words[i] != b._words[i]) return false;
            }
            return true;
        }
        public static bool operator !=(ufixed113 a, ufixed113 b) => !(a == b);
        public static bool operator >(ufixed113 a, ufixed113 b) {
            for (int i = N - 1; i >= 0; i--) {
                if (a._words[i] > b._words[i]) return true;
                if (a._words[i] < b._words[i]) return false;
            }
            return false;
        }
        public static bool operator <(ufixed113 a, ufixed113 b) => b > a;
        public static bool operator >=(ufixed113 a, ufixed113 b) => (a > b) || (a == b);
        public static bool operator <=(ufixed113 a, ufixed113 b) => (a < b) || (a == b);

        public static void Test() {
            AssertAdd(0.5, 0.5, 1, 0u);
            AssertAdd(1, 1, 0, 1u);
            AssertSub(1, 1, 0, 0u);
        }

        public static void AssertAdd(double a, double b, double q, uint carry) {
            Assert.Equal("[" + nameof(ufixed113) + "] " + a + "+" + b, ((ufixed113)a).Add((ufixed113)b, out uint c), (ufixed113)q);
            Assert.Equal("[" + nameof(ufixed113) + "] carry(" + a + "+" + b + ")", carry, c);
        }

        public static void AssertSub(double a, double b, double q, uint carry) {
            Assert.Equal("[" + nameof(ufixed113) + "] " + a + "-" + b, ((ufixed113)a).Sub((ufixed113)b, out uint c), (ufixed113)q);
            Assert.Equal("[" + nameof(ufixed113) + "] carry(" + a + "-" + b + ")", carry, c);
        }

    }
}
