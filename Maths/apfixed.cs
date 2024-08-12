using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Texts;
using Shapoco.Maths.BitArrays;

namespace Shapoco.Maths {
    struct apfixed : IComparable<apfixed> {
#if DEBUG
        public static bool Verbose = false;
#endif
        public const int MaxWidth = 1024;
        private const int Stride = sizeof(UInt32) * 8;

        private BitArray _bits;
        public readonly int FracWidth;

        public bool Signed => _bits.Signed;
        public int Width => _bits.Width;
        public int IntWidth => _bits.Width - FracWidth;
        public int SignWidth => _bits.SignWidth;
        public int WidthWithoutSign => _bits.WidthWithoutSign;
        public int IntWidthWithoutSign => _bits.WidthWithoutSign - FracWidth;

        public UInt32 Msb => _bits.Msb;
        public bool IsNegative => _bits.IsNegative;
        public bool IsZero => _bits.IsZero;
        public int Sign => _bits.Sign;

        public bool IsInteger => EnumBits(0, FracWidth).All(b => b == 0u);

        public FixedPointFormat Format => new FixedPointFormat(_bits.Signed, _bits.Width, FracWidth);

        public static apfixed Parse(string s) {
            var lexer = new SimpleLexer(s, autoSkipWhite: false);
            lexer.SkipWhite();

            bool minus = lexer.TryPop("-");
            lexer.SkipWhite();

            Radix radix = Radix.Decimal;
            if (lexer.TryPop("0x")) radix = Radix.Hexadecimal;
            else if (lexer.TryPop("0b")) radix = Radix.Binary;
            else if (lexer.TryPop("0o")) radix = Radix.Octal;
            int baseNumber = radix.ToBaseNumber();

            if (minus && radix != Radix.Decimal)
                throw Log.Here().E(lexer.Input.CreateException("Minus symbol can be applied for decimal."));

            lexer.TryPopDigitSequence(radix, out byte[] intDigits);
            byte[] fracDigits = null;
            if (lexer.TryPop('.')) {
                lexer.TryPopDigitSequence(radix, out fracDigits);
            }

            int intWidth = 0, fracWidth = 0;
            char sign;
            if (lexer.TryPop(new char[] { 'u', 's' }, out sign)) {
                lexer.TryPopDigitSequence(Radix.Hexadecimal, out byte[] intWidthDigits);
                intWidth = (int)SimpleLexer.DigitsAsInteger(intWidthDigits, Radix.Decimal);
                if (lexer.TryPop('.')) {
                    lexer.TryPopDigitSequence(Radix.Hexadecimal, out byte[] fracWidthDigits);
                    fracWidth = (int)SimpleLexer.DigitsAsInteger(fracWidthDigits, Radix.Decimal);
                }
            }
            else if (radix == Radix.Decimal) {
                throw Log.Here().E(lexer.Input.CreateExpectedException("'u' or 's'"));
            }
            else {
                sign = 'u';
                intWidth = intDigits.Length * radix.ToBinaryDigitBits();
                if (fracDigits != null) {
                    fracWidth = fracDigits.Length * radix.ToBinaryDigitBits();
                }
            }
            lexer.SkipWhite();
            lexer.AssertEos();

            bool signed = (sign == 's');
            var fmt = new FixedPointFormat(signed, intWidth + fracWidth, fracWidth);
            if (radix == Radix.Decimal) {
                if (fracDigits == null) {
                    return FromDecimalDigits(fmt, radix, minus, intDigits);
                }
                else {
                    return FromDecimalDigits(fmt, radix, minus, intDigits, fracDigits);
                }
            }
            else {
           if (fracDigits == null) {
                    return FromBinaryDigits(fmt, radix, intDigits);
                }
                else {
                    return FromBinaryDigits(fmt, radix, intDigits, fracDigits);
                }
            }
        }

