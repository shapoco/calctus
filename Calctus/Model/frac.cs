using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model {
    struct frac {
        public readonly decimal Nume;
        public readonly decimal Deno;

        public frac(decimal val) : this(val, 1) { }

        public frac(decimal n, decimal d) {
            var sign = RMath.Sign(n) * RMath.Sign(d);
            n = RMath.Abs(n);
            d = RMath.Abs(d);

            // 整数比を求める
            while (n != RMath.Floor(n)) { n *= 10; d *= 10; }
            while (d != RMath.Floor(d)) { n *= 10; d *= 10; }
            var g = RMath.Gcd(n, d);
            n /= g;
            d /= g;

            Nume = sign * n;
            Deno = d;
        }

        public override bool Equals(object obj) {
            if (obj is real || obj is frac) {
                // 通分して比較
                frac objFrac;
                if (obj is real objReal) {
                    objFrac = (frac)objReal;
                }
                else {
                    objFrac = (frac)obj;
                }
                if (RMath.Reduce(this, objFrac, out decimal an, out decimal bn, out _)) {
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

        public override string ToString() => Nume + ":" + Deno;

        public static explicit operator double(frac val) => (double)((decimal)val.Nume / (decimal)val.Deno);
        public static explicit operator decimal(frac val) => (decimal)val.Nume / (decimal)val.Deno;
        public static explicit operator long(frac val) => (long)((decimal)val.Nume / (decimal)val.Deno);
        public static explicit operator int(frac val) => (int)((decimal)val.Nume / (decimal)val.Deno);

        public static implicit operator frac(decimal val) => new frac(val);
        public static explicit operator frac(double val) => new frac((decimal)val);
        public static implicit operator frac(long val) => new frac(val);
        public static implicit operator frac(int val) => new frac(val);
        public static explicit operator frac(real val) => new frac((decimal)val);

        public static frac operator -(frac val) => new frac(-val.Nume, val.Deno);
        public static frac operator +(frac a, frac b) {
            RMath.Reduce(a, b, out decimal an, out decimal bn, out decimal deno);
            return new frac(an + bn, deno);
        }
        public static frac operator -(frac a, frac b) {
            RMath.Reduce(a, b, out decimal an, out decimal bn, out decimal deno);
            return new frac(an - bn, deno);
        }
        public static frac operator *(frac a, frac b) {
            var g0 = RMath.Gcd(a.Nume, b.Deno);
            var g1 = RMath.Gcd(b.Nume, a.Deno);
            return new frac((a.Nume / g0) * (b.Nume / g1), (a.Deno / g1) * (b.Deno / g0));
        }
        public static frac operator /(frac a, frac b) {
            var gn = RMath.Gcd(a.Nume, b.Nume);
            var gd = RMath.Gcd(b.Deno, a.Deno);
            return new frac((a.Nume / gn) * (b.Deno / gd), (a.Deno / gd) * (b.Nume / gn));
        }

        public static bool operator ==(frac a, frac b) => a.Equals(b);
        public static bool operator !=(frac a, frac b) => !a.Equals(b);
        public static bool operator <(frac a, frac b) => (a - b).Nume < 0;
        public static bool operator >(frac a, frac b) => (a - b).Nume > 0;
        public static bool operator <=(frac a, frac b) => (a - b).Nume <= 0;
        public static bool operator >=(frac a, frac b) => (a - b).Nume >= 0;

    }
}
