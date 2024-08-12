using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Texts;

namespace Shapoco.Maths.BitArrays {
    struct BitArray : IComparable<BitArray> {
#if DEBUG
        public static bool Verbose = false;
#endif
        public const int Stride = sizeof(UInt32) * 8;
        public const int Clog2Stride =
            Stride <= 8 ? 0 :
            Stride <= 16 ? 4 :
            Stride <= 32 ? 5 :
            Stride <= 64 ? 6 :
            Stride <= 128 ? 7 :
            0;

        private bool _signed;

        public readonly int Width;
        public bool Signed => _signed;
        public readonly UInt32[] Segments;

        public int SignWidth => Signed ? 1 : 0;
        public int WidthWithoutSign => Signed ? Width - 1 : Width;
        public int LastSegmentWidth => ((Width - 1) & (Stride - 1)) + 1;

        public UInt32 Msb => this[Width - 1];
        public bool IsNegative => Signed && (Msb != 0u);
        public bool IsZero => EnumSegments().All(x => x == 0u);
        public int Sign => IsNegative ? -1 : IsZero ? 0 : 1;

        public int NumSegments => BitWidthToSegment(Width);

        public static BitArray Parse(string s) {
            var lexer = new SimpleLexer(s, autoSkipWhite: false);
            lexer.SkipWhite();

            bool minus = lexer.TryPop("-");
            lexer.SkipWhite();

            Radix radix = Radix.Decimal;
            if (lexer.TryPop("0x")) radix = Radix.Hexadecimal;
            else if (lexer.TryPop("0b")) radix = Radix.Binary;
            else if (lexer.TryPop("0o")) radix = Radix.Octal;

            if (minus && radix != Radix.Decimal)
                throw Log.Here().E(lexer.Input.CreateException("Minus symbol can be applied for decimal."));

            lexer.TryPopDigitSequence(radix, out byte[] digits);

            int width = 0;

            char sign;
            if (lexer.TryPop(new char[] { 'u', 's' }, out sign)) {
                lexer.TryPopDigitSequence(Radix.Hexadecimal, out byte[] intWidthDigits);
                width = (int)SimpleLexer.DigitsAsInteger(intWidthDigits, Radix.Decimal);
            }
            else if (radix == Radix.Decimal) {
                throw Log.Here().E(lexer.Input.CreateExpectedException("'u' or 's'"));
            }
            else {
                sign = 'u';
                width = digits.Length * radix.ToBinaryDigitBits();
            }
            lexer.SkipWhite();
            lexer.AssertEos();

            bool signed = (sign == 's');
            if (radix == Radix.Decimal) {
                return FromDecimalDigits(signed, width, minus, digits);
            }
            else {
                return FromBinaryDigits(signed, width, radix, digits);
            }
        }

        public static BitArray FromBinaryDigits(bool signed, int width, Radix radix, byte[] digits) {
            int step = radix.ToBinaryDigitBits();
            var bits = new BitArray(signed, width);
            int ibit = 0;
            for (int i = digits.Length - 1; i >= 0; i--) {
                byte digit = digits[i];
                for (int j = 0; j < step; j++) {
                    if (ibit >= width) break;
                    bits[ibit++] = digit & 1u;
                    digit >>= 1;
                }
                if (ibit >= width) break;
            }
            bits.FillBlankSelf();
            return bits;
        }

        public static BitArray FromDecimalDigits(bool signed, int width, bool minus, byte[] digits) {
            if (!signed && minus) throw Log.Here().ArgException(nameof(signed) + "=false, " + nameof(minus) + "=true");
            var bits = new BitArray(signed, width);
            foreach (var digit in digits) {
                bits.Mul10AddSelf(digit);
            }
            bits.FillBlankSelf();
            if (minus) bits.ArithInvertSelf();
            return bits;
        }

