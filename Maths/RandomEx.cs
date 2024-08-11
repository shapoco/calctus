using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Maths {
    class RandomEx {
        private readonly Random _rng = new Random();
        private UInt64 _sreg = 0ul;
        private int _bufferd = 0;

        private UInt32 fetch(int width) {
            while (_bufferd < width) {
                // Random.Next() は正の値(31bit) しか返さないので _sreg に溜めて下位を切り出す
                UInt64 tmp = (UInt64)_rng.Next();
                _sreg |= tmp << _bufferd;
                _bufferd += 31;
            }

            UInt32 ret = (UInt32)(_sreg & ((1ul << width) - 1));
            _sreg >>= width;
            _bufferd -= width;
            return ret;
        }

        public UInt32 Next32() => fetch(32);

        public UInt64 Next64() => fetch(32) | ((UInt64)fetch(32) << 32);

        public double NextDouble() {
            UInt64 h = fetch(27);
            UInt64 l = fetch(26);
            return  (double)((h << 26) | l) * (1.0 / 9007199254740992.0);
        }

        public decimal NextDecimal() {
            decimal h = Next64() % (UInt64)1e14;
            decimal l = Next64() % (UInt64)1e14;
            return (h * 1e14m + l) / 1e28m;
        }


    }
}
