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
            if (lexer.TryPop("0x")) radix = Radix.Hex;
            else if (lexer.TryPop("0b")) radix = Radix.Bin;
            else if (lexer.TryPop("0o")) radix = Radix.Oct;
            int baseNumber = radix.ToBaseNumber();

            if (minus && radix != Radix.Decimal) {
                throw Log.Here().E(lexer.Input.CreateException("Minus symbol can be applied only for decimal."));
            }

            lexer.TryPopDigitSequence(radix, out byte[] intDigits);
            byte[] fracDigits = null;
            if (lexer.TryPop('.')) {
                lexer.TryPopDigitSequence(radix, out fracDigits);
            }

            int intWidth = 0, fracWidth = 0;
            char sign;
            if (lexer.TryPop(new char[] { 'u', 's' }, out sign)) {
                lexer.TryPopDigitSequence(Radix.Hex, out byte[] intWidthDigits);
                intWidth = (int)SimpleLexer.DigitsAsInteger(intWidthDigits, Radix.Decimal);
                if (lexer.TryPop('.')) {
                    lexer.TryPopDigitSequence(Radix.Hex, out byte[] fracWidthDigits);
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
            var bits = BitArray.FromDecimalDigits(fmt.RawFormat, minus, digits);
            return new apfixed(bits, fmt.FracWidth);
        }

        public static apfixed FromDecimalDigits(FixedPointFormat fmt, Radix radix, bool minus, byte[] intDigits, byte[] fracDigits) {
            if (!fmt.Signed && minus) throw Log.Here().ArgException(nameof(fmt.Signed) + "=false, " + nameof(minus) + "=true");

            var bits = BitArray.FromDecimalDigits(fmt.RawFormat, false, intDigits);
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
            var bits = BitArray.FromBinaryDigits(fmt.RawFormat, radix, digits);
            return new apfixed(bits, fmt.FracWidth);
        }

        public static apfixed FromBinaryDigits(FixedPointFormat fmt, Radix radix, byte[] intDigits, byte[] fracDigits) {
            var bits = BitArray.FromBinaryDigits(fmt.RawFormat, radix, intDigits);
            bits.LogicShiftLeftSelf(fmt.FracWidth);
            int digitWidth = radix.ToBinaryDigitBits();
            int ibit = fmt.FracWidth;
            for (int i = 0; i < fracDigits.Length; i++) {
                byte digit = fracDigits[i];
                for (int j = 0; j < digitWidth; j++) {
                    if (ibit <= 0) break;
                    digit <<= 1;
                    bits[--ibit] = (UInt32)(digit >> digitWidth) & 1u;
                }
                if (ibit <= 0) break;
            }
            return new apfixed(bits, fmt.FracWidth);
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

        public void ToBinaryStringWithPoint(Radix radix, StringBuilder sb) {
            if (IntWidth > 0) {
                _bits.Clone(FracWidth, false, IntWidth).ToBinaryString(radix, sb);
            }
            else {
                sb.Append('0');
            }
            sb.Append('.');
            if (FracWidth > 0) {
                var digitBits = radix.ToBinaryDigitBits();
                var fw = MathEx.CeilDiv(FracWidth, digitBits) * digitBits;
                _bits.Clone(FracWidth - fw, false, fw).ToBinaryString(radix, sb);
            }
            else {
                sb.Append('0');
            }
        }

        public string ToRawBinaryString(Radix radix) {
            var sb = new StringBuilder();
            ToRawBinaryString(radix, sb);
            return sb.ToString();
        }

        public void ToRawBinaryString(Radix radix, StringBuilder sb)
            => _bits.ToBinaryString(radix, sb);

        public string ToRawDecimalString() {
            var sb = new StringBuilder();
            _bits.ToRawDecimalString(sb);
            return sb.ToString();
        }

        public void ToRawDecimalString(StringBuilder sb)
            => _bits.ToRawDecimalString(sb);

        public string ToDecimalStringWithPoint(int maxFracDigits) {
            var sb = new StringBuilder();
            ToDecimalStringWithPoint(sb, maxFracDigits);
            return sb.ToString();
        }

        public void ToDecimalStringWithPoint(StringBuilder sb, int maxFracDigits) {
            var abs = Abs(out int sign);
            var absBits = abs._bits.Clone(abs.FracWidth, false, abs.IntWidth);
            if (sign < 0) sb.Append('-');
            absBits.ToRawDecimalString(sb);

            if (maxFracDigits > 0) {
                sb.Append('.');
                var fracBits = abs._bits.Clone(0, false, abs.FracWidth + 4);
                fracBits.FillSelf(abs.FracWidth, abs.FracWidth + 4, 0u);
                int idig = 0;
                do {
                    fracBits.Mul10AddSelf(0u);
                    var dig = (int)fracBits.GetSegmentFromBit(abs.FracWidth);
                    sb.Append(CStyleBinary.ToChar(dig));
                    fracBits.FillSelf(abs.FracWidth, abs.FracWidth + 4, 0u);
                } while (!fracBits.IsZero && ++idig < maxFracDigits);
            }
        }

        public string ToStringForDebug() {
            try { return ToString() + "(" + ToDecimal() + ")"; }
            catch { return ToString(); }
        }

        public override string ToString() {
            return "0x" + ToBinaryStringWithPoint(Radix.Hex) + Format.ToString();
        }

        public IEnumerable<UInt32> EnumBits()
            => _bits.EnumBits();

        public IEnumerable<UInt32> EnumBits(int start, int length)
            => _bits.EnumBits(start, length);

        public IEnumerable<UInt32> EnumSegments()
            => _bits.EnumSegments();

        public IEnumerable<UInt32> EnumSegments(int start, int width)
            => _bits.EnumSegments(start, width);

        public UInt32 this[int pos] {
            get => _bits[pos];
            private set => _bits[pos] = value;
        }

        public UInt32 GetSegment(int iseg) => _bits.GetSegment(iseg);
        public UInt32 GetSegmentFromBit(int ibit) => _bits.GetSegmentFromBit(ibit);

        // todo 性能改善 apfixed.IsInteger

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
#if true
            var a = this._bits;
            var b = apfB._bits;
            var iw = this.IntWidth + apfB.FracWidth;
            var fw = this.FracWidth;
            var qWidth = iw + fw;
            var q = BitArray.DivCoreSigned(a, b, qWidth, out  _, true, out int shift);
            if (shift > 0) q.LogicShiftRightSelf(shift);
            else if (shift < 0) q.LogicShiftLeftSelf(-shift);
            return new apfixed(q, FracWidth);
#else
            var a = this._bits;
            var b = apfB._bits;
            var iw = this.IntWidth + apfB.FracWidth;
            var fw = this.FracWidth;
            var qSigned = a.Signed || b.Signed;
            a = a.Abs(out int aSign);
            b = b.Abs(out int bSign);
            var qNegative = (aSign < 0) ^ (bSign < 0);
            var qFmt = new IntFormat(qSigned, iw + fw);
            var q = BitArray.DivCoreUnsigned(a, b, qFmt, IntFormat.Empty, out _, true, out int shift);
            if (shift > 0) q.LogicShiftRightSelf(shift);
            else if (shift < 0) q.LogicShiftLeftSelf(-shift);
            if (qNegative) q.ArithInvertSelf();
            return new apfixed(q, FracWidth);
#endif
        }

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

#if DEBUG
        public static void Test() {
            runTest();
            doTestParse("1234u12", "0x4d2u12");
            doTestParse("1234.5u12.8", "0x4d2.80u12.8");
            doTestParse("1234.25u12.8", "0x4d2.40u12.8");
            doTestParse("1234.125u12.8", "0x4d2.20u12.8");
            doTestParse("1234.0625u12.8", "0x4d2.10u12.8");
            doTestParse("1234.03125u12.8", "0x4d2.08u12.8");
            doTestParse("1234.00390625u12.8", "0x4d2.01u12.8");
            doTestParse("1234.33203125u12.8", "0x4d2.55u12.8");
            doTestParse("1234.19921875u12.8", "0x4d2.33u12.8");
            doTestParse("1234.99609375u12.8", "0x4d2.ffu12.8");
            doTestParse("0.1u4.28", "0x01999999u4.28");
            doTestParse("-0.1s4.28", "0xfe666667s4.28");
            if (Parse("0x0u7.5").ToRawBinaryString(Radix.Hex).NotEq("000")) throw Log.Here().TestFailException();
            if (Parse("0x0u7.5").ToRawBinaryString(Radix.Oct).NotEq("0000")) throw Log.Here().TestFailException();
            if (Parse("0x0u7.5").ToRawBinaryString(Radix.Bin).NotEq("000000000000")) throw Log.Here().TestFailException();
            if (Parse("0x0u7.5").ToRawDecimalString().NotEq("0")) throw Log.Here().TestFailException();
            if (Parse("0xABCu7.5").ToRawBinaryString(Radix.Hex).NotEq("abc")) throw Log.Here().TestFailException();
            if (Parse("0xABCu7.5").ToRawBinaryString(Radix.Oct).NotEq("5274")) throw Log.Here().TestFailException();
            if (Parse("0xABCu7.5").ToRawBinaryString(Radix.Bin).NotEq("101010111100")) throw Log.Here().TestFailException();
            if (Parse("0xABCu7.5").ToRawDecimalString().NotEq("2748")) throw Log.Here().TestFailException();
            if (Parse("0xABCs7.5").ToRawBinaryString(Radix.Hex).NotEq("abc")) throw Log.Here().TestFailException();
            if (Parse("0xABCs7.5").ToRawBinaryString(Radix.Oct).NotEq("5274")) throw Log.Here().TestFailException();
            if (Parse("0xABCs7.5").ToRawBinaryString(Radix.Bin).NotEq("101010111100")) throw Log.Here().TestFailException();
            if (Parse("0xABCs7.5").ToRawDecimalString().NotEq("2748")) throw Log.Here().TestFailException();
            if (Parse("0xABCDEFABCDEFABCDEFu58.14").ToRawBinaryString(Radix.Hex).NotEq("abcdefabcdefabcdef")) throw Log.Here().TestFailException();
            if (Parse("0xABCDEFABCDEFABCDEFu58.14").ToRawBinaryString(Radix.Oct).NotEq("527467575274675752746757")) throw Log.Here().TestFailException();
            if (Parse("0xABCDEFABCDEFABCDEFu58.14").ToRawBinaryString(Radix.Bin).NotEq("101010111100110111101111101010111100110111101111101010111100110111101111")) throw Log.Here().TestFailException();
            if (Parse("0x0u7.5").ToBinaryStringWithPoint(Radix.Hex).NotEq("00.00")) throw Log.Here().TestFailException();
            if (Parse("0x0u7.5").ToBinaryStringWithPoint(Radix.Oct).NotEq("000.00")) throw Log.Here().TestFailException();
            if (Parse("0x0u7.5").ToBinaryStringWithPoint(Radix.Bin).NotEq("0000000.00000")) throw Log.Here().TestFailException();
            if (Parse("0x0u7.5").ToDecimalStringWithPoint(0).NotEq("0")) throw Log.Here().TestFailException();
            if (Parse("0x0u7.5").ToDecimalStringWithPoint(8).NotEq("0.0")) throw Log.Here().TestFailException();
            if (Parse("0xABCu7.5").ToBinaryStringWithPoint(Radix.Hex).NotEq("55.e0")) throw Log.Here().TestFailException();
            if (Parse("0xABCu7.5").ToBinaryStringWithPoint(Radix.Oct).NotEq("125.70")) throw Log.Here().TestFailException();
            if (Parse("0xABCu7.5").ToBinaryStringWithPoint(Radix.Bin).NotEq("1010101.11100")) throw Log.Here().TestFailException();
            if (Parse("0xABCu7.5").ToDecimalStringWithPoint(0).NotEq("85")) throw Log.Here().TestFailException();
            if (Parse("0xABCu7.5").ToDecimalStringWithPoint(1).NotEq("85.8")) throw Log.Here().TestFailException();
            if (Parse("0xABCu7.5").ToDecimalStringWithPoint(2).NotEq("85.87")) throw Log.Here().TestFailException();
            if (Parse("0xABCu7.5").ToDecimalStringWithPoint(3).NotEq("85.875")) throw Log.Here().TestFailException();
            if (Parse("0xABCu7.5").ToDecimalStringWithPoint(4).NotEq("85.875")) throw Log.Here().TestFailException();
            if (Parse("0xABCs7.5").ToBinaryStringWithPoint(Radix.Hex).NotEq("55.e0")) throw Log.Here().TestFailException();
            if (Parse("0xABCs7.5").ToBinaryStringWithPoint(Radix.Oct).NotEq("125.70")) throw Log.Here().TestFailException();
            if (Parse("0xABCs7.5").ToBinaryStringWithPoint(Radix.Bin).NotEq("1010101.11100")) throw Log.Here().TestFailException();
            if (Parse("0xABCs7.5").ToDecimalStringWithPoint(0).NotEq("-42")) throw Log.Here().TestFailException();
            if (Parse("0xABCs7.5").ToDecimalStringWithPoint(1).NotEq("-42.1")) throw Log.Here().TestFailException();
            if (Parse("0xABCs7.5").ToDecimalStringWithPoint(2).NotEq("-42.12")) throw Log.Here().TestFailException();
            if (Parse("0xABCs7.5").ToDecimalStringWithPoint(3).NotEq("-42.125")) throw Log.Here().TestFailException();
            if (Parse("0xABCs7.5").ToDecimalStringWithPoint(4).NotEq("-42.125")) throw Log.Here().TestFailException();
            if (Parse("0xABCDEFABCDEFABCDEFu58.14").ToBinaryStringWithPoint(Radix.Hex)
                .NotEq("2af37beaf37beaf.37bc")) throw Log.Here().TestFailException();
            if (Parse("0xABCDEFABCDEFABCDEFu58.14").ToBinaryStringWithPoint(Radix.Oct)
                .NotEq("12571573725715737257.15736")) throw Log.Here().TestFailException();
            if (Parse("0xABCDEFABCDEFABCDEFu58.14").ToBinaryStringWithPoint(Radix.Bin)
                .NotEq("1010101111001101111011111010101111001101111011111010101111.00110111101111")) throw Log.Here().TestFailException();

            doTestBinaryOp("100.0u8", '/', "25.0u8", "4.0u8");
            doTestBinaryOp("100.0u8.4", '/', "25.0u8.4", "4.0u12.4");
            doTestBinaryOp("100.0u8.24", '/', "3.0u8", "0x21.555555u8.24");
            doTestBinaryOp("0x0.0001u0.16", '/', "0x8000.0u16", "0x1.0u0.16");
            doTestBinaryOp("0x8000.0u16", '/', "0x0.0001u0.16", "0x80000000.0u32");
            doTestBinaryOp("0x8000.0s16", '/', "0x0.0001u0.16", "0x80000000.0s32");
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
            if (a.FracWidth.NotEq(b.FracWidth)) throw Log.Here().TestFailException(label);
            if (a.Width.NotEq(b.Width)) throw Log.Here().TestFailException(label);
        }

        private static void doTestBinaryOp(string aStr, char op, string bStr, string cStr) {
            try { doTestBinaryOpInner(aStr, op, bStr, cStr); }
            catch { 
                Verbose = true;
                BitArray.Verbose = true;
                doTestBinaryOpInner(aStr, op, bStr, cStr);
            }
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
            string label = a.ToStringForDebug() + " " + op + " " + b.ToStringForDebug() 
                + " = " + cAct.ToStringForDebug() + " != " + cExp.ToStringForDebug();
            if (cAct.Sign.NotEq(cExp.Sign)) throw Log.Here().TestFailException(label);
            if (cAct.Width.NotEq(cExp.Width)) throw Log.Here().TestFailException(label);
            if (cAct.FracWidth.NotEq(cExp.FracWidth)) throw Log.Here().TestFailException(label);
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
                bitsB = bitsB.Clone(0, true, bitsB.Width + 1);
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
        }
#endif

    }
}