        public static apfixed FromDecimalDigits(FixedPointFormat fmt, Radix radix, bool minus, byte[] digits) {
            if (!fmt.Signed && minus) throw Log.Here().ArgException(nameof(fmt.Signed) + "=false, " + nameof(minus) + "=true");
            var bits = BitArray.FromDecimalDigits(fmt.Signed, fmt.Width, minus, digits);
            return new apfixed(bits, fmt.FracWidth);
        }

        public static apfixed FromDecimalDigits(FixedPointFormat fmt, Radix radix, bool minus, byte[] intDigits, byte[] fracDigits) {
            if (!fmt.Signed && minus) throw Log.Here().ArgException(nameof(fmt.Signed) + "=false, " + nameof(minus) + "=true");

            var bits = BitArray.FromDecimalDigits(fmt.Signed, fmt.Width, false, intDigits);
            bits.LogicShiftLeftSelf(fmt.FracWidth);
#if DEBUG
            if (Verbose) {
                var sb = new StringBuilder();
                foreach (var dig in fracDigits) {
                    sb.Append(Convert.ToString(dig, 16));
                }
                Log.Here().T("fracDigits=" + sb.ToString());
            }
#endif
            fracDigits = (byte[])fracDigits.Clone();
            bool notZero = false;
            for (int ibit = fmt.FracWidth - 1; ibit >= 0; ibit--) {
                byte carry = 0;
                notZero = false;
                for (int idig = fracDigits.Length - 1; idig >= 0; idig--) {
                    byte tmp = (byte)(fracDigits[idig] * 2 + carry);
                    carry = (byte)(tmp / 10);
                    tmp = (byte)(tmp % 10);
                    notZero |= tmp != 0;
                    fracDigits[idig] = tmp;
                }
#if DEBUG
                if (carry >= 2) throw Log.Here().F(new InvalidOperationException());
#endif
                bits[ibit] = carry & 1u;
#if DEBUG
                if (Verbose) {
                    var sb = new StringBuilder();
                    foreach (var dig in fracDigits) {
                        sb.Append(Convert.ToString(dig, 16));
                    }
                    Log.Here().T(
                        "ibit=" + ibit + ", " +
                        "carry=" + carry + ", " +
                        "fracDigits=" + sb.ToString() + ", " +
                        "bits=" + bits.ToStringForDebug());
                }
#endif
            }

            bits.FillBlankSelf();
            if (minus) bits.ArithInvertSelf();

            return new apfixed(bits, fmt.FracWidth);
        }

        public static apfixed FromBinaryDigits(FixedPointFormat fmt, Radix radix, byte[] digits) {
            var bits = BitArray.FromBinaryDigits(fmt.Signed, fmt.Width, radix, digits);
            return new apfixed(bits, fmt.FracWidth);
        }

        public static apfixed FromBinaryDigits(FixedPointFormat fmt, Radix radix, byte[] intDigits, byte[] fracDigits) {
            int digitWidth = radix.ToBinaryDigitBits();

            var work = new BitArray(fmt.Signed, fmt.Width);

            int ibit = fmt.FracWidth;
            for (int i = intDigits.Length - 1; i >= 0; i--) {
                byte digit = intDigits[i];
                for (int j = 0; j < digitWidth; j++) {
                    if (ibit >= fmt.Width) break;
                    work[ibit++] = digit & 1u;
                    digit >>= 1;
                }
                if (ibit >= fmt.Width) break;
            }
            work.FillBlankSelf();

            ibit = fmt.FracWidth;
            for (int i = 0; i < fracDigits.Length; i++) {
                byte digit = fracDigits[i];
                for (int j = 0; j < digitWidth; j++) {
                    if (ibit <= 0) break;
                    digit <<= 1;
                    work[--ibit] = (UInt32)(digit >> digitWidth) & 1u;
                }
                if (ibit <= 0) break;
            }

            return new apfixed(work, fmt.FracWidth);
        }