        public void Mul10AddSelf(UInt32 x) {
            UInt32 carry = x;
            for (int iseg = 0; iseg < Segments.Length; iseg++) {
                UInt64 seg = (UInt64)Segments[iseg] * 10u + carry;
                carry = (UInt32)(seg >> Stride);
                Segments[iseg] = (UInt32)(seg & ((1ul << Stride) - 1));
            }
            if (carry != 0u) throw Log.Here().E(new OverflowException());
        }

        public BitArray(bool signed, int width, UInt32[] segs, bool forceCopy) {
#if DEBUG
            if (width < 1) throw Log.Here().ArgException(nameof(width));
#endif
            var nsegs = BitWidthToSegment(width);
            if (segs.Length != nsegs || forceCopy) {
                var newArray = new UInt32[nsegs];
                Array.Copy(segs, newArray, Math.Min(nsegs, segs.Length));
                segs = newArray;
            }

            this._signed = signed;
            this.Width = width;
            this.Segments = segs;

            FillBlankSelf();
        }

        public BitArray(bool signed, int width)
            : this(signed, width, new UInt32[BitWidthToSegment(width)], false) { }

        public UInt32 this[int ibit] {
            get {
                if (ibit < 0) return 0u;
                if (ibit >= Width) {
                    if (!Signed) return 0u;
                    ibit = Width - 1;
                }
                var iseg = BitIndexToSegment(ibit, out int shift);
                return (Segments[iseg] >> shift) & 1u;
            }
            set {
#if DEBUG
                if (ibit < 0 || Segments.Length * Stride <= ibit) throw Log.Here().ArgException(nameof(ibit));
#endif
                var iseg = BitIndexToSegment(ibit, out int shift);
                var w = Segments[iseg];
                w &= ~((UInt32)1u << shift);
                w |= (value & 1u) << shift;
                Segments[iseg] = w;
            }
        }

        public UInt32 GetSegment(int iseg) {
            var nseg = NumSegments;
            if (iseg < 0)
                return 0u;
            else if (iseg >= nseg)
                return IsNegative ? ~(UInt32)0u : 0u;
            else if (iseg == nseg - 1)
                return Segments[iseg].FillBlank(Signed, LastSegmentWidth);
            else
                return Segments[iseg];
        }

        public UInt32 GetSegmentFromBit(int ibit) {
            var iseg = BitIndexToSegment(ibit, out int shift);
            var ret = GetSegment(iseg) >> shift;
            if (shift != 0) {
                ret |= GetSegment(iseg + 1) << (Stride - shift - 1);
            }
            return ret;
        }

        public void ExtendSignSelf(int ibit) {
            FillSelf(ibit + 1, Segments.Length * Stride, this[ibit]);
        }

        // todo 性能改善: SegmentedBitArrays.Fill()
        public void FillSelf(int from, int toExclusive, UInt32 value) {
#if DEBUG
            if (from < 0) throw Log.Here().ArgException(nameof(from));
            if (Segments.Length * Stride < toExclusive) throw Log.Here().ArgException(nameof(toExclusive));
            if (from > toExclusive) throw Log.Here().ArgException(nameof(from) + ", " + nameof(toExclusive));
#endif
            if (from == toExclusive) return;
            int segFrom = BitWidthToSegment(from);
            int segTo = toExclusive / Stride;
            var segVal = value == 0u ? 0u : ~(UInt32)0u;
            for (int ibit = from; ibit < segFrom * Stride; ibit++) {
                this[ibit] = value;
            }
            for (int iseg = segFrom; iseg < segTo; iseg++) {
                Segments[iseg] = segVal;
            }
            for (int ibit = segTo * Stride; ibit < toExclusive; ibit++) {
                this[ibit] = value;
            }
        }

        public static UInt32 GetLastSegmentMask(int width) {
            width %= Stride;
            if (width == 0) {
                return ~(UInt32)0u;
            }
            else {
                return (1u << width) - 1u;
            }
        }

        public void BitwiseInvertSelf() {
            int nseg = NumSegments;
            for (int i = 0; i < nseg; i++) {
                Segments[i] = ~Segments[i];
            }
            FillBlankSelf();
        }

