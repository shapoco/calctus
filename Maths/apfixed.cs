using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Maths.BitArrays;

namespace Shapoco.Maths {
    using word = UInt32;
    struct apfixed : IComparable<apfixed> {
        public const int MaxWidth = 1024;

        private const int S = sizeof(word) * 8;
        public readonly bool IsSigned;

        private readonly int fp;

        public readonly BitArray Bits;
        public int IntWidth => Bits.Width - fp;
        public int SignWidth => IsSigned ? 1 : 0;
        public int FracWidth => fp;
        public int Width => Bits.Width;
        public int IntWidthWithoutSign => Bits.Width - fp - SignWidth;
        public int WidthWithoutSign => Bits.Width - SignWidth;

        public FixedPointFormat Format => new FixedPointFormat(IsSigned, Bits.Width, FracWidth);

        public static apfixed FromBinaryDigits(int radix, FixedPointFormat fmt, byte[] intDigits, byte[] fracDigits) {
            int digitWidth = binaryRadixToWidth(radix);

            var work = BitArray.CreateSegmentArray(fmt.Width);
            int pos = fmt.FracWidth;
            for (int i = intDigits.Length - 1; i >= 0; i--) {
                byte digit = intDigits[i];
                for (int j = 0; j < digitWidth; j++) {
                    work.SetBit(pos++, digit & 1u);
                    digit >>= 1;
                    if (pos >= fmt.Width) break;
                }
                if (pos >= fmt.Width) break;
            }
            if (fmt.Signed) work.ExtendSign(fmt.Width - 1);

            pos = fmt.FracWidth;
            for (int i = 0; i < fracDigits.Length; i++) {
                byte digit = fracDigits[i];
                for (int j = 0; j < digitWidth; j++) {
                    digit <<= 1;
                    work.SetBit(--pos, (uint)(digit >> digitWidth) & 1u);
                    if (pos <= 0) break;
                }
                if (pos <= 0) break;
            }

            return new apfixed(fmt, work, false);
        }

        public static apfixed FromBinaryDigits(int radix, FixedPointFormat fmt, byte[] digits) {
            int digitWidth = binaryRadixToWidth(radix);

            var work = BitArray.CreateSegmentArray(fmt.Width);
            int pos = 0;
            for (int i = digits.Length - 1; i >= 0; i--) {
                byte digit = digits[i];
                for (int j = 0; j < digitWidth; j++) {
                    work.SetBit(pos++, digit & 1u);
                    digit >>= 1;
                    if (pos >= fmt.Width) break;
                }
                if (pos >= fmt.Width) break;
            }
            if (fmt.Signed) work.ExtendSign(fmt.Width - 1);

            return new apfixed(fmt, work, false);
        }

        public void ToBinaryStringWithPoint(int radix, StringBuilder digits) {
            int digitWidth = binaryRadixToWidth(radix);

            var w = Width;
            var iw = IntWidth;
            var fw = FracWidth;

            if (iw == 0) {
                digits.Append('0');
            }
            else {
                int shift = (iw + digitWidth - 1) % digitWidth;
                int n = iw;
                word digit = 0u;
                foreach (var bit in Bits.EnumBits(w - 1, -iw)) {
                    digit |= bit << shift;
                    n -= 1;
                    if (shift-- <= 0 || n == 0) {
                        digits.Append(digitToChar(digit));
                        shift = digitWidth - 1;
                        digit = 0u;
                    }
                }
            }

            digits.Append('.');

            if (fw == 0) {
                digits.Append('0');
            }
            else {
                int shift = digitWidth - 1;
                int n = fw;
                word digit = 0u;
                foreach (var bit in Bits.EnumBits(fw - 1, -fw)) {
                    digit |= bit << shift;
                    n -= 1;
                    if (shift-- <= 0 || n == 0) {
                        digits.Append(digitToChar(digit));
                        shift = digitWidth - 1;
                        digit = 0u;
                    }
                }
            }
        }