        private apfixed(FixedPointFormat fmt, UInt32[] array, bool forceCopy) {
            this.FracWidth = fmt.FracWidth;
            this._bits = new BitArray(fmt.Signed, fmt.Width, array, forceCopy);
        }

        private apfixed(BitArray bits, int frac) {
            this.FracWidth = frac;
            this._bits = bits;
        }

        public string ToBinaryStringWithPoint(Radix radix) {
            var sb = new StringBuilder();
            ToBinaryStringWithPoint(radix, sb);
            return sb.ToString();
        }
        public void ToBinaryStringWithPoint(Radix radix, StringBuilder digits) {
            int digitWidth = radix.ToBinaryDigitBits();

            var w = Width;
            var iw = IntWidth;
            var fw = FracWidth;

            if (iw == 0) {
                digits.Append('0');
            }
            else {
                int shift = (iw + digitWidth - 1) % digitWidth;
                int n = iw;
                UInt32 digit = 0u;
                foreach (var bit in EnumBits(w - 1, -iw)) {
                    digit |= bit << shift;
                    n -= 1;
                    if (shift-- <= 0 || n == 0) {
                        digits.Append(BitArray.DigitToChar(digit));
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
                UInt32 digit = 0u;
                foreach (var bit in EnumBits(fw - 1, -fw)) {
                    digit |= bit << shift;
                    n -= 1;
                    if (shift-- <= 0 || n == 0) {
                        digits.Append(BitArray.DigitToChar(digit));
                        shift = digitWidth - 1;
                        digit = 0u;
                    }
                }
            }
        }

        public void ToRawBinaryString(Radix radix, StringBuilder digits) {
            int w = Width;
            int digitWidth = radix.ToBinaryDigitBits();
            int shift = (w - 1) % digitWidth;
            UInt32 digit = 0u;
            var scan = new BitScan(_bits, w - 1, -w);
            for (int i = 0; i < w; i++) {
                var bit = scan.Read();
                digit |= bit << shift;
                if (shift-- <= 0 || i + 1 == w) {
                    digits.Append(BitArray.DigitToChar(digit));
                    shift = digitWidth - 1;
                    digit = 0u;
                }
            }
        }

        public string ToStringForDebug() {
            try { return ToString() + "(" + ToDecimal() + ")"; }
            catch { return ToString(); }
        }

        public override string ToString() {
            return "0x" + ToBinaryStringWithPoint(Radix.Hexadecimal) + Format.ToString();
        }

        public IEnumerable<UInt32> EnumBits()
            => _bits.EnumBits();

        public IEnumerable<UInt32> EnumBits(int start, int length)
            => _bits.EnumBits(start, length);

        public IEnumerable<UInt32> EnumSegments()
            => _bits.EnumSegments();

        public IEnumerable<UInt32> EnumSegments(int start, int width)
            => _bits.EnumSegments(start, width);

        //private UInt32[] copySegments(int intExtend, int fracExtend, bool signExt) {
        //    var words = SegArray.Create(intExtend + Width + fracExtend);
        //    _bits.CopyBits(0, words, fracExtend, Width);
        //    if (signExt) words.ExtendSign(fracExtend + Width - 1);
        //    return words;
        //}


        public UInt32 this[int pos] {
            get => _bits[pos];
            private set => _bits[pos] = value;
        }

        public UInt32 GetSegment(int iseg) => _bits.GetSegment(iseg);
        public UInt32 GetSegmentFromBit(int ibit) => _bits.GetSegmentFromBit(ibit);

        // todo 性能改善 apfixed.IsInteger

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
            if (sub.Msb != 0u) return -1;
            else if (sub.IsZero) return 0;
            else return 1;
        }

        // todo ApFixed.GetHashCode() もうちょっと真面目に実装
        public override int GetHashCode()
            => _bits.GetHashCode() ^ (0x71a84c73 * FracWidth);

        public apfixed Add(apfixed b) => add(this, b, false);
        public apfixed Sub(apfixed b) => add(this, b, true);

        private static apfixed add(apfixed a, apfixed b, bool sub) {
            var iw = Math.Max(a.IntWidthWithoutSign, b.IntWidthWithoutSign) + Math.Max(a.SignWidth, b.SignWidth) + 1;
            var fw = Math.Max(a.FracWidth, b.FracWidth);
            var o = a._bits.AddSub(b._bits, sub, a.FracWidth - fw, b.FracWidth - fw, iw + fw);
            return new apfixed(o, fw);
        }

        public apfixed Mul(apfixed b) {
            var bits = _bits.Mul(b._bits);
            return new apfixed(bits, FracWidth + b.FracWidth);
        }

        public apfixed Div(apfixed apfB) {
            var a = this._bits;
            var b = apfB._bits;
            var iw = this.IntWidth + apfB.FracWidth;
            var fw = apfB.FracWidth;
            var signed = a.Signed || b.Signed;
            a = a.Abs(out int signA);
            b = b.Abs(out int signB);
            var negative = (signA < 0) ^ (signB < 0);
            var width = iw + fw;
            var q = BitArray.DivCore(a, b, signed, width, out int shift, out _);
            if (shift > 0) q.LogicShiftRightSelf(shift);
            else if (shift < 0) q.LogicShiftLeftSelf(-shift);
            if (negative) q.ArithInvertSelf();
            return new apfixed(q, FracWidth);
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
        public apfixed ArithInvert() => new apfixed(_bits.ArithInvert(), FracWidth);

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

        public apfixed Abs(out int origSign) => new apfixed(_bits.Abs(out origSign), FracWidth);

        public decimal ToDecimal() => _bits.ToDecimal() / MathEx.PowN(2m, FracWidth);

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
        public static apfixed operator *(apfixed a, apfixed b) => a.Mul(b);
        public static apfixed operator /(apfixed a, apfixed b) => a.Div(b);

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

#if DEBUG
        public static void Test() {
            runTest();
            doTestParse("100u8", "0x64u8");
            doTestParse("100.5u8.8", "0x64.80u8.8");
            doTestParse("100.25u8.8", "0x64.40u8.8");
            doTestParse("100.125u8.8", "0x64.20u8.8");
            doTestParse("100.0625u8.8", "0x64.10u8.8");
            doTestParse("100.03125u8.8", "0x64.08u8.8");
            doTestParse("100.00390625u8.8", "0x64.01u8.8");
            doTestParse("100.33203125u8.8", "0x64.55u8.8");
            doTestParse("100.19921875u8.8", "0x64.33u8.8");
            doTestParse("100.99609375u8.8", "0x64.ffu8.8");
            doTestParse("0.1u4.28", "0x01999999u4.28");
            doTestParse("-0.1s4.28", "0xfe666667s4.28");
            doTestBinaryOp("100u8", '/', "25u8", "4u8");
        }

        private static void doTestParse(string aStr, string bStr) {
            try { doTestParseInner(aStr, bStr); }
            catch { Verbose = true; doTestParseInner(aStr, bStr); }
        }

        private static void doTestParseInner(string aStr, string bStr) {
            var a = Parse(aStr);
            var b = Parse(bStr);
            string label = a.ToStringForDebug() + " != " + b.ToStringForDebug();
            if (a.NotEq(b)) throw Log.Here().TestFailException(label);
            if (a.Sign.NotEq(b.Sign)) throw Log.Here().TestFailException(label);
            if (a.Width.NotEq(b.Width)) throw Log.Here().TestFailException(label);
        }

        private static void doTestBinaryOp(string aStr, char op, string bStr, string cStr) {
            try { doTestBinaryOpInner(aStr, op, bStr, cStr); }
            catch { Verbose = true; doTestBinaryOpInner(aStr, op, bStr, cStr); }
        }

        private static void doTestBinaryOpInner(string aStr, char op, string bStr, string cStr) {
            var a = Parse(aStr);
            var b = Parse(bStr);
            var cExp = Parse(cStr);
            apfixed cAct;
            switch (op) {
                case '+': cAct = a + b; break;
                case '-': cAct = a - b; break;
                case '*': cAct = a * b; break;
                case '/': cAct = a / b; break;
                default: throw new NotImplementedException();
            }
            string label = a.ToStringForDebug() + " " + op + " " + b.ToStringForDebug() + " = " + cExp.ToStringForDebug();
            if (cAct.Sign.NotEq(cExp.Sign)) throw Log.Here().TestFailException(label);
            if (cAct.Width.NotEq(cExp.Width)) throw Log.Here().TestFailException(label);
            if (cAct.NotEq(cExp)) throw Log.Here().TestFailException(label);
        }

        private static void runTest() {
            var rng = new Random();
            var widths = new int[] { 1, 2, 3, 20, 31, 32, 33, 48, 63, 64 /*, 65, 81, 96*/ };
            var signs = new bool[] { false, true };
            var vals = new List<apfixed>();
            foreach (var signed in signs) {
                foreach (var w in widths) {
                    var fws = new List<int>();
                    fws.Add(0);
                    fws.Add(1);
                    if (w >= 4) fws.Add(w * 3 / 4);
                    if (w >= 2) fws.Add(w / 2);
                    if (w >= 4) fws.Add(w / 4);
                    foreach (var fw in fws) {
                        var segs = new UInt32[BitArray.BitWidthToSegment(w)];
                        for (int i = 0; i < segs.Length; i++) {
                            segs[i] = (UInt32)rng.Next();
                        }
                        var fmt = new FixedPointFormat(signed, w, fw);
                        vals.Add(new apfixed(fmt, segs, false));
                    }
                }
            }
            foreach (var a in vals) {
                foreach (var b in vals) {
                    try { testAdd(a, b, false); } catch { Verbose = true; testAdd(a, b, false); }
                    try { testAdd(a, b, true); } catch { Verbose = true; testAdd(a, b, true); }
                }
            }
        }

        private static void testAdd(apfixed a, apfixed b, bool sub) {
            var c = sub ? (a - b) : (a + b);

            var iw = Math.Max(a.IntWidthWithoutSign, b.IntWidthWithoutSign) + Math.Max(a.SignWidth, b.SignWidth) + 1;
            var fw = Math.Max(a.FracWidth, b.FracWidth);
            var w = iw + fw;
            var signed = a.Signed || b.Signed || sub;

            var bitsA = a._bits;
            var bitsB = b._bits;

            if (sub) {
                bitsB = bitsB.Clone(0, bitsB.Width + 1, true);
                bitsB.ArithInvertSelf();
            }

            var bitsC = new BitArray(signed, w);
            int ibitA = a.FracWidth - fw;
            int ibitB = b.FracWidth - fw;
            UInt32 carry = 0u;
            for (int ibitC = 0; ibitC < w; ibitA++, ibitB++, ibitC++) {
                UInt32 bitC = bitsA[ibitA] + bitsB[ibitB] + carry;
                carry = bitC >> 1;
                bitsC[ibitC] = bitC & 1u;
            }
            bitsC.FillBlankSelf();

            var label =
                "a=" + a.ToString() + ", " +
                "b=" + b.ToString() + ", " +
                "a" + (sub ? "+" : "-") + "b=" + a.ToString();
            if (c.Sign.NotEq(bitsC.Sign)) throw Log.Here().TestFailException(label);
            if (c.Width.NotEq(bitsC.Width)) throw Log.Here().TestFailException(label);
            if (c.FracWidth.NotEq(fw)) throw Log.Here().TestFailException(label);
            if (c._bits.Segments.NotEqHex(bitsC.Segments)) throw Log.Here().TestFailException(label);
            //if (c._bits.Segments.SequenceEqual(bitsC.Segments).NotTrue()) throw Log.Here().TestFailException();
        }
#endif

    }
}
