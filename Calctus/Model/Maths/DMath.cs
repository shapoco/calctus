using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Shapoco.Calctus.Model.Maths.Types;

namespace Shapoco.Calctus.Model.Maths {
    static class DMath {
        public static readonly Regex Pattern = new Regex(@"^(?<frac>-?([1-9][0-9]*(_[0-9]+)*|0)*(\.[0-9]+(_[0-9]+)*)?)(?<exppart>(?<echar>e|E)(?<exp>[+\-]?[0-9]+(_[0-9]+)*))?$");
        public static bool IsInteger(this decimal x) => x == Math.Floor(x);

        public const decimal PI = 3.1415926535897932384626433833m;
        public const decimal E = 2.7182818284590452353602874714m;
        public const decimal LogE2 = 0.6931471805599453094172321215m;
        public const decimal LogE10 = 2.3025850929940456840179914547m;

        /// <summary>a と b の最大公約数</summary>
        public static decimal Gcd(decimal a, decimal b) {
            if (!a.IsInteger()) throw new ArgumentException("Arguments must be integers");
            if (!b.IsInteger()) throw new ArgumentException("Arguments must be integers");
            a = Math.Abs(a);
            b = Math.Abs(b);
            while (b != 0) {
                var tmp = a;
                a = b;
                b = tmp % b;
            }
            return a;
        }

        public static bool TryParse(string str, out decimal frac, out char eChar, out int exp) {
            frac = 0;
            eChar = '\0';
            exp = 0;

            var m = Pattern.Match(str);
            if (!m.Success) {
                return false;
            }
            frac = decimal.Parse(m.Groups["frac"].Value.Replace("_", ""), CultureInfo.InvariantCulture);
            if (m.Groups["exppart"].Success) {
                eChar = m.Groups["echar"].Value[0];
                exp = int.Parse(m.Groups["exp"].Value.Replace("_", ""), CultureInfo.InvariantCulture);
            }
            return true;
        }

        public static void Parse(string str, out decimal frac, out char eChar, out int exp) {
            if (!TryParse(str, out frac, out eChar, out exp)) {
                throw new CalctusError("Invalid number format.");
            }
        }

        public static decimal Parse(string str) {
            Parse(str, out decimal frac, out _, out int exp);
            if (exp >= 0) {
                return frac * Math.Round((decimal)Math.Pow(10, exp));
            }
            else {
                return frac / Math.Round((decimal)Math.Pow(10, -exp));
            }
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
            if (s < int.MinValue || int.MaxValue < s) throw new OverflowException();
            var t = x - s;
            var p = 1m;
            var oldExpT = 1m;
            for (int i = 1; i < 100; i++) {
                p *= t / i;
                var newExpT = oldExpT + p;
                if (newExpT == oldExpT) break;
                oldExpT = newExpT;
            }
            return PowN(E, (int)s) * oldExpT;
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
            return flog2x * LogE2 + logT * 2;
        }

        public static decimal Log10(decimal a) {
            if (a <= 0) throw new ArgumentOutOfRangeException();
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
                return (decimal)(decimal)QMath.Log2((quad)(decimal)a);
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
            if (x.Length == 0) throw new ArgumentException("Empty array");
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
            if (x.Length == 0) throw new ArgumentException("Empty array");
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

        // 最大最小
        public static decimal Clip(decimal min, decimal max, decimal x) => x < min ? min : (x > max ? max : x);
        public static int Clip(int min, int max, int x) => x < min ? min : (x > max ? max : x);

        /// <summary>素数判定</summary>
        public static bool IsPrime(decimal a) {
            if (a >= (1L << 52)) throw new ArgumentOutOfRangeException();
            return IsPrime((long)a);
        }

        /// <summary>素数判定</summary>
        public static bool IsPrime(long a) {
            if (a < 2) return false;
            if (a == 2) return true;
            if (a % 2 == 0) return false;
            if (a >= (1L << 52)) throw new ArgumentOutOfRangeException();

            long n = (long)Math.Sqrt(a);
            for (long i = 3; i <= n; i += 2) {
                if (a % i == 0) return false;
            }
            return true;
        }

        /// <summary>n番目の素数</summary>
        public static decimal Prime(int n) {
            if (n < 0 || n > 100000) throw new ArgumentOutOfRangeException();
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
            if (realN.IsInteger()) throw new ArgumentException();
            if (realN < 1 || 100000000000000 < realN) throw new ArgumentOutOfRangeException();
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
            if (n < 0) throw new ArgumentOutOfRangeException();
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