        public void ArithInvertSelf() {
            UInt32 carry = 1u;
            for (int i = 0; i < Segments.Length; i++) {
                Segments[i] = (~Segments[i]).Add(0, carry, out carry);
            }
            FillBlankSelf();
        }

        public BitArray ArithInvert() {
            var w = Width + (Signed ? 1 : 0);
            var bits = Clone(0, w, true);
            bits.ArithInvertSelf();
            return bits;
        }

        public BitArray Abs(out int origSign) {
            var bits = Clone(0, Width);
            bits.AbsSelf(out origSign);
            return bits;
        }

        public void AbsSelf(out int origSign) {
            origSign = Sign;
            _signed = false;
            if (origSign < 0) ArithInvertSelf();
        }

        public void FillBlankSelf() {
            var w = Segments.Length * Stride;
            if (Signed) {
                ExtendSignSelf(Width - 1);
            }
            else if (Width < w) {
                FillSelf(Width, w, 0u);
            }
        }

        public override bool Equals(object obj) {
            if (obj is BitArray objB) {
                return Equals(objB);
            }
            else {
                // todo BitArray.Equals(): BitArray 以外との比較
                return false;
            }
        }
        public bool Equals(BitArray b) => CompareTo(b) == 0;

        // todo 性能改善: apfixed.CompareTo()
        public int CompareTo(BitArray b) {
            var sub = Sub(b);
            if (sub.Msb != 0u) return -1;
            else if (sub.IsZero) return 0;
            else return 1;
        }

        public BitArray Add(BitArray b) => AddSub(b, false);
        public BitArray Sub(BitArray b) => AddSub(b, true);

        public BitArray AddSub(BitArray b, bool sub, int ibitA = 0, int ibitB = 0, int width = -1) {
            if (width <= 0) {
                width = Math.Max(this.WidthWithoutSign, b.WidthWithoutSign) + Math.Max(this.SignWidth, b.SignWidth) + 1;
            }

            var signed = Signed || b.Signed || sub;

            if (sub) {
                b = b.Clone(0, b.Width + 1, true);
                b.ArithInvertSelf();
            }

            var ascan = new SegScan(this, ibitA);
            var bscan = new SegScan(b, ibitB);
            var segs = CreateArray(width);
            UInt32 oCarry = 0u;
            for (int i = 0; i < segs.Length; i++) {
                var aseg = ascan.Read();
                var bseg = bscan.Read();
                segs[i] = aseg.Add(bseg, oCarry, out oCarry);
            }

            return new BitArray(signed, width, segs, false);
        }

        public BitArray Mul(BitArray b) {
            var a = this;
            var signedC = a.Signed || b.Signed;
            a = a.Abs(out int signA);
            b = b.Abs(out int signB);
            var negative = (signA < 0) ^ (signB < 0);

            var segsA = a.Segments;
            var segsB = b.Segments;

            var nsegA = a.NumSegments;
            var nsegB = b.NumSegments;
            var nsegAB = nsegA + nsegB;

            var accum = new UInt64[nsegAB];
            for (int isegB = 0; isegB < nsegB; isegB++) {
                for (int isegA = 0; isegA < nsegA; isegA++) {
                    accum[isegA + isegB] += (UInt64)segsA[isegA] * segsB[isegB];
                }
            }

            var carry = 0ul;
            for (int iseg = 0; iseg < nsegAB; iseg++) {
                accum[iseg] += carry;
                carry = accum[iseg] >> Stride;
                accum[iseg] &= ((1ul << Stride) - 1);
            }

            int widthC = a.Width + b.Width;
            var segsC = CreateArray(widthC);
            for (int iseg = 0; iseg < nsegAB; iseg++) {
                if (iseg < segsC.Length) {
                    segsC[iseg] = (UInt32)accum[iseg];
                }
                else {
#if DEBUG
                    if (accum[iseg].NotEq(0ul)) throw Log.Here().TestFailException();
#endif
                }
            }

            var bitsC = new BitArray(signedC, widthC, segsC, false);
            if (negative) bitsC.ArithInvertSelf();
            return bitsC;
        }

