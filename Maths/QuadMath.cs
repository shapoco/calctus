using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Maths {
    // todo Delete QMath
    //[Obsolete("Deprecated", false)]
    static class QuadMath {
        public static quad Log2(quad a) {
            //CalctusError.CheckArgMin(nameof(Log2), 0, )
            if (a <= 0) throw new ArgumentOutOfRangeException();

            // 整数部
            quad iLog = 0;
            while (a >= 2) {
                a /= 2;
                iLog++;
            }
            while (a < 1) {
                a *= 2;
                iLog--;
            }

            // 小数部
            quad fLog = 0;
            quad p = 1;
            for (int i = 0; i < ufixed113.NumBits; i++) {
                a *= a;
                p /= 2;
                if (a >= 2) {
                    a /= 2;
                    fLog += p;
                }
            }

            return iLog + fLog;
        }

        public static void Test() {
            Assert.Equal(nameof(QuadMath) + "." + nameof(Log2), Log2(1), 0);
            Assert.Equal(nameof(QuadMath) + "." + nameof(Log2), Log2(2), 1);
            Assert.Equal(nameof(QuadMath) + "." + nameof(Log2), Log2(4), 2);
            Assert.Equal(nameof(QuadMath) + "." + nameof(Log2), Log2(8), 3);
            Assert.Equal(nameof(QuadMath) + "." + nameof(Log2), Log2((quad)0.5m), -1);
            Assert.Equal(nameof(QuadMath) + "." + nameof(Log2), Log2((quad)0.25m), -2);
            Assert.Equal(nameof(QuadMath) + "." + nameof(Log2), Log2((quad)0.125m), -3);
        }
    }
}
