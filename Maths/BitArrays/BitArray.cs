using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Texts;

namespace Shapoco.Maths.BitArrays {
    struct BitArray : IComparable<BitArray> {
        public static readonly BitArray Empty = new BitArray(false);

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

        public IntFormat Format => new IntFormat(_signed, Width);

        public int SignWidth => Signed ? 1 : 0;
        public int WidthWithoutSign => Signed ? Width - 1 : Width;
        public int LastSegmentWidth => ((Width - 1) & (Stride - 1)) + 1;

        public UInt32 Msb => this[Width - 1];
        public bool IsNegative => Signed && (Msb != 0u);
        public bool IsZero => EnumSegments().All(x => x == 0u);
        public int Sign => IsNegative ? -1 : IsZero ? 0 : 1;

        public int NumSegments => BitWidthToSegment(Width);

        public static BitArray FromInt(int n) {
            int width = MathEx.CeilLog2((uint)Math.Abs(n));
            bool signed = (n < 0);
            if (signed) width += 1;
            return FromInt(new IntFormat(signed, width), n);
        }

        public static BitArray FromInt(IntFormat fmt, int n)
            => new BitArray(fmt, new UInt32[] { (UInt32)n }, false);

        public static BitArray Parse(string s) {
            var lexer = new SimpleLexer(s, autoSkipWhite: false);
            lexer.SkipWhite();

            bool minus = lexer.TryPop("-");
            lexer.SkipWhite();

            Radix radix = Radix.Decimal;
            if (lexer.TryPop("0x")) radix = Radix.Hex;
            else if (lexer.TryPop("0b")) radix = Radix.Bin;
            else if (lexer.TryPop("0o")) radix = Radix.Oct;

            if (minus && radix != Radix.Decimal) {
                throw Log.Here().E(lexer.Input.CreateException("Minus symbol can be applied only for decimal."));
            }

            lexer.TryPopDigitSequence(radix, out byte[] digits);

            IntFormat fmt = new IntFormat(false , 0);

            char sign;
            if (lexer.TryPop(new char[] { 'u', 's' }, out sign)) {
                lexer.TryPopDigitSequence(Radix.Hex, out byte[] intWidthDigits);
                fmt.Width = (int)SimpleLexer.DigitsAsInteger(intWidthDigits, Radix.Decimal);
            }
            else if (radix == Radix.Decimal) {
                throw Log.Here().E(lexer.Input.CreateExpectedException("'u' or 's'"));
            }
            else {
                sign = 'u';
                fmt.Width = digits.Length * radix.ToBinaryDigitBits();
            }
            lexer.SkipWhite();
            lexer.AssertEos();

            fmt.Signed = (sign == 's');
            if (radix == Radix.Decimal) {
                return FromDecimalDigits(fmt, minus, digits);
            }
            else {
                return FromBinaryDigits(fmt, radix, digits);
            }
        }

        public static BitArray FromBinaryDigits(IntFormat fmt, Radix radix, byte[] digits) {
            int step = radix.ToBinaryDigitBits();
            var bits = new BitArray(fmt);
            int ibit = 0;
            for (int i = digits.Length - 1; i >= 0; i--) {
                byte digit = digits[i];
                for (int j = 0; j < step; j++) {
                    if (ibit >= fmt.Width) break;
                    bits[ibit++] = digit & 1u;
                    digit >>= 1;
                }
                if (ibit >= fmt.Width) break;
            }
            bits.FillBlankSelf();
            return bits;
        }