        public BitArray Div(BitArray b, out BitArray mod) {
            var a = this;
            var signed = a.Signed || b.Signed;
            a = a.Abs(out int signA);
            b = b.Abs(out int signB);
            var negative = (signA < 0) ^ (signB < 0);

            var q = DivCore(a, b, signed, -1, out int shift, out mod);
#if DEBUG
            string traceStr = null;
            if (Verbose) traceStr = "q=" + q.ToStringForDebug() + " >> " + shift;
#endif
            q.LogicShiftRightSelf(shift);
#if DEBUG
            if (Verbose) Log.Here().T(traceStr + " --> " + q.ToStringForDebug());
#endif
            if (negative) q.ArithInvertSelf();
            return q;
        }

        public static BitArray DivCore(BitArray a, BitArray b, bool signed, int width, out int shift, out BitArray mod) {
#if DEBUG
            if (a.Signed) throw Log.Here().ArgException(nameof(a) + nameof(BitArray.Signed));
            if (b.Signed) throw Log.Here().ArgException(nameof(b) + nameof(BitArray.Signed));
#endif
            if (b.IsZero) throw Log.Here().E(new DivideByZeroException());
            if (width <= 0) width = a.Width;

            if (a.IsZero) {
                shift = 0;
                mod = new BitArray(signed, b.Width);
                return new BitArray(signed, width);
            }

            var bw = b.Width;
            b = b.Trim(out int msbB, out _);

            var aw = a.Width;
            var msbA = a.FindMostSignificantBit();
            var lsbA = msbA + 1 - width;
            a = a.Clone(lsbA - (b.Width - 1), width + (b.Width - 1));
            shift = (aw - (msbA + 1)) + msbB;
#if DEBUG
            string traceString = null;
            if (Verbose) {
                Log.Here().T(
                    "msbA=" + msbA + ", " +
                    "lsbA=" + lsbA + ", " +
                    "a=" + a + ", " +
                    "b=" + b + ", " +
                    "msbB=" + msbB + ", " +
                    "shift=" + shift);
            }
#endif

            var q = new BitArray(signed, width + (signed ? 1 : 0));
            for (int ibitQ = width - 1; ibitQ >= 0; ibitQ--) {
#if DEBUG
                if (Verbose) traceString = "[" + ibitQ + "] " + "q=" + q + ", " + "a=" + a;
#endif
                if (a.compareForDiv(b, ibitQ) >= 0) {
                    a.subSelfForDiv(b, ibitQ);
                    q[ibitQ] = 1u;
                }
                else {
                    q[ibitQ] = 0u;
                }
#if DEBUG
                if (Verbose) Log.Here().T(traceString + " --> q[" + ibitQ + "]=" + q[ibitQ] + ", a=" + a);
#endif
            }

            mod = new BitArray(a.Signed, b.Width);
            return q;
        }

        // todo 性能改善 BitArray.FindMostSignificantBit()
        public int FindMostSignificantBit() {
            var higherBlankBit = IsNegative ? 1u : 0u;
            var w = Width;
            int msb = w - 1;
            while (this[msb] == higherBlankBit && msb >= 0) msb--;
            if (Signed) msb += 1;
            return msb;
        }

        // todo 性能改善 BitArray.FindLeastSignificantBit()
        public int FindLeastSignificantBit() {
            var w = Width;
            int lsb = 0;
            while (this[lsb] == 0u && lsb < w) lsb++;
            return lsb < w ? lsb : -1;
        }

        // todo 性能改善 BitArray.leftAliendCompare()
        private int compareForDiv(BitArray b, int offsetA) {
            var aw = this.Width;
            var bw = b.Width;
            for (int ibitB = bw; ibitB >= 0; ibitB--) {
                var diff = (int)this[offsetA + ibitB] - (int)b[ibitB];
                if (diff != 0) return diff;
            }
            return 0;
        }