        public void ToRawBinaryString(int radix, StringBuilder digits) {
            int digitWidth = binaryRadixToWidth(radix);
            int shift = 0;
            int n = Width;
            word digit = 0u;
            foreach (var bit in Bits.EnumBits()) {
                digit |= bit << shift;
                n -= 1;
                if (++shift >= digitWidth) {
                    digits.Append(digitToChar(digit));
                    shift = 0;
                    digit = 0u;
                }
            }
        }

        private static int binaryRadixToWidth(int radix) {
            switch (radix) {
                case 2: return 1;
                case 8: return 3;
                case 16: return 4;
                default: throw Log.Here().ArgErr(nameof(radix));
            }
        }

        private static char digitToChar(uint digit) {
            if (0 <= digit && digit <= 9) return (char)('0' + digit);
            if (10 <= digit && digit <= 15) return (char)('a' + digit - 10);
            throw Log.Here().ArgErr(nameof(digit), "Digit value out of range: " + digit);
        }

        private apfixed(FixedPointFormat fmt, word[] array, bool forceCopy)
            : this(fmt.Signed, new BitArray(array, 0, fmt.Width, forceCopy), fmt.FracWidth) { }

        public apfixed(bool signed, BitArray bits, int fracWidth) {
#if DEBUG
            Assert.ArgInRange(nameof(apfixed), nameof(bits) + "." + nameof(BitArray.Width),
                (signed ? 1 : 0) <= bits.Width && bits.Width <= MaxWidth);
            Assert.ArgInRange(nameof(apfixed), nameof(fracWidth),
                0 <= fracWidth && fracWidth <= (signed ? bits.Width - 1 : bits.Width));
#endif
            this.IsSigned = signed;
            this.Bits = bits;
            this.fp = fracWidth;
        }

        public bool IsNegative => IsSigned && (Bits.Msb != 0u);

        // todo 性能改善 apfixed.IsInteger
        public bool IsInteger {
            get {
                var fw = FracWidth;
                for (int i = 0; i < fw; i++) {
                    if (Bits[i] != 0u) return false;
                }
                return true;
            }
        }

        //public ulong Lower64Bits =>
        //    (ulong)_seg[0] | ((ulong)_seg[1] << 28) | ((ulong)_seg[2] << 56);

        public override bool Equals(object obj) {
            if (obj is apfixed objB) {
                return Equals(objB);
            }
            else {
                // todo apfixed.Equals(): apfixed 以外との比較
                return false;
            }
        }

        public bool Equals(apfixed b) => CompareTo(b) == 0;

        // todo 性能改善: apfixed.CompareTo()
        public int CompareTo(apfixed b) {
            var sub = Sub(b);
            if (sub.Bits.Msb != 0u) return -1;
            else if (sub.Bits.IsZero) return 0;
            else return 1;
        }

        // todo ApFixed.GetHashCode() もうちょっと真面目に実装
        public override int GetHashCode() {
            int hash = Bits.GetHashCode();
            hash ^= (12345 * fp);
            if (IsSigned) hash *= 789012;
            return hash;
        }

