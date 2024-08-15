using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Maths {
    struct rational : IScalar<rational> {
        public const decimal FindFracMaxDeno = 1000000000000m;

        /// <summary>分子</summary>
        public readonly decimal Nume;

        /// <summary>分母</summary>
        public readonly decimal Deno;

        public bool IsNegative => (Nume < 0) ^ (Deno < 0);
        public bool IsInteger => Deno == 1;

        public static rational FromDecimal(decimal val, out bool degraded) {
            FindFrac(val, out decimal nume, out decimal deno, out degraded);
            return new rational(nume, deno);
        }

        public static rational FromDecimal(decimal nume, decimal deno, out bool degraded) {
            if (nume.IsInteger() && deno.IsInteger()) {
                var sign = Math.Sign(nume) * Math.Sign(deno);
                nume = Math.Abs(nume);
                deno = Math.Abs(deno);

                // 約分
                var gcd = MathEx.Gcd(nume, deno);
                nume /= gcd;
                deno /= gcd;

                nume *= sign;
                degraded = false;
            }
            else {
                FindFrac(nume / deno, out nume, out deno, out degraded);
            }
            return new rational(nume, deno);
        }

        private rational(decimal nume, decimal deno) {
            this.Nume = nume;
            this.Deno = deno;
        }

        public override bool Equals(object other) {
            if (other is decimal otherDec) return CompareTo(otherDec) == 0;
            if (other is rational otherFrac) return CompareTo(otherFrac) == 0;
            return false;
        }

        public decimal ToDecimal(CastOptions opt, out bool degraded) {
            var logOut = MathEx.Log(Math.Abs(Nume)) - MathEx.Log(Math.Abs(Deno));
            var logMax = MathEx.Log(Math.Abs(decimal.MaxValue));
            if (logOut >= logMax) {
                if (!opt.HasFlag(CastOptions.AllowOverflow)) {
                    throw Log.Here().E(new OverflowException());
                }
                else {
                    degraded = true;
                    return IsNegative ? decimal.MinValue : decimal.MaxValue;
                }
            }
            var ret = Nume / Deno;
            degraded = ret * Deno != Nume; // todo rational.ToDecimal() degrade の判定これでいいか？
            return ret;
        }

        public rational ToFrac(CastOptions opt, out bool degraded) { degraded = false; return this; }

        // todo 実装 rational.ToApFixed()
        public apfixed ToApFixed(CastOptions opt, out bool degraded) {
            throw new NotImplementedException();
        }

        public int CompareTo(rational other) => Math.Sign((this - other).Nume);

        // todo 正確な比較 rational.CompareTo(decimal)
        public int CompareTo(decimal other) {
            return ((decimal)this).CompareTo(other);
        }

        // todo 実装 rational.CompareTo(apfixed)
        public int CompareTo(apfixed other) {
            throw new NotImplementedException();
        }

        public override int GetHashCode() => Nume.GetHashCode() ^ Deno.GetHashCode();

        public override string ToString() => "(" + Nume + "/" + Deno + ")";

        public static explicit operator double(rational val) => (double)(val.Nume / val.Deno);
        public static explicit operator decimal(rational val) => val.Nume / val.Deno;
        public static explicit operator long(rational val) => (long)(val.Nume / val.Deno);
        public static explicit operator int(rational val) => (int)(val.Nume / val.Deno);

        public static implicit operator rational(decimal val) => FromDecimal(val, out _);
        public static explicit operator rational(double val) => FromDecimal((decimal)val, out _);
        public static implicit operator rational(long val) => FromDecimal(val, out _);
        public static implicit operator rational(int val) => FromDecimal(val, out _);

        public static rational operator -(rational val) => new rational(-val.Nume, val.Deno);
        public static rational operator +(rational a, rational b) {
            // todo Frac.+ 通分に失敗した場合の処理
            Reduce(a, b, out decimal an, out decimal bn, out decimal deno);
            return new rational(an + bn, deno);
        }
        public static rational operator -(rational a, rational b) {
            // todo Frac.- 通分に失敗した場合の処理
            Reduce(a, b, out decimal an, out decimal bn, out decimal deno);
            return new rational(an - bn, deno);
        }
        public static rational operator *(rational a, rational b) {
            var g0 = MathEx.Gcd(a.Nume, b.Deno);
            var g1 = MathEx.Gcd(b.Nume, a.Deno);
            return new rational((a.Nume / g0) * (b.Nume / g1), (a.Deno / g1) * (b.Deno / g0));
        }
        public static rational operator /(rational a, rational b) {
            var gn = MathEx.Gcd(a.Nume, b.Nume);
            var gd = MathEx.Gcd(b.Deno, a.Deno);
            return new rational((a.Nume / gn) * (b.Deno / gd), (a.Deno / gd) * (b.Nume / gn));
        }

        public static bool operator ==(rational a, rational b) => a.Equals(b);
        public static bool operator !=(rational a, rational b) => !a.Equals(b);
        public static bool operator <(rational a, rational b) => a.CompareTo(b) < 0;
        public static bool operator >(rational a, rational b) => a.CompareTo(b) > 0;
        public static bool operator <=(rational a, rational b) => a.CompareTo(b) <= 0;
        public static bool operator >=(rational a, rational b) => a.CompareTo(b) >= 0;

        /// <summary>通分</summary>
        public static void Reduce(rational a, rational b, out decimal aNume, out decimal bNume, out decimal deno) {
            var d = MathEx.Gcd(a.Deno, b.Deno);
            deno = a.Deno * b.Deno / d;
            aNume = a.Nume * deno / a.Deno;
            bNume = b.Nume * deno / b.Deno;
        }

        /// <summary>通分</summary>
        public static bool TryReduce(rational a, rational b, out decimal aNume, out decimal bNume, out decimal deno) {
            try {
                Reduce(a, b, out aNume, out bNume, out deno);
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
        public static rational FindFrac(decimal x, out bool degraded, 
                decimal maxNume = FindFracMaxDeno, decimal maxDeno = FindFracMaxDeno) {
            FindFrac(x, out decimal nume, out decimal deno, out degraded, maxNume, maxDeno);
            return new rational(nume, deno);
        }

        /// <summary>
        /// 分母・分子が max以下の分数で x に最も近いものを返す
        /// </summary>
        public static void FindFrac(decimal x, out decimal nume, out decimal deno, out bool degraded,
                decimal maxNume = FindFracMaxDeno, decimal maxDeno = FindFracMaxDeno) {
            if (maxNume < 1 || 1e12m < maxNume || !maxNume.IsInteger()) throw Log.Here().ArgException(nameof(maxNume));
            if (maxDeno < 1 || 1e12m < maxDeno || !maxNume.IsInteger()) throw Log.Here().ArgException(nameof(maxDeno));

            if (x.IsInteger()) {
                nume = x;
                deno = 1;
                degraded = false;
                return;
            }

            int sign = Math.Sign(x);
            x = Math.Abs(x);

            var xis = new List<decimal>();

            // 連分数展開
            nume = 1;
            deno = 1;
            degraded = true;
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

                var frac = x - xi;
                if (Math.Abs(frac) < 1e-20m) {
                    degraded = (frac == 0);
                    break;
                }

                if (Math.Abs(nume / deno - x) < 1e-20m) break;
                
                x = 1m / frac;
            }

            nume *= sign;
        }
    }
}
