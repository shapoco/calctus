using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Shapoco.Calctus.Model.Maths {
    static class FracMath {
        public const decimal FindFracMaxDeno = 1000000000000m;

        /// <summary>通分</summary>
        public static bool Reduce(frac a, frac b, out decimal aNume, out decimal bNume, out decimal deno) {
            try {
                var d = MathEx.Gcd(a.Deno, b.Deno);
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

            int sign = Math.Sign(x);
            x = Math.Abs(x);

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
                        var gcd = MathEx.Gcd(d, n);
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

    }
}
