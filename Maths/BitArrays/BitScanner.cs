using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Maths.BitArrays {
    using segment = UInt32;
    class BitScanner {
        public const int Stride = sizeof(segment) * 8;

        public bool WriteMode;
        public readonly int Direction;
        //private readonly FixedPointFormat fmt;
        private readonly segment[] segs;
        private segment work;
        private int coarse;
        private int fine;

        public BitScanner(segment[] segs, bool wr, int start, int dir) {
            this.WriteMode = wr;
            this.segs = segs;
            this.coarse = start / Stride;
            this.fine = start % Stride;
            if (dir >= 0) {
                this.Direction = 1;
                this.work = segs[coarse] >> fine;
            }
            else {
                this.Direction = -1;
                this.work = segs[coarse] << (31 - fine);
            }
        }

        public uint ReadNext() {
#if DEBUG
            if (WriteMode) throw Log.Here().E(new InvalidOperationException());
#endif
            if (Direction >= 0) {
                var ret = work & 1u;
                work >>= 1;
                if (++fine >= Stride) {
                    coarse += 1;
                    fine = 0;
                    work = coarse < segs.Length ? segs[coarse] : 0u;
                }
                return ret;
            }
            else {
                var ret = (work >> 31) & 1u;
                work <<= 1;
                if (fine-- <= 0) {
                    coarse -= 1;
                    fine = Stride - 1;
                    work = coarse >= 0 ? segs[coarse] : 0u;
                }
                return ret;
            }
        }
    }
}
