using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Maths.BitArrays {
    class SegScan {
#if DEBUG
        public static bool Verbose = false;
#endif

        public const int Stride = BitArray.Stride;

        public readonly BitArray Bits;
        private int _iseg;
        private int _shift;
        private UInt32 _buff;

        public SegScan(BitArray bits, int start) {

            this.Bits = bits;
            this._iseg = BitArray.BitIndexToSegment(start, out _shift);
            if (_shift == 0) {
                this._buff = 0u;
            }
            else {
                this._buff = Bits.GetSegment(_iseg++);
            }
        }

        public UInt32 Read() {
#if DEBUG
            if (Verbose) Log.Here().T(
                "fmt=" + Bits.FormatString + ", " +
                "_iseg=" + _iseg + ", " +
                "_shift=" + _shift + ", " +
                "_buff=0x" + Convert.ToString(_buff, 16));
#endif
            UInt32 ret;
            if (_shift == 0) {
                ret = Bits.GetSegment(_iseg);
            }
            else {
                var lo = _buff;
                var hi = Bits.GetSegment(_iseg);
                _buff = hi;
                ret = hi.CatSlice(lo, _shift);
            }
#if DEBUG
            if (Verbose) Log.Here().T("ret=0x" +  Convert.ToString(ret, 16) );
#endif

            _iseg += 1;

            return ret;
        }

#if DEBUG
        public static void Test() {
            var segs = new UInt32[] { 0x7654321fu, 0xfedcba98u };
            var widths = new int[] { 1, 2, 17, 31, 32, 33, 45, 63, 64 };
            var starts = new int[] { 0, 1, 8, 31, 32, 40, -1, -10, -31, -32, -33, -40, -64 };
            var bools = new bool[] { false, true };
            foreach (var signed in bools) {
                foreach (var w in widths) {
                    foreach (var s in starts) {
                        var bits = new BitArray(signed, w, segs, true);
                        try { doTest(bits, s); }
                        catch { Verbose = true; doTest(bits, s); }
                    }
                }
            }
        }

        private static void doTest(BitArray inBits, int start) {
            var numSegs = inBits.NumSegments;
            var scan = new SegScan(inBits, start);
            var iseg = BitArray.BitIndexToSegment(start, out int shift);

            var segs = inBits.Segments;
            var extSeg = (inBits.IsNegative) ? 0xffffffffu : 0u;

            while (iseg <= numSegs) {
                var label =
                    "fmt=" + inBits.FormatString + ", " +
                    "start=" + start + ", " +
                    "iseg=" + iseg;
                if (0 <= iseg && iseg < numSegs)
                    label += ", segs[iseg]=0x" + Convert.ToString(segs[iseg], 16);
                if (0 <= iseg + 1 && iseg + 1 < numSegs)
                    label += ", segs[iseg+1]=0x" + Convert.ToString(segs[iseg + 1], 16);

                var act = scan.Read();
                var exp =
                    (iseg < -1) ? 0u :
                    (iseg == -1) ? segs[0].CatSlice(0u, shift) :
                    (iseg < numSegs - 1) ? segs[iseg + 1].CatSlice(segs[iseg], shift) :
                    (iseg == numSegs - 1) ? extSeg.CatSlice(segs[iseg], shift) :
                    extSeg;
                if (act.NotEqHex(exp)) throw Log.Here().TestFailException(label);
                iseg++;
            }
        }
#endif
    }
}
