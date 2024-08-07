using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Maths.BitArrays {
    using segment = UInt32;
    using wide = UInt64;
    static class BitArrayEx {
        public const int Stride = sizeof(segment) * 8;

        public static segment GetBit(this segment[] segs, int pos)
            => (segs[pos / Stride] >> (pos % Stride)) & 1u;

        // todo 性能改善: BitArray.BitSet()
        public static void SetBit(this segment[] segs, int pos, segment value) {
            var w = segs[pos / Stride];
            w &= ~(1u << (pos % Stride));
            w |= (value & 1u) << (pos % Stride);
            segs[pos / Stride] = w;
        }

        // todo 性能改善: BitArray.CopyBits()
        public static void CopyBits(this segment[] src, int isrc, segment[] dest, int idest, int width) {
            for (int i = 0; i < width; i++) {
                var bit = (src[isrc / Stride] >> (isrc % Stride)) & 1u;
                var tmp = dest[idest / Stride];
                tmp &= ~(1u << (idest % Stride));
                tmp |= (bit << (idest % Stride));
                dest[idest / Stride] = tmp;
                isrc++; idest++;
            }
        }

        // todo 性能改善: BitArray.Extend()
        public static void ExtendSign(this segment[] segs, int pos) {
            var bit = (segs[pos / Stride] >> (pos % Stride)) & 1u;
            for (int i = pos + 1; i < segs.Length * Stride; i++) {
                SetBit(segs, i, bit);
            }
        }

        public static void LogicInvertSelf(this segment[] segs) {
            for (int i = 0; i < segs.Length; i++) {
                segs[i] = ~segs[i];
            }
        }

        public static void ArithInvertSelf(this segment[] segs, bool signed, int width) {
            segment carry = 1u;
            for (int i = 0; i < segs.Length; i++) {
                segs[i] = (~segs[i]).Add(0, carry, out carry);
            }
            segs.NormalizeBlankBitsSelf(signed, width);
        }

        public static void NormalizeBlankBitsSelf(this segment[] segs, bool signed, int width) {
            if (signed) {
                segs.ExtendSign(width - 1);
            }
            else if (width < segs.Length * Stride) {
                segs.SetBit(width, 0u);
                segs.ExtendSign(width);
            }
        }

        public static IEnumerable<segment> EnumBits(this segment[] segs, int start, int length) {
            var scanner = new BitScanner(segs, false, start, length);
            length = Math.Abs(length);
            for (int i = 0; i < length; i++) {
                yield return scanner.ReadNext();
            }
        }

        public static segment Add(this segment a, segment b, segment carryIn, out segment carryOut) {
#if DEBUG
            Assert.Equal(nameof(carryIn), carryIn & ~1u, 0u);
#endif
            wide tmp = (wide)a + b + carryIn;
            carryOut = (segment)(tmp >> Stride);
#if DEBUG
            Assert.Equal(nameof(carryOut), carryOut & ~1u, 0u);
#endif
            return (segment)tmp;
        }
    }
}
