using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;

namespace Shapoco.Calctus.Model.Mathematics {
    /// <summary>real に対する数学関数の定義</summary>
    static class RMath {
        public const decimal FindFracMaxDeno = 1000000000000m;

        // 指数関数
        // 暫定的に System.Math の関数を使用する
        public static real Pow(real a, real b) => (real)Math.Pow((double)a.Raw, (double)b.Raw);
        public static real Sqrt(real a) => (real)Math.Sqrt((double)a.Raw);
        public static real Log(real a) => (real)Math.Log((double)a.Raw);

        public static real Log10(real a) {
            if (a <= 0) throw new ArgumentOutOfRangeException();
            real ret = 0;
            while (a >= 10) {
                a /= 10;
                ret++;
            }
            while (a < 1) {
                a *= 10;
                ret--;
            }
            return ret + (real)Math.Log10((double)a);
        }

        public static real Log2(real a) => (decimal)QMath.Log2((quad)(decimal)a);

        /// <summary>
        /// a が 0 の場合は 0 を返し、それ以外の場合は floor(log10(abs(a))) を返す。
        /// </summary>
        public static int FLog10(real a) {
            if (a == 0) {
                return 0;
            }
            else {
                return (int)Floor(Log10(Abs(a)));
            }
        }

        /// <summary>
        /// 10のe乗を返す。
        /// 数値を文字列に変換する際に桁合わせのために使用する。
        /// Math.Pow() をそのまま使用すると double の仮数部のビット数が足りずに誤差が出るため、10進10桁ずつに分割して計算する。
        /// </summary>
        public static real Pow10(real e) {
            int stride = 10;
            real coeff = (real)Math.Pow(10, stride);
            real pow = 1m;
            if (e >= 0) {
                while (e >= stride) {
                    pow *= coeff;
                    e -= stride;
                }
                pow *= (real)Math.Pow(10, (double)e);
            }
            else {
                while (e <= -stride) {
                    pow /= coeff;
                    e += stride;
                }
                pow /= (real)Math.Pow(10, -(double)e);
            }
            return pow;
        }

        /// <summary>a と b の最大公約数</summary>
        public static decimal Gcd(decimal a, decimal b) {
            if (a != Floor(a)) throw new ArgumentException("Arguments must be integers");
            if (b != Floor(b)) throw new ArgumentException("Arguments must be integers");
            a = Abs(a);
            b = Abs(b);
            while (b != 0) {
                var tmp = a;
                a = b;
                b = tmp % b;
            }
            return a;
        }

        /// <summary>a と b の最小公倍数。a * b が非常に大きくなる場合、この関数は例外を投げる。</summary>
        public static decimal Lcm(decimal a, decimal b) {
            return a * b / Gcd(a, b);
        }

        /// <summary>通分</summary>
        public static bool Reduce(frac a, frac b, out decimal aNume, out decimal bNume, out decimal deno) {
            try {
                var d = RMath.Gcd(a.Deno, b.Deno);
                deno = a.Deno * b.Deno / d;
                aNume = a.Nume * deno / a.Deno;
                bNume = b.Nume * deno / b.Deno;
                return true;
            }
            catch {
                aNume = 0;
                bNume = 0;
                deno = 1;
                return false;
            }
        }

        /// <summary>
        /// 分母・分子が max以下の分数で x に最も近いものを返す
        /// </summary>
        public static frac FindFrac(decimal x, decimal maxNume = FindFracMaxDeno, decimal maxDeno = FindFracMaxDeno) {
            FindFrac(x, out decimal nume, out decimal deno, maxNume, maxDeno);
            return new frac(nume, deno);
        }

        /// <summary>
        /// 分母・分子が max以下の分数で x に最も近いものを返す
        /// </summary>
        public static void FindFrac(decimal x, out decimal nume, out decimal deno, decimal maxNume = FindFracMaxDeno, decimal maxDeno = FindFracMaxDeno) {
            if (maxNume < 1) throw new ArgumentOutOfRangeException();
            if (maxDeno < 1) throw new ArgumentOutOfRangeException();
            if (maxNume > 1000000000000m) throw new ArgumentOutOfRangeException();
            if (maxDeno > 1000000000000m) throw new ArgumentOutOfRangeException();

            if (x == 0) {
                nume = 0;
                deno = 1;
                return;
            }
            
            if (x == Math.Floor(x)) {
                nume = x;
                deno = 1;
                return;
            }

            int sign = Sign(x);
            x = Abs(x);

            var xis = new List<decimal>();

            // 連分数展開
            nume = 1;
            deno = 1;
            while (true) {
                var xi = Math.Floor(x);
                xis.Add(xi);

                try {
                    var n = xi;
                    var d = 1m;
                    for (int i = xis.Count - 2; i >= 0; i--) {
                        var tmp = n;
                        n = n * xis[i] + d;
                        d = tmp;
                        var gcd = Gcd(d, n);
                        d /= gcd;
                        n /= gcd;
                    }
                    if (n > maxNume || d > maxDeno) break;
                    nume = n;
                    deno = d;
                }
                catch {
                    break;
                }

                if (Math.Abs(nume / deno - x) < 1e-20m) break;
                if (Math.Abs(x - xi) < 1e-20m) break;

                x = 1m / (x - xi);
            }

            nume *= sign;
        }

        // 三角関数
        // 暫定的に System.Math の関数を使用する
        public static real Sin(real a) => (real)Math.Sin((double)a.Raw);
        public static real Cos(real a) => (real)Math.Cos((double)a.Raw);
        public static real Tan(real a) => (real)Math.Tan((double)a.Raw);
        public static real Asin(real a) => (real)Math.Asin((double)a.Raw);
        public static real Acos(real a) => (real)Math.Acos((double)a.Raw);
        public static real Atan(real a) => (real)Math.Atan((double)a.Raw);
        public static real Atan2(real a, real b) => (real)Math.Atan2((double)a.Raw, (double)b.Raw);
        public static real Sinh(real a) => (real)Math.Sinh((double)a.Raw);
        public static real Cosh(real a) => (real)Math.Cosh((double)a.Raw);
        public static real Tanh(real a) => (real)Math.Tanh((double)a.Raw);

        // 丸め関数
        public static real Floor(real val) => (real)Math.Floor(val.Raw);
        public static real Ceiling(real val) => (real)Math.Ceiling(val.Raw);
        public static real Truncate(real val) => (real)Math.Truncate(val.Raw);
        public static real Round(real val) => (real)Math.Round(val.Raw);
        
        // 絶対値と符号
        public static real Abs(real val) => val >= 0 ? val : -val;
        public static int Sign(real val) => Math.Sign(val.Raw);

        // 最大最小
        public static real Max(real a, real b) => a > b ? a : b;
        public static real Min(real a, real b) => a < b ? a : b;

        /// <summary>素数判定</summary>
        public static bool IsPrime(real a) {
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
        public static real Prime(int n) {
            if (n < 0 || n > 100000) throw new ArgumentOutOfRangeException();
            int a = 2;
            for (int i = 0; i < n; i++) {
                a++;
                while (!IsPrime(a)) a++;
            }
            return a;
        }

        /// <summary>素因数分解</summary>
        public static real[] PrimeFactors(real realN) {
            var res = new List<real>();
            if (Floor(realN) != realN) throw new ArgumentException();
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
    }
}