        public static BitArray FromDecimalDigits(IntFormat fmt, bool minus, byte[] digits) {
            if (!fmt.Signed && minus) throw Log.Here().ArgException(nameof(fmt.Signed) + "=false, " + nameof(minus) + "=true");
            var bits = new BitArray(fmt);
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

        public BitArray(IntFormat fmt, UInt32[] segs, bool forceCopy) {
#if DEBUG
            if (fmt.Width < 1) throw Log.Here().ArgException(nameof(fmt.Width));
#endif
            var nsegs = BitWidthToSegment(fmt.Width);
            if (segs.Length != nsegs || forceCopy) {
                var newArray = CreateArray(fmt.Width);
                Array.Copy(segs, newArray, Math.Min(nsegs, segs.Length));
                segs = newArray;
            }

            this._signed = fmt.Signed;
            this.Width = fmt.Width;
            this.Segments = segs;

            FillBlankSelf();
        }

        public BitArray(bool signed, int width, UInt32[] segs, bool forceCopy)
            : this(new IntFormat(signed, width), segs, forceCopy) { }

        public BitArray(IntFormat fmt)
            : this(fmt, CreateArray(fmt.Width), false) { }

        public BitArray(bool signed, int width)
            : this(new IntFormat( signed, width), CreateArray(width), false) { }

        private BitArray(bool dummy) {
            this._signed = false;
            this.Width = 0;
            this.Segments = null;
        }

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
#if true
            for (int ibit = from; ibit < toExclusive; ibit++) {
                this[ibit] = value;
            }
#else
            int segFrom = BitWidthToSegment(from);
            int segTo = BitIndexToSegment(toExclusive, out _);
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
#endif
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
            var w = Width + (Signed ? 0 : 1);
            var bits = Clone(0, true, w);
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
                // todo b.Clone(0, true, b.Width) --> AsSigned()
                b = b.Clone(0, true, b.Width + 1);
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

        public BitArray IntDiv(BitArray b, out BitArray m) {
#if true
            return DivCoreSigned(this, b, -1, out m, false, out _);
#else
            var a = this;
            var signedQ = a.Signed || b.Signed; // 商の符号有無
            var signedM = a.Signed; // 余の符号有無
            var extendM = a.Signed && !b.Signed; // 余を符号の分だけ拡張するか否か
            a = a.Abs(out int signA);
            b = b.Abs(out int signB);
            var negativeQ = (signA < 0) ^ (signB < 0); // 商は負
            var negativeM = (signA < 0); // 余は負
            var qFmt = new IntFormat(signedQ, a.Width);
            var q = DivCoreUnsigned(a, b, qFmt, IntFormat.Empty, out m, false, out _); // 整数除算実行
            if (negativeQ) q.ArithInvertSelf(); // 商の符号反転
            if (signedM) m = m.Clone(0, true,  m.Width + (extendM ? 1 : 0)); // 
            if (negativeM) m.ArithInvertSelf();
            return q;
#endif
        }

        public static BitArray DivCoreSigned(BitArray a, BitArray b, int qWidth, out BitArray m, bool fracMode, out int fracWidth) {
            var qSigned = a.Signed || b.Signed; // 商の符号有無
            var mSigned = a.Signed; // 余の符号有無
            if (qWidth <= 0) {
                // 商の幅の自動決定
                qWidth = a.Width;
                if (qSigned && !a.Signed) qWidth += 1;
            }
            var mWidth = b.Width; // 余のビット幅
            if (a.Signed && !b.Signed) mWidth++; // 余のビットを符号の分だけ拡張
            a = a.Abs(out int aSign);
            b = b.Abs(out int bSign);
            var qNegative = (aSign < 0) ^ (bSign < 0); // 商は負
            var mNegative = (aSign < 0); // 余は負
            var qFmt = new IntFormat(qSigned, qWidth);
            var mFmt = new IntFormat(mSigned, mWidth);
            var q = DivCoreUnsigned(a, b, qFmt, mFmt, out m, fracMode, out fracWidth); // 除算実行
            if (qNegative) q.ArithInvertSelf(); // 商の符号反転
            if (mNegative) m.ArithInvertSelf(); // 余の符号反転
            return q;
        }

        // todo 性能改善: BitArray.DivCoreUnsigned
        public static BitArray DivCoreUnsigned(BitArray a, BitArray b, IntFormat qFmt, IntFormat mFmt, out BitArray m, bool fracMode, out int fracWidth) {
#if DEBUG
            if (a.Signed) throw Log.Here().ArgException(nameof(a) + "." + nameof(a.Signed));
            if (b.Signed) throw Log.Here().ArgException(nameof(b) + "." + nameof(b.Signed));
#endif
            if (b.IsZero) throw Log.Here().E(new DivideByZeroException());
            if (qFmt.Width <= 0) qFmt.Width = a.Width;
            if (mFmt.Width <= 0) mFmt.Width = b.Width;

            if (a.IsZero) {
                fracWidth = 0;
                m = new BitArray(mFmt);
                return new BitArray(qFmt);
            }

            var aOrigWidth = a.Width;
            var aMsb = a.FindMostSignificantBit(true);

            int bMsb, xLsb, xWidth, qMsb;
            if (fracMode) {
                b = b.Trim(out bMsb, out _);
                xLsb = aMsb + 1 - qFmt.Width - (b.Width - 1);
                xWidth = qFmt.Width + (b.Width - 1);
                qMsb = qFmt.Width - 1;
                fracWidth = (aOrigWidth - (aMsb + 1)) + bMsb;
            }
            else {
                b = b.TrimLeft(out bMsb);
                xLsb = 0;
                xWidth = aMsb + 1;
                qMsb = qFmt.Width - b.Width;
                fracWidth = 0;
            }

            //var lsbA = a.FindLeastSignificantBit();

            var x = a.Clone(xLsb, xWidth);
#if DEBUG
            string traceString = null;
            if (Verbose) {
                Log.Here().T(
                    "aMsb=" + aMsb + ", " +
                    //"lsbA=" + lsbA + ", " +
                    "xLsb=" + xLsb + ", " +
                    "xWidth=" + xWidth + ", " +
                    "x=" + x + ", " +
                    "b=" + b + ", " +
                    "bMsb=" + bMsb + ", " +
                    "fracWidth=" + fracWidth);
            }
#endif

            var q = new BitArray(qFmt);
            for (int ibitQ = qMsb; ibitQ >= 0; ibitQ--) {
#if DEBUG
                if (Verbose) traceString = "[" + ibitQ + "] " + "q=" + q + ", " + "x=" + x;
#endif
                if (x.compareForDiv(b, ibitQ) >= 0) {
                    x.subSelfForDiv(b, ibitQ);
                    q[ibitQ] = 1u;
                }
                else {
                    q[ibitQ] = 0u;
                }
#if DEBUG
                if (Verbose) Log.Here().T(traceString + " --> q[" + ibitQ + "]=" + q[ibitQ] + ", x=" + x);
#endif
            }

            m = x.Clone(0, mFmt);
            return q;
        }

        // todo 性能改善 BitArray.FindMostSignificantBit()
        public int FindMostSignificantBit(bool skipNegativeSignBits) {
            var higherBlankBit = (IsNegative && skipNegativeSignBits) ? 1u : 0u;
            var w = Width;
            int msb = w - 1;
            while (this[msb] == higherBlankBit && msb >= 0) msb--;
            if (Signed && skipNegativeSignBits) msb += 1;
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
                int ibitA = offsetA + ibitB;
                if (ibitA >= aw) break;
                UInt32 bit = this[ibitA] - (b[ibitB] + carry);
                this[ibitA] = bit & 1u;
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

        public BitArray TrimLeft(out int msb) => trim(true, false, out msb, out _);
        public BitArray Trim(out int msb, out int lsb) => trim(true, true, out msb, out lsb);
        private BitArray trim(bool leftTrim, bool rightTrim, out int msb, out int lsb) {
            if (IsZero) {
                msb = 0;
                lsb = 0;
                return new BitArray(Signed, 1);
            }
            msb = leftTrim ? FindMostSignificantBit(true) : Width - 1;
            lsb = rightTrim ? FindLeastSignificantBit() : 0;
            return Clone(lsb, msb + 1 - lsb);
        }

        // todo 性能改善 BitArray.LogicShiftLeftSelf()
        public void ShiftLeftSelf(int shift, ShiftMode mode) {
            if (shift < 0) throw Log.Here().ArgException(nameof(shift));
            if (shift == 0) return;
            bool arithMode = false;
            switch (mode) {
                case ShiftMode.Auto: arithMode = Signed; break;
                case ShiftMode.Logical: arithMode = false; break;
                case ShiftMode.Arithmetic: arithMode = true; break;
                default: throw Log.Here().E(new NotImplementedException());
            }
            var loopStart = arithMode ? (Width - 2) : (Width - 1);
            for (int ibit = loopStart; ibit >= 0; ibit--) {
                this[ibit] = this[ibit - shift];
            }
            FillBlankSelf();
        }

        public BitArray ShiftLeft(int shift, ShiftMode mode) {
            var ret = Clone();
            ret.ShiftLeftSelf(shift, mode);
            return ret;
        }

        // todo 性能改善 BitArray.LogicShiftRightSelf()
        public void ShiftRightSelf(int shift, ShiftMode mode) {
            if (shift < 0) throw Log.Here().ArgException(nameof(shift));
            if (shift == 0) return;
            var w = Width;
            bool arithMode = false;
            switch (mode) {
                case ShiftMode.Auto: arithMode = Signed; break;
                case ShiftMode.Logical: arithMode = false; break;
                case ShiftMode.Arithmetic: arithMode = true; break;
                default: throw Log.Here().E(new NotImplementedException());
            }
            var overMsb = arithMode ? Msb : 0u;
            var loopEnd = arithMode ? (w - 1) : w;
            for (int ibit = 0; ibit < loopEnd; ibit++) {
                int ibitSrc = ibit + shift;
                if (ibitSrc < w) {
                    this[ibit] = this[ibitSrc];
                }
                else {
                    this[ibit] = overMsb;
                }
            }
            FillBlankSelf();
        }

        public BitArray ShiftRight(int shift, ShiftMode mode) {
            var ret = Clone();
            ret.ShiftRightSelf(shift, mode);
            return ret;
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

        public BitArray Clone() => Clone(0, new IntFormat(Signed, Width));
        public BitArray Clone(int start, int width) => Clone(start, new IntFormat(Signed, width));
        public BitArray Clone(int start, bool signed, int width) => Clone(start, new IntFormat(signed, width));
        public BitArray Clone(int start, IntFormat fmt) {
#if DEBUG
            if (fmt.Width < 0) throw Log.Here().ArgException(nameof(fmt.Width));
#endif
            // todo BitArray.Clone() アライメント取れてるときは Array.CopyTo を使う
            var segs = CreateArray(fmt.Width);
            var scan = new SegScan(this, start);
            for (int iseg = 0; iseg < segs.Length; iseg++) {
                segs[iseg] = scan.Read();
            }
            return new BitArray(fmt, segs, false);
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
            return "0x" + ToBinaryString(Radix.Hex, true) + FormatString;
        }

        public string ToBinaryString(Radix radix, bool allowShrink) {
            var sb = new StringBuilder();
            ToBinaryString(radix, sb, allowShrink);
            return sb.ToString();
        }

        public void ToBinaryString(Radix radix, StringBuilder sb, bool allowShrink) {
            int w = Width;
            int msb = (IsNegative || !allowShrink) ? (w - 1) : Math.Max(0, FindMostSignificantBit(false));
            int digitWidth = radix.ToBinaryDigitBits();
            int shift = msb % digitWidth;
            UInt32 digit = 0u;
            var scan = new BitScan(this, msb, -msb - 1);
            for (int i = 0; i <= msb; i++) {
                var bit = scan.Read();
                digit |= bit << shift;
                if (shift-- <= 0 || i == msb) {
                    sb.Append(DigitToChar(digit));
                    shift = digitWidth - 1;
                    digit = 0u;
                }
            }
        }

        public string ToRawDecimalString() {
            var sb = new StringBuilder();
            ToRawDecimalString(sb);
            return sb.ToString();
        }

        public void ToRawDecimalString(StringBuilder sb) {
            var bits = this.Clone(0, false, this.Width);
            var ten = BitArray.FromInt(10);
            var stack = new Stack<char>();
            do {
                bits = bits.IntDiv(ten, out BitArray m);
                stack.Push(CStyleBinary.ToChar((int)m.ToDecimal()));
            } while (!bits.IsZero);
            while (stack.Count > 0) sb.Append(stack.Pop());
        }

        public static char DigitToChar(uint digit) {
            if (0 <= digit && digit <= 9) return (char)('0' + digit);
            if (10 <= digit && digit <= 15) return (char)('a' + digit - 10);
            throw Log.Here().ArgException(nameof(digit));
        }

        // todo 性能改善 apfixed.ToDecimal()
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

        // todo 性能改善 apfixed.ToInt()
        public int ToInt32(bool allowOverflow) {
            var dec = ToDecimal();
            if (!allowOverflow && (dec <= int.MinValue || int.MaxValue <= dec)) {
                throw Log.Here().E(new InvalidCastException("The value exceeds the Int32 value range."));
            }
            return (int)dec;
        }

        public static BitArray operator -(BitArray a) => a.ArithInvert();

        public static BitArray operator +(BitArray a, BitArray b) => a.Add(b);
        public static BitArray operator -(BitArray a, BitArray b) => a.Sub(b);
        public static BitArray operator *(BitArray a, BitArray b) => a.Mul(b);
        public static BitArray operator /(BitArray a, BitArray b) => a.IntDiv(b, out _);
        public static BitArray operator %(BitArray a, BitArray b) { a.IntDiv(b, out BitArray mod); return mod; }
        
        // << / >> だけでは論理シフトなのか算術シフトなのか曖昧なので定義しない
        // public static BitArray operator <<(BitArray a, int n) => a.LogicShiftLeft(n);
        // public static BitArray operator >>(BitArray a, int n) => a.ArithShiftRight(n);

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

            testParse("0x0u8", "0u8");
            testParse("0x1234u16", "4660u16");
            testParse("0x12345678abcdu48", "20015998348237u48");
            testParse("0x92345678abcds48", "-120721490007091s48");

            testBinaryOp(
                "18716516548761564187165487973534u128", "+",
                "79456185330078728708723456062187u128",
                "98172701878840292895888944035721u129");

            testBinaryOp(
                "18716516548761564187165487973534u128", "-",
                "79456185330078728708723456062187u128",
                "-60739668781317164521557968088653s129");

            testBinaryOp("0x0u8", "*", "0x0u8", "0x0u16");
            testBinaryOp("0x10u8", "*", "0x10u8", "0x100u16");
            testBinaryOp("0x1234u16", "*", "0x5678u16", "0x6260060u32");
            testBinaryOp("0x12u8", "*", "0x3456u16", "0x3ae0cu24");
            testBinaryOp("0x1234u16", "*", "0x56u8", "0x61d78u24");
            testBinaryOp("0xabs8", "*", "0xcdefs16", "0x109fa5s24");
            testBinaryOp("0xffffffffu32", "*", "0xffffffffu32", "0xfffffffe00000001u64");
            testBinaryOp("0x100000000u33", "*", "0x100000000u33", "0x10000000000000000u66");

            testBinaryOp("0x1000u16", "/", "0x10u16", "0x100u16");
            testBinaryOp("0x5500u16", "/", "0x10u16", "0x550u16");
            testBinaryOp("0x5555u16", "/", "0x5u16", "0x1111u16");
            testBinaryOp("0x5555u16", "/", "-5s16", "-4369s17");
            testBinaryOp("0x8000s16", "/", "0x100u16", "0x1ff80s16");
            testBinaryOp("0x89abs16", "/", "-10s8", "0xbd5s16");
            testBinaryOp("10000u16", "/", "3u16", "3333u16");
            testBinaryOp("-10000s16", "/", "3s16", "-3333s16");
            testBinaryOp("10000s16", "/", "-3s16", "-3333s16");
            testBinaryOp("-10000s16", "/", "-3s16", "3333s16");
            testBinaryOp("0x100000000000u48", "/", "0x10u8", "0x10000000000u48");
            testBinaryOp("10000000000000u48", "/", "3u8", "3333333333333u48");

            testBinaryOp("0u16", "%", "5u8", "0u8");
            testBinaryOp("3u16", "%", "5u8", "3u8");
            testBinaryOp("5u16", "%", "5u8", "0u8");
            testBinaryOp("7u16", "%", "5u8", "2u8");
            testBinaryOp("10u16", "%", "5u8", "0u8");
            testBinaryOp("14u16", "%", "5u8", "4u8");
            testBinaryOp("1234u16", "%", "1u8", "0u8");
            testBinaryOp("1234u16", "%", "100u8", "34u8");
            testBinaryOp("-1234s16", "%", "100u8", "-34s9");
            testBinaryOp("1234u16", "%", "-100s8", "34u8");
            testBinaryOp("-1234s16", "%", "-100s8", "-34s8");
            testBinaryOp("123456789123456789u57", "%", "333333333333u39", "122456913579u39");
            testBinaryOp("-123456789123456789s58", "%", "333333333333u39", "-122456913579s40");
            testBinaryOp("123456789123456789u57", "%", "-333333333333s40", "122456913579u40");
            testBinaryOp("-123456789123456789s58", "%", "-333333333333s40", "-122456913579s40");

            {
                var bools = new bool[] { false, true };
                var ops = new string[] { "<<", ">>", "<<<", ">>>" };
                var patterns = new string[] {
                    "x", // 1
                    "xy", // 2
                    "xxy", // 3
                    "xyyy", // 4
                    "xxxyxyyy", // 8
                    "xyyyxxyyxxyyxxxy", // 16
                    "xyyyxxxxyyyyxxyyxxyyxxxxyyyyxxy", // 31
                    "xyyyxxxxyyyyxxyyxxyyxxxxyyyyxxxy", // 32
                    "xyyyxxxxyyyyxxyyxxyyxxxxyyyyxxxxy", // 33
                    "xyyyxxxyxxxxyyyyxxxxxxxxyyyyyyyyxxxxyyyyxxxxxxxxyyyyxxxxyxxxyyx", // 63
                    "xyyyxxxyxxxxyyyyxxxxxxxxyyyyyyyyxxxxyyyyxxxxxxxxyyyyxxxxyxxxyyyx", // 64
                    "xyyyxxxyxxxxyyyyxxxxxxxxyyyyyyyyxxxxyyyyxxxxxxxxyyyyxxxxyxxxyyyyx", // 65
                    "xyyyxxxyxxxxyyyyxxxxxxxxyyyyyyyyxxxxxxxxyyyyyyyyxxxxxxxxyyyyxxxxyxxxyyyx", // 72
                };

                foreach (var signed in bools) {
                    foreach (var op in ops) {
                        foreach (var polarity in bools) {
                            foreach (var ptn in patterns) {
                                string ibits = polarity ?
                                    ptn.Replace('x', '0').Replace('y', '1') :
                                    ptn.Replace('x', '1').Replace('y', '0');
                                var w = ibits.Length;
                                testShift(signed, op, ibits, 0);
                                if (w >= 2) testShift(signed, op, ibits, 1);
                                if (w >= 4) testShift(signed, op, ibits, w / 2);
                                if (w >= 16) {
                                    testShift(signed, op, ibits, w * 1 / 5);
                                    testShift(signed, op, ibits, w * 4 / 5);
                                }
                                if (w >= 3) testShift(signed, op, ibits, w - 1);
                                testShift(signed, op, ibits, w);
                                testShift(signed, op, ibits, w + 1);
                            }
                        }
                    }
                }
            }
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

        private static void testParse(string aStr, string bStr) {
            var a = Parse(aStr);
            var b = Parse(bStr);
            string label = a.ToStringForDebug() + " != " + b.ToStringForDebug();
            if (a.NotEq(b)) throw Log.Here().TestFailException(label);
            if (a.Sign.NotEq(b.Sign)) throw Log.Here().TestFailException(label);
            if (a.Width.NotEq(b.Width)) throw Log.Here().TestFailException(label);
        }

        private static void testShift(bool signed, string op, string ibits, int shift) {
            var w = ibits.Length;
            char sign = signed ? 's' : 'u';
            string obits;
            switch (op) {
                case "<<": obits = (ibits + new string('0', shift)).Substring(shift, w); break;
                case "<<<": obits = ibits[0] + (ibits + new string('0', shift)).Substring(shift + 1, w - 1); break;
                case ">>": obits = (new string('0', shift) + ibits).Substring(0, w); break;
                case ">>>": obits = (new string(ibits[0], shift) + ibits).Substring(0, w); break;
                default: throw Log.Here().F(new InvalidOperationException());
            }
            ibits = "0b" + ibits + sign + w.ToString();
            obits = "0b" + obits + sign + w.ToString();
            //Log.Here().T(ibits + " " + op + " " + shift + "u8 --> " + obits);
            testBinaryOp(ibits, op, shift + "u8", obits);
        }

        private static void testBinaryOp(string aStr, string op, string bStr, string cStr) {
            try { doTestBinaryOpInner(aStr, op, bStr, cStr); }
            catch { Verbose = true; doTestBinaryOpInner(aStr, op, bStr, cStr); }
        }

        private static void doTestBinaryOpInner(string aStr, string op, string bStr, string cStr) {
            var a = Parse(aStr);
            var b = Parse(bStr);
            var cExp = Parse(cStr);
            BitArray cAct;
            switch (op) {
                case "+": cAct = a + b; break;
                case "-": cAct = a - b; break;
                case "*": cAct = a * b; break;
                case "/": cAct = a / b; break;
                case "%": cAct = a % b; break;
                case "<<": cAct = a.ShiftLeft(b.ToInt32(false), ShiftMode.Logical); break;
                case ">>": cAct = a.ShiftRight(b.ToInt32(false), ShiftMode.Logical); break;
                case "<<<": cAct = a.ShiftLeft(b.ToInt32(false), ShiftMode.Arithmetic); break;
                case ">>>": cAct = a.ShiftRight(b.ToInt32(false), ShiftMode.Arithmetic); break;
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
