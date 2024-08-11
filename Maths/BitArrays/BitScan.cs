using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Maths.BitArrays {
    class BitScan {
        public const int Stride = BitArray.Stride;

        public readonly int Step;
        public int Position => _ibit;

        public readonly BitArray Bits;
        private UInt32 _sign;
        private int _ibit;
        private bool _empty;
        private UInt32 _sreg;

        public BitScan(BitArray bits, int start, int dir) {
            this.Bits = bits;
            this.Step = dir >= 0 ? 1 : -1;
            this._ibit = start;
            this._sreg = 0u; // dummy
            this._empty = true;
            this._sign = bits.IsNegative ? 1u : 0u;
        }

        public UInt32 Read() {
#if false
            Console.WriteLine(
                "Step=" + Step + ", " +
                "_width=" + _width + ", " +
                "_sign=" + _sign + ", " +
                "_ibit=" + _ibit + ", " +
                "_empty=" + _empty + ", " +
                "_sreg=" + Convert.ToString(_sreg, 16));
#endif
            if (_ibit < 0) {
                _ibit += Step;
                _empty = true;
                return 0u;
            }
            else if (_ibit >= Bits.Width) {
                _ibit += Step;
                _empty = true;
                return _sign;
            }

            var iseg = BitArray.BitIndexToSegment(_ibit, out int shift);
            UInt32 ret;
            if (Step >= 0) {
                if (_empty) _sreg = Bits.Segments[iseg] >> shift;
                ret = _sreg & 1u;
                _sreg >>= 1;
                _empty = (shift >= Stride - 1);
            }
            else {
                if (_empty) _sreg = Bits.Segments[iseg] << (Stride - shift - 1);
                ret = (_sreg >> (Stride - 1)) & 1u;
                _sreg <<= 1;
                _empty = (shift <= 0);
            }
            _ibit += Step;
            return ret;
        }

#if DEBUG
        public static void Test() {
            var signedArray = new bool[] { false, true };
            foreach (var signed in signedArray) {
                var segs = new UInt32[] { 0x7654321fu, 0xfedcba98u };
                var w = segs.Length * Stride;
                var bits = new BitArray(signed, w, segs, false);
                doTest(bits, 0, w);
                doTest(bits, 5, w);
                doTest(bits, -5, w);
                doTest(bits, -5, w + 10);
                doTest(bits, w, -w);
                doTest(bits, w - 5, -w);
                doTest(bits, w + 5, -w);
                doTest(bits, w + 5, -w - 10);
            }
        }

        private static void doTest(BitArray bits, int start, int len) {
            var scan = new BitScan(bits, start, len);
            var ibit = start;
            var step = len >= 0 ? 1 : -1;
            len = Math.Abs(len);
            for (int i = start; i < len; i++) {
                var act = scan.Read();
                var exp = bits[ibit];
                if (act.NotEq(exp)) 
                    throw Log.Here().TestFailException(
                        "width=" + bits.Width + ", " +
                        (bits.Signed ? "signed" : "unsigned") + ", " +
                        "start=" + start + ", " +
                        "len=" + len + ", " +
                        "ibit=" + ibit);
                ibit += step;
            }
        }
#endif
    }
}