        public apfixed Add(apfixed b) => addSub(false, b);
        public apfixed Sub(apfixed b) => addSub(true, b);
        public apfixed addSub(bool sub, apfixed b) {
            var yIntWidth = Math.Max(this.IntWidthWithoutSign, b.IntWidthWithoutSign) + 1 + Math.Max(this.SignWidth, b.SignWidth);
            var yFracWidth = Math.Max(this.FracWidth, b.FracWidth);
            var fmt = new FixedPointFormat(sub || this.IsSigned || this.IsSigned, yIntWidth + yFracWidth, yFracWidth);
            var aWords = this.Bits.GetSegments(yIntWidth - this.IntWidth, yFracWidth - this.FracWidth, this.IsSigned);
            var bWords = b.Bits.GetSegments(yIntWidth - b.IntWidth, yFracWidth - b.FracWidth, b.IsSigned);
            if (sub) bWords.ArithInvertSelf(b.IsSigned, fmt.Width);
#if DEBUG
            Assert.Equal(nameof(apfixed) + "." + nameof(Add) + "(): wordA.Length, wordB.Length", aWords.Length, bWords.Length);
#endif
            word carry = 0u;
            for (int i = 0; i < aWords.Length; i++) {
                aWords[i] = aWords[i].Add(bWords[i], carry, out carry);
            }
            aWords.NormalizeBlankBitsSelf(fmt.Signed, fmt.Width);
            return new apfixed(fmt, aWords, false);
        }
        /*
        public apfixed Mul(apfixed b, out uint carryOut) {
            int fw = this.FracWidth + b.FracWidth;
            int w = this.Width + b.Width;

            var wordA = this.Bits.GetWords( this.IsSigned);
            var wordB = b.Bits.GetWords(b.IsSigned);
            var signed = this.IsSigned ^ b.IsSigned;


            var a = this;
            var accum = new ulong[2 * N];
            for (int i = 0; i < N; i++) {
                for (int j = 0; j < N; j++) {
                    accum[i + j] += (ulong)a.Bits[j] * (ulong)b.Bits[i];
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
            return new ApFixed(w);
        }
        
        public apfixed Div(apfixed b, out apfixed q) {
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
        
        public apfixed Align(out int shift) {
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
        */
        public apfixed ArithInvert() {
            var intExtend = IsSigned ? 0 : 1;
            var newWidth = Width + intExtend;
            var work = Bits.GetSegments(intExtend, 0, IsSigned);
            work.ArithInvertSelf(true, newWidth);
            return new apfixed(new FixedPointFormat(true, newWidth, FracWidth), work, false);
        }
        /*
        public apfixed SingleShiftLeft(uint carry) {
            var w = (uint[])Bits.Clone();
            uint msb;
            for (int i = 0; i < N - 1; i++) {
                msb = (w[i] >> 27) & 1u;
                w[i] = ((w[i] << 1) & 0xffffffeu) | (carry);
                carry = msb;
            }
            w[N - 1] = carry;
            return new ApFixed(w);
        }
        
        public apfixed SingleShiftRight(uint carry) {
            var w = (uint[])Bits.Clone();
            uint lsb = w[N - 1] & 1u;
            w[N - 1] = carry;
            carry = lsb;
            for (int i = N - 2; i >= 0; i--) {
                lsb = w[i] & 1u;
                w[i] = (carry << 27) | ((w[i] >> 1) & 0x7ffffffu);
                carry = lsb;
            }
            return new ApFixed(w);
        }
        
        public apfixed LogicShiftLeft(int n) {
            if (n <= 0) throw new ArgumentException();
            var a = this;
            for (int i = 0; i < n; i++) {
                a = a.SingleShiftLeft(0u);
            }
            return a;
        }
        
        public apfixed LogicShiftRight(int n) {
            if (n < 0) throw new ArgumentException();
            var a = this;
            for (int i = 0; i < n; i++) {
                a = a.SingleShiftRight(0u);
            }
            return a;
        }
        
        public apfixed ArithShiftRight(int n) {
            if (n < 0) throw new ArgumentException();
            uint msb = Msb;
            var a = this;
            for (int i = 0; i < n; i++) {
                a = a.SingleShiftRight(msb);
            }
            return a;
        }
        
        public apfixed TruncateRight(int n) {
            var w = (uint[])Bits.Clone();
            int i = 0;
            while (n >= 28 && i < N) {
                w[i] = 0;
                i++;
                n -= 28;
            }
            if (n > 0 && i < N) {
                w[i] &= ~((1u << n) - 1u);
            }
            return new ApFixed(w);
        }
        */

        // todo 性能改善 apfixed.decimal()
        public decimal ToDecimal() {
            var neg = IsNegative;
            var a = neg ? ArithInvert() : this;
            var w = a.Width;
            var fw = a.FracWidth;
            decimal intVal = 0;
            for (int i = w - 1; i >= fw; i--) {
                intVal = (intVal * 2) + a.Bits[i];
            }
            decimal fracVal = 0;
            for (int i = 0; i < fw; i++) {
                fracVal = (fracVal + a.Bits[i]) / 2;
            }
            decimal val = intVal + fracVal;
            return neg ? -val : val;
        }