        // todo 性能改善 BitArray.leftAliendSub()
        private void subSelfForDiv(BitArray b, int offsetA) {
            var aw = this.Width;
            var bw = b.Width;
            UInt32 carry = 0u;
            for (int ibitB = 0; ibitB < bw + 1; ibitB++) {
                UInt32 bit = this[offsetA + ibitB] - (b[ibitB] + carry);
                this[offsetA + ibitB] = bit & 1u;
                carry = (bit >> 1) & 1u;
            }
        }

        // todo 性能改善 BitArray.LeftAlignedCompareTo()
        private int leftAlignedCompareTo(BitArray b) {
            var aw = Width;
            var bw = b.Width;
            var w = Math.Max(aw, bw);
            var diff = AddSub(b, true, aw - w, bw - w, w);
            if (diff.IsNegative) return -1;
            if (diff.IsZero) return 0;
            return 1;
        }

        public BitArray Trim(out int msb, out int lsb) => trim(true, true, out msb, out lsb);
        private BitArray trim(bool leftTrim, bool rightTrim, out int msb, out int lsb) {
            if (IsZero) {
                msb = 0;
                lsb = 0;
                return new BitArray(Signed, 1);
            }
            msb = FindMostSignificantBit();
            lsb = FindLeastSignificantBit();
            return Clone(lsb, msb + 1 - lsb);
        }

        // todo 性能改善 BitArray.LogicShiftLeftSelf()
        public void LogicShiftLeftSelf(int shift) {
            if (shift < 0) throw Log.Here().ArgException(nameof(shift));
            if (shift == 0) return;
            var w = Width;
            for (int ibit = w - 1; ibit >= 0; ibit--) {
                this[ibit] = this[ibit - shift];
            }
            FillBlankSelf();
        }

        // todo 性能改善 BitArray.LogicShiftRightSelf()
        public void LogicShiftRightSelf(int shift) {
            if (shift < 0) throw Log.Here().ArgException(nameof(shift));
            if (shift == 0) return;
            var w = Width;
            for (int ibit = 0; ibit < w; ibit++) {
                this[ibit] = this[ibit + shift];
            }
            FillBlankSelf();
        }

        public IEnumerable<UInt32> EnumBits() => EnumBits(0, Width);
        public IEnumerable<UInt32> EnumBits(int start, int length) {
            var scan = new BitScan(this, start, length);
            length = Math.Abs(length);
            for (int i = 0; i < length; i++) {
                yield return scan.Read();
            }
        }

        public IEnumerable<UInt32> EnumSegments() => EnumSegments(0, Width);
        public IEnumerable<UInt32> EnumSegments(int start, int length) {
#if DEBUG
            if (length < 0) throw Log.Here().ArgException(nameof(length));
#endif
            var nseg = BitWidthToSegment(length);
            var scan = new SegScan(this, start);
            for (int iseg = 0; iseg < nseg; iseg++) {
                yield return scan.Read();
            }
        }

        public BitArray Clone() => Clone(0, Width, Signed);
        public BitArray Clone(int start, int width) => Clone(start, width, Signed);
        public BitArray Clone(int start, int width, bool signed) {
#if DEBUG
            if (width < 0) throw Log.Here().ArgException(nameof(width));
#endif
            // todo BitArray.Clone() アライメント取れてるときは Array.CopyTo を使う
            var segs = CreateArray(width);
            var scan = new SegScan(this, start);
            for (int iseg = 0; iseg < segs.Length; iseg++) {
                segs[iseg] = scan.Read();
            }
            return new BitArray(signed, width, segs, false);
        }

        public static UInt32[] CreateArray(int width) => new UInt32[BitWidthToSegment(width)];

        // ビット番号が負の場合も考慮して小さい方に丸める
        public static int BitIndexToSegment(int ibit, out int shift) {
            var iseg = ibit >> Clog2Stride;
            shift = ibit & (Stride - 1);
            return iseg;
        }

