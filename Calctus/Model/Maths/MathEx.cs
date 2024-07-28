using System;
using System.Collections.Generic;
using Shapoco.Calctus.Model.Maths.Types;

namespace Shapoco.Calctus.Model.Maths {
    static class MathEx {
        public static bool IsInteger(this decimal x) => x == Math.Floor(x);

        /// <summary>a と b の最大公約数</summary>
        public static decimal Gcd(decimal a, decimal b) {
            Assert.ArgIsInteger(nameof(Gcd), nameof(a), a);
            Assert.ArgIsInteger(nameof(Gcd), nameof(b), b);
            a = Math.Abs(a);
            b = Math.Abs(b);
            while (b != 0) {
                var tmp = a;
                a = b;
                b = tmp % b;
            }
            return a;
        }

        // 指数関数
        public static decimal Pow(decimal a, decimal b) {
            if (Math.Floor(b) == b && int.MinValue <= b && b <= int.MaxValue) {
                return PowN(a, (int)b);
            }
            else {
                return Exp(Log(a) * b);
            }
        }

        public static decimal PowN(decimal xReal, int y) {
            decimal x = xReal;
            decimal ret = 1;
            if (y > 0) {
                while (true) {
                    if ((y & 1) != 0) ret *= x;
                    y >>= 1;
                    if (y == 0) break;
                    x *= x;
                }
            }
            else if (y < 0) {
                y = -y;
                while (true) {
                    if ((y & 1) != 0) ret /= x;
                    y >>= 1;
                    if (y == 0) break;
                    x *= x;
                }
            }
            return ret;
        }

        public static decimal Sqrt(decimal x) {
            var p = (decimal)Math.Sqrt((double)x);
            for (int i = 0; i < 100; i++) {
                decimal tmp = (p * p + x) / (2 * p);
                if (tmp == p) break;
                p = tmp;
            }
            return p;
        }

        public static decimal Exp(decimal x) {
            var s = Math.Round(x);
            Assert.ArgInRange(nameof(Exp), int.MinValue <= s && s <= int.MaxValue);
            var t = x - s;
            var p = 1m;
            var oldExpT = 1m;
            for (int i = 1; i < 100; i++) {
                p *= t / i;
                var newExpT = oldExpT + p;
                if (newExpT == oldExpT) break;
                oldExpT = newExpT;
            }
            return PowN(DecMath.E, (int)s) * oldExpT;
        }

        public static decimal Log(decimal x) {
            var flog2x = Math.Floor(Log2(x, highAccuracy: true));
            if (flog2x < int.MinValue || int.MaxValue < flog2x) throw new OverflowException();
            var s = PowN(2, (int)flog2x);
            var t = x / s;
            var a = (t - 1m) / (t + 1m);
            var aa = a * a;
            var dend = a;
            var dsor = 1;
            var logT = 0m;
            for (int i = 1; i < 100; i++) {
                var tmp = logT + dend / dsor;
                if (tmp == logT) break;
                logT = tmp;
                dend *= aa;
                dsor += 2;
            }
            return flog2x * DecMath.LogE2 + logT * 2;
        }

        public static decimal Log10(decimal a) {
            Assert.ArgInRange(nameof(Log10), a > 0);
            decimal ret = 0;
            while (a >= 10) {
                a /= 10;
                ret++;
            }
            while (a < 1) {
                a *= 10;
                ret--;
            }
            return ret + (decimal)Math.Log10((double)a);
        }

        public static decimal Log2(decimal a, bool highAccuracy) {
            if (highAccuracy) {
                return (decimal)(decimal)QuadMath.Log2((quad)(decimal)a);
            }
            else {
                return (decimal)(Math.Log((double)a) / Math.Log(2));
            }
        }

        /// <summary>
        /// a が 0 の場合は 0 を返し、それ以外の場合は floor(log10(abs(a))) を返す。
        /// </summary>
        public static int FLog10Abs(decimal a) {
            if (a == 0) {
                return 0;
            }
            else {
                return (int)Math.Floor(Log10(Math.Abs(a)));
            }
        }

        /// <summary>
        /// 10のe乗を返す。
        /// 数値を文字列に変換する際に桁合わせのために使用する。
        /// Math.Pow() をそのまま使用すると double の仮数部のビット数が足りずに誤差が出るため、10進10桁ずつに分割して計算する。
        /// </summary>
        public static decimal Pow10(decimal e) {
            int stride = 10;
            decimal coeff = (decimal)Math.Pow(10, stride);
            decimal pow = 1m;
            if (e >= 0) {
                while (e >= stride) {
                    pow *= coeff;
                    e -= stride;
                }
                pow *= (decimal)Math.Pow(10, (double)e);
            }
            else {
                while (e <= -stride) {
                    pow /= coeff;
                    e += stride;
                }
                pow /= (decimal)Math.Pow(10, -(double)e);
            }
            return pow;
        }

        /// <summary>配列の全要素の最大公約数</summary>
        public static decimal Gcd(decimal[] x) {
            Assert.AssertArgNonEmpty(nameof(Gcd), x);
            return gcdRecursive(x, 0, x.Length - 1);
        }
        private static decimal gcdRecursive(decimal[] x, int il, int ir) {
            if (il == ir) {
                return x[il];
            }
            else {
                int im = il + (ir - il) / 2;
                return Gcd(gcdRecursive(x, il, im), gcdRecursive(x, im + 1, ir));
            }
        }

