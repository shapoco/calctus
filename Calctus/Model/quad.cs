using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model {
    struct quad {
        private const int ExpBias = 0x3fff;
        public static readonly quad PositiveZero = new quad(false, 0, ufixed113.Zero);
        public static readonly quad NegativeZero = new quad(true, 0, ufixed113.Zero);

        public readonly bool Neg;
        public readonly ushort Exp;
        public readonly ufixed113 Coe;

        public quad(bool neg, ushort exp, ufixed113 coe) {
            Neg = neg;
            Exp = exp;
            Coe = coe;
        }

        public bool IsZero => (Exp == 0) && (Coe == 0);
        public bool IsNormalized => Exp != 0 && Exp != 0x7fff;

        public quad Truncate() {
            if (Exp == 0) return new quad(Neg, 0, 0);
            int truncBits = ((int)ExpBias - (int)Exp) + ufixed113.NumBits - 1;
            var coe = Coe;
            if (truncBits > 0) coe = Coe.TruncateRight(truncBits);
            return Normalize(Neg, Exp, coe);
        }

        public override bool Equals(object obj) {
            if (obj is quad qObj) return this == qObj;
            return false;
        }

        public override int GetHashCode() {
            if (IsZero) return 0;
            var hash = Coe.GetHashCode() ^ (int)Exp;
            return Neg ? -hash : hash;
        }

        public override string ToString() {
            return (Neg ? "1 " : "0 ") + Convert.ToString(Exp, 16) + " " + Coe.ToString();
        }

        public static quad Normalize(bool neg, int exp, ufixed113 coe) {
            if (exp >= 0x7fff) throw new OverflowException();

            // ゼロ
            if (coe == 0) return new quad(neg, 0, 0);

            // 正規化
            coe = coe.Align(out int shift);
            exp -= shift;
            if (exp <= 0) {
                coe = coe.LogicShiftLeft(-exp);
                exp = 0;
            }

            return new quad(neg, (ushort)exp, coe);
        }

        public static implicit operator quad(long a) => (quad)(decimal)a;

        public static explicit operator quad(decimal d) {
            // 符号
            bool neg = (d < 0);
            d = Math.Abs(d);

            var di = Math.Floor(d);
            var df = d - di;

            // 整数部
            quad qi;
            if (di != 0) {
                ushort ei = ExpBias - 1;
                ufixed113 ci = ufixed113.Zero;
                while (di > 0) {
                    ci = ci.SingleShiftRight((uint)(di % 2));
                    ei++;
                    di = Math.Floor(di / 2);
                }
                qi = new quad(neg, ei, ci);
            }
            else {
                qi = neg ? NegativeZero : PositiveZero;
            }

            // 小数部
            quad qf;
            if (df != 0) {
                ushort ef = ExpBias + ufixed113.NumBits - 1;
                ufixed113 cf = ufixed113.Zero;
                while (cf.Msb == 0) {
                    df *= 2;
                    var tmp = (uint)Math.Floor(df);
                    cf = cf.SingleShiftLeft(tmp);
                    ef--;
                    df -= tmp;
                }
                qf = new quad(neg, ef, cf);
            }
            else {
                qf = neg ? NegativeZero : PositiveZero;
            }

            return qi + qf;
        }

        public static explicit operator ulong(quad a) {
            if (a.IsZero) return 0;
            if (a.Exp == 0) return 0;
            var coe = a.Coe;
            int shifts = ((int)ExpBias - (int)a.Exp) + ufixed113.NumBits - 1;
            if (shifts > 0) {
                coe = coe.LogicShiftRight(shifts);
            }
            return coe.Lower64Bits;
        }

        public static explicit operator decimal(quad q) {
            if (q.IsZero) return 0;
            var sign = q.Neg ? -1m : 1m;
            if (q.Neg) q = -q;

            var qi = q.Truncate();
            var qf = q - qi;

            // 整数部
            decimal di = 0;
            {
                decimal p = 1;
                while (qi > 0) {
                    if ((((ulong)qi) & 1) == 1) {
                        di += p;
                    }
                    qi = (qi / 2).Truncate();
                    p *= 2;
                }
            }

            // 小数部
            decimal df = 0;
            {
                decimal p = 1;
                while (qf != 0) {
                    qf *= 2;
                    p /= 2;
                    if (qf >= 1) {
                        df += p;
                        qf -= 1;
                    }
                }
            }

            return sign * (di + df);
        }

        public static quad operator -(quad a) => new quad(!a.Neg, a.Exp, a.Coe);

        public static quad operator +(quad a, quad b) {
            if (a.IsZero) return b;
            if (b.IsZero) return a;

            var aCoe = a.Coe;
            var bCoe = b.Coe;
            var aExp = Math.Max((ushort)1, a.Exp);
            var bExp = Math.Max((ushort)1, b.Exp);
            var qExp = (int)Math.Max(aExp, bExp);

            // 桁合わせ
            if (aExp > bExp) {
                bCoe = bCoe.LogicShiftRight(aExp - bExp);
            }
            else if (aExp < bExp) {
                aCoe = aCoe.LogicShiftRight(bExp - aExp);
            }

            bool qNeg;
            ufixed113 qCoe;
            if (a.Neg == b.Neg) {
                // 符号が同じ場合: 単純に足す
                qNeg = a.Neg;
                qCoe = aCoe.Add(bCoe, out uint carry);

                // 桁上がり
                if (carry == 1) {
                    qCoe = qCoe.SingleShiftRight(1u);
                    qExp++;
                }
            }
            else {
                // 符号が異なる場合: 大きい方から小さい方を引く
                if (aCoe < bCoe) {
                    qNeg = b.Neg;
                    qCoe = bCoe - aCoe;
                }
                else {
                    qNeg = a.Neg;
                    qCoe = aCoe - bCoe;
                }
            }

            return Normalize(qNeg, qExp, qCoe);
        }

        public static quad operator -(quad a, quad b) => a + (-b);

        public static quad operator ++(quad a) => a + 1;
        public static quad operator --(quad a) => a - 1;

        public static quad operator *(quad a, quad b) {
            var neg = a.Neg ^ b.Neg;
            var exp = (int)a.Exp + (int)b.Exp - (int)ExpBias;
            var coe = a.Coe.Mul(b.Coe, out uint carry);
            if (carry == 1) {
                coe = coe.SingleShiftRight(1);
                exp++;
            }
            return Normalize(neg, exp, coe);
        }

        public static quad operator /(quad a, quad b) {
            var neg = a.Neg ^ b.Neg;
            var exp = (int)a.Exp - (int)b.Exp + (int)ExpBias;
            var coe = a.Coe / b.Coe;
            return Normalize(neg, exp, coe);
        }

        public static bool operator ==(quad a, quad b) {
            if (a.IsZero) return b.IsZero;
            if (b.IsZero) return a.IsZero;
            return (a.Neg == b.Neg) && (a.Exp == b.Exp) && (a.Coe == b.Coe);
        }
        public static bool operator !=(quad a, quad b) => !(a == b);

        public static bool operator >(quad a, quad b) {
            if (a.IsZero && b.IsZero) return false;
            if (a.Neg && !b.Neg) return false;
            if (!a.Neg && b.Neg) return true;
            if (a.Exp > b.Exp) return !a.Neg;
            if (a.Exp < b.Exp) return a.Neg;
            if (a.Coe > b.Coe) return !a.Neg;
            return false;
        }
        public static bool operator <(quad a, quad b) => b > a;
        public static bool operator >=(quad a, quad b) => (a > b) || (a == b);
        public static bool operator <=(quad a, quad b) => (a < b) || (a == b);

        public static void Test() {
            AssertAdd(5, 3, 8);
            AssertAdd(3, 5, 8);
            AssertAdd(3, -5, -2);
            AssertAdd(-3, 5, 2);
            AssertAdd(0.25m, 0.75m, 1);
            AssertAdd(0xffffffff, 1, 0x100000000);
            AssertAdd(0x100000000, 1, 0x100000001);
            AssertAdd(0xffffffff, 0xffffffff, 0x1fffffffe);
            AssertAdd(1, 1.0m / 0x10000000, 1 + 1.0m / 0x10000000);
            
            AssertSub(5, 3, 2);
            AssertSub(3, 5, -2);
            AssertSub(3, -5, 8);
            AssertSub(-3, 5, -8);
            AssertSub(0.25m, 0.75m, -0.5m);
            AssertSub(0xffffffff, 1, 0xfffffffe);
            AssertSub(0x100000000, 1, 0xffffffff);
            AssertSub(0xffffffff, 0xffffffff, 0);
            AssertSub(1, 1.0m / 0x10000000, 1 - 1.0m / 0x10000000);

            AssertMul(5, 3, 15);
            AssertMul(3, 5, 15);
            AssertMul(3, -5, -15);
            AssertMul(-3, 5, -15);
            AssertMul(0.25m, 0.75m, 0.1875m);
            AssertMul(0x10000000, 1.0m / 0x10000000, 1);
            AssertMul(1.75m, 1.75m, 3.0625m);

            AssertTrunc(0, 0);
            AssertTrunc(0.5m, 0);
            AssertTrunc(1m - 1m / 0x10000000, 0);
            AssertTrunc(1m, 1);
            AssertTrunc(1.5m, 1);
            AssertTrunc(2m - 1m / 0x10000000, 1);
            AssertTrunc(2m, 2);
            AssertTrunc(-0.5m, 0);
            AssertTrunc(-1m + 1m / 0x10000000, 0);
            AssertTrunc(-1m, -1);
            AssertTrunc(-1.5m, -1);
            AssertTrunc(-2m + 1m / 0x10000000, -1);
            AssertTrunc(-2m, -2);

            Assert.Equal("[" + nameof(quad) + "] ", (decimal)(quad)12345m, 12345m);
            Assert.Equal("[" + nameof(quad) + "] ", (decimal)(quad)0.03125m, 0.03125m);
            Assert.Equal("[" + nameof(quad) + "] ", (decimal)(quad)12345.03125m, 12345.03125m);
            //Assert.Equal("[" + nameof(quad) + "] ", (decimal)(quad)0.1m, 0.1m);
        }

        public static void AssertAdd(decimal a, decimal b, decimal q) {
            Assert.Equal("[" + nameof(quad) + "] " + a + "+" + b, (quad)a + (quad)b, (quad)q);
        }

        public static void AssertSub(decimal a, decimal b, decimal q) {
            Assert.Equal("[" + nameof(quad) + "] " + a + "-" + b, (quad)a - (quad)b, (quad)q);
        }

        public static void AssertMul(decimal a, decimal b, decimal q) {
            Assert.Equal("[" + nameof(quad) + "] " + a + "*" + b, (quad)a * (quad)b, (quad)q);
        }

        public static void AssertTrunc(decimal a, decimal q) {
            Assert.Equal("[" + nameof(quad) + "] Truncate(" + a + ")", ((quad)a).Truncate(), (quad)q);
        }
    }
}