        // ビット番号が負の場合も考慮して大きい方に丸める
        public static int BitWidthToSegment(int width) {
#if DEBUG
            if (width < 1) throw Log.Here().ArgException(nameof(width));
#endif
            return (width + Stride - 1) >> Clog2Stride;
        }

        // todo BitArray.GetHashCode() もうちょっと真面目に実装
        public override int GetHashCode() {
            int hash = Signed ? Width : -Width;
            foreach (var seg in EnumSegments()) {
                hash = ((hash << 1) | (hash >> Stride - 1)) ^ (int)seg;
            }
            return hash;
        }

        public string FormatString => (Signed ? "s" : "u") + Width;

        public string ToStringForDebug() {
            try { return ToString() + "(" + ToDecimal() + ")"; }
            catch { return ToString(); }
        }

        public override string ToString() {
            return "0x" + ToBinaryString(Radix.Hexadecimal) + FormatString;
        }

        public string ToBinaryString(Radix radix) {
            var sb = new StringBuilder();
            ToBinaryString(radix, sb);
            return sb.ToString();
        }

        public void ToBinaryString(Radix radix, StringBuilder digits) {
            int w = Width;
            int digitWidth = radix.ToBinaryDigitBits();
            int shift = (w - 1) % digitWidth;
            UInt32 digit = 0u;
            var scan = new BitScan(this, w - 1, -w);
            for (int i = 0; i < w; i++) {
                var bit = scan.Read();
                digit |= bit << shift;
                if (shift-- <= 0 || i + 1 == w) {
                    digits.Append(DigitToChar(digit));
                    shift = digitWidth - 1;
                    digit = 0u;
                }
            }
        }

        public static char DigitToChar(uint digit) {
            if (0 <= digit && digit <= 9) return (char)('0' + digit);
            if (10 <= digit && digit <= 15) return (char)('a' + digit - 10);
            throw Log.Here().ArgException(nameof(digit));
        }

        // todo 性能改善 apfixed.decimal()
        public decimal ToDecimal() {
            var abs = Abs(out int sign);
            if (sign == 0) return 0m;
            int shift = 0;
            var tmp = 0m;
            foreach (var seg in abs.EnumSegments()) {
                tmp = tmp + (decimal)seg * MathEx.PowN(2m, shift);
                shift += Stride;
            }
            return sign * tmp;
        }

        public static BitArray operator -(BitArray a) => a.ArithInvert();