        /// <summary>a と b の最小公倍数。a * b が非常に大きくなる場合、この関数は例外を投げる。</summary>
        public static decimal Lcm(decimal a, decimal b) {
            return a * b / Gcd(a, b);
        }

        /// <summary>配列の全要素の最小公倍数</summary>
        public static decimal Lcm(decimal[] x) {
            Assert.AssertArgNonEmpty(nameof(Lcm), x);
            return lcmRecursive(x, 0, x.Length - 1);
        }
        private static decimal lcmRecursive(decimal[] x, int il, int ir) {
            if (il == ir) {
                return x[il];
            }
            else {
                int im = il + (ir - il) / 2;
                return Lcm(lcmRecursive(x, il, im), lcmRecursive(x, im + 1, ir));
            }
        }

        // 三角関数
        // 暫定的に System.Math の関数を使用する
        public static decimal Sin(decimal a) => (decimal)Math.Sin((double)a);
        public static decimal Cos(decimal a) => (decimal)Math.Cos((double)a);
        public static decimal Tan(decimal a) => (decimal)Math.Tan((double)a);
        public static decimal Asin(decimal a) => (decimal)Math.Asin((double)a);
        public static decimal Acos(decimal a) => (decimal)Math.Acos((double)a);
        public static decimal Atan(decimal a) => (decimal)Math.Atan((double)a);
        public static decimal Atan2(decimal a, decimal b) => (decimal)Math.Atan2((double)a, (double)b);
        public static decimal Sinh(decimal a) => (decimal)Math.Sinh((double)a);
        public static decimal Cosh(decimal a) => (decimal)Math.Cosh((double)a);
        public static decimal Tanh(decimal a) => (decimal)Math.Tanh((double)a);

        public static int CDiv(int a, int b) {
            Assert.ArgInRange(nameof(CDiv), nameof(a), a < 0);
            Assert.ArgInRange(nameof(CDiv), nameof(b), b < 1);
            return (a + b - 1) / b;
        }

        // 最大最小
        public static decimal Clip(decimal min, decimal max, decimal x) {
            Assert.ArgInRange(nameof(Clip), min <= max, "min > max");
            return x < min ? min : (x > max ? max : x);
        }
        public static int Clip(int min, int max, int x) {
            Assert.ArgInRange(nameof(Clip), min <= max, "min > max");
            return x < min ? min : (x > max ? max : x);
        }

        private const long IsPrimeArgMax = 1L << 52;

        /// <summary>素数判定</summary>
        public static bool IsPrime(decimal a) {
            Assert.ArgInRange(nameof(IsPrime), 0m <= a && a <= IsPrimeArgMax);
            return IsPrime((long)a);
        }

        /// <summary>素数判定</summary>
        public static bool IsPrime(long a) {
            if (a < 2) return false;
            if (a == 2) return true;
            if (a % 2 == 0) return false;
            Assert.ArgInRange(nameof(IsPrime), a <= IsPrimeArgMax);

            long n = (long)Math.Sqrt(a);
            for (long i = 3; i <= n; i += 2) {
                if (a % i == 0) return false;
            }
            return true;
        }

        /// <summary>n番目の素数</summary>
        public static decimal Prime(int n) {
            Assert.ArgInRange(nameof(Prime), 0 <= n && n <= 100000);
            int a = 2;
            for (int i = 0; i < n; i++) {
                a++;
                while (!IsPrime(a)) a++;
            }
            return a;
        }

        /// <summary>素因数分解</summary>
        public static decimal[] PrimeFactors(decimal realN) {
            var res = new List<decimal>();
            Assert.ArgIsInteger(nameof(PrimeFactors), realN);
            Assert.ArgInRange(nameof(PrimeFactors), 1m <= realN && realN <= 1e14m);
            long n = (long)realN;
            for (long a = 2; a * a <= n; ++a) {
                while (n % a == 0) {
                    res.Add(a);
                    n /= a;
                }
            }
            if (n != 1) res.Add(n);
            return res.ToArray();
        }

        /// <summary>等差数列の生成</summary>
        public static decimal[] Range(decimal start, decimal stop, decimal step, bool inclusive = false) {
            if (step == 0) step = start < stop ? 1 : -1;
            int n = (int)Math.Ceiling((stop - start) / step);
            Assert.ArgInRange(nameof(Range), n >= 0, "Range broken");
            if (inclusive && start + n * step == stop) n++;
            var array = new decimal[n];
            for (int i = 0; i < n; i++) {
                array[i] = start + step * i;
            }
            return array;
        }

        public static long ToLong(decimal val) {
            val = Math.Round(val);
            if (val < long.MinValue && long.MaxValue < val) throw new OverflowException("Out of range of int64.");
            return (long)val;
        }
        public static int ToInt(decimal val) {
            val = Math.Round(val);
            if (val < int.MinValue && int.MaxValue < val) throw new OverflowException("Out of range of int32.");
            return (int)val;
        }
        public static char ToChar(decimal val) {
            val = Math.Round(val);
            if (val < char.MinValue && char.MaxValue < val) throw new OverflowException("Out of range of char.");
            return (char)val;
        }
        public static byte ToByte(decimal val) {
            val = Math.Round(val);
            if (val < byte.MinValue && byte.MaxValue < val) throw new OverflowException("Out of range of byte.");
            return (byte)val;
        }
    }
}
