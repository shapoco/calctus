using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model {
    /// <summary>real に対する数学関数の定義</summary>
    static class RMath {
        // 指数関数
        // 暫定的に System.Math の関数を使用する
        public static real Pow(real a, real b) => (real)Math.Pow((double)a.Raw, (double)b.Raw);
        public static real Sqrt(real a) => (real)Math.Sqrt((double)a.Raw);
        public static real Log(real a) => (real)Math.Log((double)a.Raw);
        public static real Log10(real a) => (real)Math.Log10((double)a.Raw);
        
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
        public static frac FindFrac(decimal x, decimal maxNume, decimal maxDeno) {
            int sign = Sign(x);
            x = Abs(x);

            if (maxNume < 1) throw new ArgumentOutOfRangeException();
            if (maxDeno < 1) throw new ArgumentOutOfRangeException();

            // Stern–Brocot tree
            decimal a = 0, b = 1;
            decimal c = 1, d = 0;
            decimal bestDiff = decimal.MaxValue;
            decimal bestNume = 1, bestDeno = 1;
            while (true) {
                var nume = a + c;
                var deno = b + d;
                if (nume > maxNume || deno > maxDeno) break;
                var q = nume / deno;
                var diff = Abs(x - q);
                if (diff < bestDiff) {
                    bestDiff = diff;
                    bestNume = nume;
                    bestDeno = deno;
                }
                if (diff == 0) break;
                if (x < q) {
                    c = nume;
                    d = deno;
                }
                else {
                    a = nume;
                    b = deno;
                }
            }

            return new frac(sign * bestNume, bestDeno);
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
    }
}