        /*
        public static implicit operator apfixed(int i) {
            if (i == 0) return Zero;
            if (i == 1) return One;
            throw new OverflowException();
        }
        
        public static implicit operator apfixed(double d) {
            if (d < 0 || d >= 2) throw new OverflowException();
            apfixed ret = Zero;
            for (int i = 0; i < NumBits; i++) {
                var tmp = (uint)Math.Floor(d);
                ret = ret.SingleShiftLeft(tmp);
                d -= tmp;
                d *= 2;
            }
            return ret;
        }
        
        public static apfixed operator ~(apfixed a) {
            var w = (word[])a.Bits.Clone();
            for (int i = 0; i < N; i++) {
                w[i] ^= 0xfffffffu;
            }
            w[N - 1] &= 1u;
            return new ApFixed(w);
        }
        
        */
        public static apfixed operator -(apfixed a) => a.ArithInvert();

        public static apfixed operator +(apfixed a, apfixed b) => a.Add(b);
        public static apfixed operator -(apfixed a, apfixed b) => a.Sub(b);
        /*
        public static apfixed operator *(apfixed a, apfixed b) => a.Mul(b, out _);
        public static apfixed operator /(apfixed a, apfixed b) { a.Div(b, out apfixed q); return q; }
        
        public static apfixed operator <<(apfixed a, int n) => a.LogicShiftLeft(n);
        public static apfixed operator >>(apfixed a, int n) => a.ArithShiftRight(n);
        */
        public static bool operator ==(apfixed a, apfixed b) => a.Equals(b);
        public static bool operator !=(apfixed a, apfixed b) => !a.Equals((object)b);
        public static bool operator >(apfixed a, apfixed b) => a.CompareTo(b) > 0;
        public static bool operator <(apfixed a, apfixed b) => a.CompareTo(b) < 0;
        public static bool operator >=(apfixed a, apfixed b) => a.CompareTo(b) >= 0;
        public static bool operator <=(apfixed a, apfixed b) => a.CompareTo(b) <= 0;

        public static explicit operator decimal(apfixed a) => a.ToDecimal();

        /*
        public static void Test() {
            AssertAdd(0.5, 0.5, 1, 0u);
            AssertAdd(1, 1, 0, 1u);
            AssertSub(1, 1, 0, 0u);
        }
        
        public static void AssertAdd(double a, double b, double q, uint carry) {
            Assert.Equal("[" + nameof(apfixed) + "] " + a + "+" + b, ((apfixed)a).Add((apfixed)b, out uint c), (apfixed)q);
            Assert.Equal("[" + nameof(apfixed) + "] carry(" + a + "+" + b + ")", carry, c);
        }
        
        public static void AssertSub(double a, double b, double q, uint carry) {
            Assert.Equal("[" + nameof(apfixed) + "] " + a + "-" + b, ((apfixed)a).Sub((apfixed)b, out uint c), (apfixed)q);
            Assert.Equal("[" + nameof(apfixed) + "] carry(" + a + "-" + b + ")", carry, c);
        }
        */
        /*
        private static void adjust(apfixed src, int destW, int destF, out uint[] destX, bool forceCopy) {
            if (destW == src.W && destF == src.fp && !forceCopy) {
                destX = src.Bits;
            }
            else {
                destX = new word[destW / S];
                copyBits(src.Bits, src.fp, destX, 0, src.W);
            }
        }

        private static void extend(word[] X, int pos) {
            var bit = (X[pos / S] >> (pos % S)) & 1u;
            for (int i = pos+1; i < X.Length * S; i++) {
                X[i]
            }
        }

        private static void copyBits(word[] src, int iSrc, word[] dest, int iDest, int width) {
            for (int i = 0; i < width; i++) {
                var bit = src.get(iSrc++);
                var bit = (src[iSrc / S] >> (iSrc % S)) & 1u;
                var tmp = dest[iDest / S];
                tmp &= ~(1u << (iDest % S));
                tmp |= (bit << (iDest % S));
                dest[iDest / S] = tmp;
                iSrc++; iDest++;
            }
        }
        */

    }
}
