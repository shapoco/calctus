using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.Mathematics {
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

        public static long Reverse(long val, int nbits) {
            if (nbits < 1 || 64 < nbits) throw new ArgumentOutOfRangeException();
            long ret = 0;
            for (int i = 0; i < nbits; i++) {
                ret <<= 1;
                ret |= (val & 1);
                val >>= 1;
            }
            return ret;
        }

        public static long ReverseBytes(long val) {
            return
                ((val & 0x0101010101010101) << 7) |
                ((val & 0x0202020202020202) << 5) |
                ((val & 0x0404040404040404) << 3) |
                ((val & 0x0808080808080808) << 1) |
                ((val >> 1) & 0x0808080808080808) |
                ((val >> 3) & 0x0404040404040404) |
                ((val >> 5) & 0x0202020202020202) |
                ((val >> 7) & 0x0101010101010101);
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

        public static int XorReduce(long val) {
            val ^= val >> 32;
            val ^= val >> 16;
            val ^= val >> 8;
            val ^= val >> 4;
            val ^= val >> 2;
            val ^= val >> 1;
            return (int)(val & 1L);
        }

        public static int OddParity(long val) => XorReduce(val) ^ 1;

        public static int CountOnes(long val) {
            val = (val & 0x5555555555555555) + ((val >> 1) & 0x5555555555555555);
            val = (val & 0x3333333333333333) + ((val >> 2) & 0x3333333333333333);
            val = (val & 0x0f0f0f0f0f0f0f0f) + ((val >> 4) & 0x0f0f0f0f0f0f0f0f);
            val = (val & 0x00ff00ff00ff00ff) + ((val >> 8) & 0x00ff00ff00ff00ff);
            val = (val & 0x0000ffff0000ffff) + ((val >> 16) & 0x0000ffff0000ffff);
            val = (val & 0x00000000ffffffff) + ((val >> 32) & 0x00000000ffffffff);
            return (int)val;
        }

        public static readonly long[] EccXorMask = {
            unchecked((long)0xab55555556aaad5b),
            unchecked((long)0xcd9999999b33366d),
            unchecked((long)0xf1e1e1e1e3c3c78e),
            unchecked((long)0x01fe01fe03fc07f0),
            unchecked((long)0x01fffe0003fff800),
            unchecked((long)0x01fffffffc000000),
            unchecked((long)0xfe00000000000000),
        };

        public static readonly int[] EccCorrectionTable = {
             0, -1, -2,  1, -3,  2,  3,  4, -4,  5,  6,  7,  8,  9, 10, 11,
            -5, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26,
            -6, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41,
            42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57,
            -7, 58, 59, 60, 61, 62, 63, 64,
        };

        public static int EccWidth(int dataWidth) {
            if (dataWidth < 1 || 1024 < dataWidth) throw new ArgumentOutOfRangeException();
            int eccWidth = (int)RMath.Ceiling(RMath.Log2(dataWidth + 1));
            if (eccWidth + dataWidth >= (1 << eccWidth)) {
                eccWidth += 1;
            }
            return eccWidth + 1;
        }

        public static int EccEncode(long data, int dataWidth) {
            if (dataWidth < 1 || 64 < dataWidth) throw new ArgumentOutOfRangeException();
            var eccWidth = EccWidth(dataWidth);
            var ecc = 0;
            for (int i = 0; i < eccWidth - 1; i++) {
                var bit = XorReduce(data & EccXorMask[i]);
                ecc |= bit << i;
            }
            ecc |= (OddParity(ecc) ^ OddParity(data)) << (eccWidth - 1);
            return ecc;
        }

        public static int EccDecode(int ecc, long data, int dataWidth) {
            var parity = OddParity(ecc) ^ OddParity(data);
            var eccWidth = EccWidth(dataWidth);
            var syndrome = ecc ^ EccEncode(data, dataWidth);
            syndrome &= (1 << (eccWidth - 1)) - 1;
            var errPos = EccCorrectionTable[syndrome];
            if (parity == 0) {
                if (errPos == 0) {
                    return 0; // no error
                }
                else {
                    return -1; // 2-bit error
                }
            }
            else {
                if (errPos == 0) {
                    return dataWidth + eccWidth; // parity error
                }
                else if (errPos < 0) {
                    return dataWidth - errPos; // 1-bit error in ECC bits
                }
                else {
                    return errPos; // 1-bit error in data bits
                }
            }
        }

        /// <summary>パッキング</summary>
        public static long Pack(int elemWidth, long[] array) {
            if (elemWidth < 1) throw new ArgumentOutOfRangeException();
            if (elemWidth * array.Length > 64) throw new ArgumentOutOfRangeException();
            long buff = 0;
            for (int i = 0; i < array.Length; ++i) {
                buff |= array[i] << (elemWidth * i);
            }
            return buff;
        }

        /// <summary>アンパッキング</summary>
        public static long[] Unpack(int elemWidth, long val) {
            if (elemWidth < 1) throw new ArgumentOutOfRangeException();
            var list = new List<long>();
            while (val != 0) {
                list.Add(val & ((1L << elemWidth) -1));
                val >>= elemWidth;
                val &= (1L << (64 - elemWidth)) - 1L;
            }
            return list.ToArray();
        }


    }
}
