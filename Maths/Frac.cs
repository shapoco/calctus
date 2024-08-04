using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Maths {
    struct Frac {
        public const decimal FindFracMaxDeno = 1000000000000m;

        public readonly decimal Nume;
        public readonly decimal Deno;

        public Frac(decimal val) {
            Frac.FindFrac(val, out Nume, out Deno);
        }

        public Frac(decimal n, decimal d) {
            if (n.IsInteger() && d.IsInteger()) {
                var sign = Math.Sign(n) * Math.Sign(d);
                n = Math.Abs(n);
                d = Math.Abs(d);

                // 約分
                var gcd = MathEx.Gcd(n, d);
                n /= gcd;
                d /= gcd;

                Nume = sign * n;
                Deno = d;
            }
            else {
                Frac.FindFrac(n / d, out Nume, out Deno);
            }
        }

        public override bool Equals(object obj) {
            if (obj is decimal || obj is Frac) {
                // 通分して比較
                Frac objFrac;
                if (obj is decimal objDecimal) {
                    objFrac = (Frac)objDecimal;
                }
                else {
                    objFrac = (Frac)obj;
                }
                if (Frac.Reduce(this, objFrac, out decimal an, out decimal bn, out _)) {
                    return an == bn;
                }
                else {
                    return false;
                }
            }
            else {
                return false;
            }
        }

        public override int GetHashCode() => Nume.GetHashCode() ^ Deno.GetHashCode();

        public override string ToString() => "(" + Nume + "/" + Deno + ")";

        public static explicit operator double(Frac val) => (double)(val.Nume / val.Deno);
        public static explicit operator decimal(Frac val) => val.Nume / val.Deno;
        public static explicit operator long(Frac val) => (long)(val.Nume / val.Deno);
        public static explicit operator int(Frac val) => (int)(val.Nume / val.Deno);

        public static implicit operator Frac(decimal val) => new Frac(val);
        public static explicit operator Frac(double val) => new Frac((decimal)val);
        public static implicit operator Frac(long val) => new Frac(val);
        public static implicit operator Frac(int val) => new Frac(val);

        public static Frac operator -(Frac val) => new Frac(-val.Nume, val.Deno);
        public static Frac operator +(Frac a, Frac b) {
            Frac.Reduce(a, b, out decimal an, out decimal bn, out decimal deno);
            return new Frac(an + bn, deno);
        }
        public static Frac operator -(Frac a, Frac b) {
            Frac.Reduce(a, b, out decimal an, out decimal bn, out decimal deno);
            return new Frac(an - bn, deno);
        }
        public static Frac operator *(Frac a, Frac b) {
            var g0 = MathEx.Gcd(a.Nume, b.Deno);
            var g1 = MathEx.Gcd(b.Nume, a.Deno);
            return new Frac((a.Nume / g0) * (b.Nume / g1), (a.Deno / g1) * (b.Deno / g0));
        }
        public static Frac operator /(Frac a, Frac b) {
            var gn = MathEx.Gcd(a.Nume, b.Nume);
            var gd = MathEx.Gcd(b.Deno, a.Deno);
            return new Frac((a.Nume / gn) * (b.Deno / gd), (a.Deno / gd) * (b.Nume / gn));
        }

        public static bool operator ==(Frac a, Frac b) => a.Equals(b);
        public static bool operator !=(Frac a, Frac b) => !a.Equals(b);
        public static bool operator <(Frac a, Frac b) => (a - b).Nume < 0;
        public static bool operator >(Frac a, Frac b) => (a - b).Nume > 0;
        public static bool operator <=(Frac a, Frac b) => (a - b).Nume <= 0;
        public static bool operator >=(Frac a, Frac b) => (a - b).Nume >= 0;


        /// <summary>通分</summary>
        public static bool Reduce(Frac a, Frac b, out decimal aNume, out decimal bNume, out decimal deno) {
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
        public static Frac FindFrac(decimal x, decimal maxNume = FindFracMaxDeno, decimal maxDeno = FindFracMaxDeno) {
            FindFrac(x, out decimal nume, out decimal deno, maxNume, maxDeno);
            return new Frac(nume, deno);
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
