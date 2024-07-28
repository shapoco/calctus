using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Maths;

namespace Shapoco.Calctus.Model.Maths.Types {
    struct frac {
        public readonly decimal Nume;
        public readonly decimal Deno;

        public frac(decimal val) {
            FracMath.FindFrac(val, out Nume, out Deno);
        }

        public frac(decimal n, decimal d) {
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
                FracMath.FindFrac(n / d, out Nume, out Deno);
            }
        }

        public override bool Equals(object obj) {
            if (obj is decimal || obj is frac) {
                // 通分して比較
                frac objFrac;
                if (obj is decimal objDecimal) {
                    objFrac = (frac)objDecimal;
                }
                else {
                    objFrac = (frac)obj;
                }
                if (FracMath.Reduce(this, objFrac, out decimal an, out decimal bn, out _)) {
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

        public override string ToString() => Nume + "$" + Deno;

        public static explicit operator double(frac val) => (double)(val.Nume / val.Deno);
        public static explicit operator decimal(frac val) => val.Nume / val.Deno;
        public static explicit operator long(frac val) => (long)(val.Nume / val.Deno);
        public static explicit operator int(frac val) => (int)(val.Nume / val.Deno);

        public static implicit operator frac(decimal val) => new frac(val);
        public static explicit operator frac(double val) => new frac((decimal)val);
        public static implicit operator frac(long val) => new frac(val);
        public static implicit operator frac(int val) => new frac(val);

        public static frac operator -(frac val) => new frac(-val.Nume, val.Deno);
        public static frac operator +(frac a, frac b) {
            FracMath.Reduce(a, b, out decimal an, out decimal bn, out decimal deno);
            return new frac(an + bn, deno);
        }
        public static frac operator -(frac a, frac b) {
            FracMath.Reduce(a, b, out decimal an, out decimal bn, out decimal deno);
            return new frac(an - bn, deno);
        }
        public static frac operator *(frac a, frac b) {
            var g0 = MathEx.Gcd(a.Nume, b.Deno);
            var g1 = MathEx.Gcd(b.Nume, a.Deno);
            return new frac((a.Nume / g0) * (b.Nume / g1), (a.Deno / g1) * (b.Deno / g0));
        }
        public static frac operator /(frac a, frac b) {
            var gn = MathEx.Gcd(a.Nume, b.Nume);
            var gd = MathEx.Gcd(b.Deno, a.Deno);
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