        public static BitArray operator +(BitArray a, BitArray b) => a.Add(b);
        public static BitArray operator -(BitArray a, BitArray b) => a.Sub(b);
        public static BitArray operator *(BitArray a, BitArray b) => a.Mul(b);
        public static BitArray operator /(BitArray a, BitArray b) => a.Div(b, out _);
        /*
        
        public static BitArray operator <<(BitArray a, int n) => a.LogicShiftLeft(n);
        public static BitArray operator >>(BitArray a, int n) => a.ArithShiftRight(n);
        */
        public static bool operator ==(BitArray a, BitArray b) => a.Equals(b);
        public static bool operator !=(BitArray a, BitArray b) => !a.Equals((object)b);
        public static bool operator >(BitArray a, BitArray b) => a.CompareTo(b) > 0;
        public static bool operator <(BitArray a, BitArray b) => a.CompareTo(b) < 0;
        public static bool operator >=(BitArray a, BitArray b) => a.CompareTo(b) >= 0;
        public static bool operator <=(BitArray a, BitArray b) => a.CompareTo(b) <= 0;


#if DEBUG
        public static void Test() {
            {
                var segs = new UInt32[] { 0x7654321fu, 0xfedcba98u };
                var widths = new int[] { 1, 2, 31, 32, 40, 63, 64 };
                var bools = new bool[] { false, true };
                foreach (var signed in bools) {
                    foreach (var w in widths) {
                        var bits = new BitArray(signed, w, segs, true);
                        doTestGetSegment(bits);
                    }
                }
            }
            doTestParse("0x0u8", "0u8");
            doTestParse("0x1234u16", "4660u16");
            doTestParse("0x12345678abcdu48", "20015998348237u48");
            doTestParse("0x92345678abcds48", "-120721490007091s48");
            doTestBinaryOp(
                "18716516548761564187165487973534u128", '+',
                "79456185330078728708723456062187u128",
                "98172701878840292895888944035721u129");
            doTestBinaryOp(
                "18716516548761564187165487973534u128", '-',
                "79456185330078728708723456062187u128",
                "-60739668781317164521557968088653s129");
            doTestBinaryOp("0x0u8", '*', "0x0u8", "0x0u16");
            doTestBinaryOp("0x10u8", '*', "0x10u8", "0x100u16");
            doTestBinaryOp("0x1234u16", '*', "0x5678u16", "0x6260060u32");
            doTestBinaryOp("0x12u8", '*', "0x3456u16", "0x3ae0cu24");
            doTestBinaryOp("0x1234u16", '*', "0x56u8", "0x61d78u24");
            doTestBinaryOp("0xabs8", '*', "0xcdefs16", "0x109fa5s24");
            doTestBinaryOp("0xffffffffu32", '*', "0xffffffffu32", "0xfffffffe00000001u64");
            doTestBinaryOp("0x100000000u33", '*', "0x100000000u33", "0x10000000000000000u66");

            doTestBinaryOp("0x1000u16", '/', "0x10u16", "0x100u16");
            doTestBinaryOp("0x5500u16", '/', "0x10u16", "0x550u16");
            doTestBinaryOp("0x5555u16", '/', "0x5u16", "0x1111u16");
            doTestBinaryOp("0x8000s16", '/', "0x100u16", "0x1ff80s17");
            doTestBinaryOp("0x89abs16", '/', "-10s8", "0xbd5s17");
            doTestBinaryOp("10000u16", '/', "3u16", "3333u16");
            doTestBinaryOp("-10000s16", '/', "3s16", "-3333s17");
            doTestBinaryOp("10000s16", '/', "-3s16", "-3333s17");
            doTestBinaryOp("-10000s16", '/', "-3s16", "3333s17");
            doTestBinaryOp("0x100000000000u48", '/', "0x10u8", "0x10000000000u48");
            doTestBinaryOp("10000000000000u48", '/', "3u8", "3333333333333u48");
        }

        private static void doTestGetSegment(BitArray bits) {
            var nseg = bits.NumSegments;
            var extSeg = bits.IsNegative ? ~(UInt32)0u : 0u;
            for (int iseg = -1; iseg <= nseg; iseg++) {
                var act = bits.GetSegment(iseg);
                var exp =
                    (iseg < 0) ? 0u :
                    (iseg < nseg - 1) ? bits.Segments[iseg] :
                    (iseg == nseg - 1) ? bits.Segments[iseg].FillBlank(bits.Signed, bits.LastSegmentWidth) :
                    extSeg;
                if (act.NotEqHex(exp)) throw Log.Here().TestFailException();
            }
        }

        private static void doTestParse(string aStr, string bStr) {
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
            BitArray cAct;
            switch (op) {
                case '+': cAct = a + b; break;
                case '-': cAct = a - b; break;
                case '*': cAct = a * b; break;
                case '/': cAct = a / b; break;
                default: throw new NotImplementedException();
            }
            string label = a.ToStringForDebug() + " " + op + " " + b.ToStringForDebug() + " = " + cAct.ToStringForDebug() + " != " + cExp.ToStringForDebug();
            if (cAct.Sign.NotEq(cExp.Sign)) throw Log.Here().TestFailException(label);
            if (cAct.Width.NotEq(cExp.Width)) throw Log.Here().TestFailException(label);
            if (cAct.NotEq(cExp)) throw Log.Here().TestFailException(label);
        }
#endif
    }
}
