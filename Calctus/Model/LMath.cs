using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model {
    class LMath {
        public static long ToGray(long val) {
            return val ^ ((val >> 1) & 0x7fffffffffffffff);
        }

        public static long FromGray(long gray) {
            var bin = gray;
            for (int i = 1; i < 64; i++) {
                bin = bin ^ ((gray >> i) & 0x7fffffffffffffff);
            }
            return bin;
        }

        public static long SwapNibbles(long val) {
            return
                ((val & 0x0f0f0f0f0f0f0f0f) << 4) |
                ((val >> 4) & 0x0f0f0f0f0f0f0f0f);
        }

        public static long Swap2(long val) {
            return
                ((val & 0x00ff00ff00ff00ff) << 8) |
                ((val >> 8) & 0x00ff00ff00ff00ff);
        }

        public static long Swap4(long val) {
            return
                ((val & 0x000000ff000000ff) << 24) |
                ((val & 0x0000ff000000ff00) << 8) |
                ((val >> 8) & 0x0000ff000000ff00) |
                ((val >> 24) & 0x000000ff000000ff);
        }

        public static long Swap8(long val) {
            return
                ((val & 0x00000000000000ff) << 56) |
                ((val & 0x000000000000ff00) << 40) |
                ((val & 0x0000000000ff0000) << 24) |
                ((val & 0x00000000ff000000) << 8) |
                ((val >> 8) & 0x00000000ff000000) |
                ((val >> 24) & 0x0000000000ff0000) |
                ((val >> 40) & 0x000000000000ff00) |
                ((val >> 56) & 0x00000000000000ff);
        }

        public static long Revert(long val, int nbits) {
            if (nbits < 1 || 64 < nbits) throw new ArgumentOutOfRangeException();
            long ret = 0;
            for (int i = 0; i < nbits; i++) {
                ret <<= 1;
                ret |= (val & 1);
                val >>= 1;
            }
            return ret;
        }

        public static long RotateLeft(long val, int nbits) {
            if (nbits < 1 || 64 < nbits) throw new ArgumentOutOfRangeException();
            var carry = (val >> (nbits - 1)) & 1L;
            val &= (1L << (nbits - 1)) - 1L;
            val <<= 1;
            val |= carry;
            return val;
        }

        public static long RotateRight(long val, int nbits) {
            if (nbits < 1 || 64 < nbits) throw new ArgumentOutOfRangeException();
            var carry = (val & 1L) << (nbits - 1);
            val >>= 1;
            val &= (1L << (nbits - 1)) - 1L;
            val |= carry;
            return val;
        }

        public static long XorReduce(long val) {
            val ^= val >> 32;
            val ^= val >> 16;
            val ^= val >> 8;
            val ^= val >> 4;
            val ^= val >> 2;
            val ^= val >> 1;
            return val & 1L;
        }
    }
}
