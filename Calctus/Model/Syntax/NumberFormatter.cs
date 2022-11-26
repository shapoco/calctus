using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Shapoco.Calctus.Model.UnitSystem;

namespace Shapoco.Calctus.Model.Syntax {
    abstract class NumberFormatter {
        public readonly Regex Pattern;
        public readonly int CaptureGroupIndex;

        public NumberFormatter(Regex ptn, int capIndex) {
            this.Pattern = ptn;
            this.CaptureGroupIndex = capIndex;
        }

        public abstract Val Parse(Match m);

        public virtual string Format(Val val, EvalContext e) => OnFormat(val, e);
        protected virtual string OnFormat(Val val, EvalContext e) => RealToString(val.AsReal, e);

        public static readonly IntFormatter CStyleInt = new IntFormatter(10, "", new Regex(@"([1-9][0-9]*|0)([eE][+-]?[0-9]+)?"), 0 );
        public static readonly RealFormatter CStyleReal = new RealFormatter("", new Regex(@"([1-9][0-9]*|0)+\.[0-9]+([eE][+-]?[0-9]+)?"), 0);
        public static readonly IntFormatter CStyleHex = new IntFormatter(16, "0x", new Regex(@"0[xX]([0-9a-fA-F]+)"), 1);
        public static readonly IntFormatter CStyleOct = new IntFormatter(8, "0", new Regex(@"0([0-7]+)"), 1);
        public static readonly IntFormatter CStyleBin = new IntFormatter(2, "0b", new Regex(@"0[bB]([01]+)"), 1);

        public static NumberFormatter[] NativeFormats => new NumberFormatter[] {
            CStyleInt, 
            CStyleReal, 
            CStyleHex, 
            CStyleOct, 
            CStyleBin,
        };

        public static string RealToString(real val, EvalContext e) {
            if (val == 0.0m) return "0.0";

            int exp = RMath.FLog10(val);
            var s = e.Settings;
            if (s.ENotationEnabled && exp >= s.ENotationExpPositiveMin) {
                if (s.ENotationAlignment) {
                    exp = (int)Math.Floor((double)exp / 3) * 3;
                }
                var frac = val / RMath.Pow10(exp);
                return frac.ToString("0.############################") + "e+" + exp;
            }
            else if (s.ENotationEnabled && exp <= s.ENotationExpNegativeMax) {
                if (s.ENotationAlignment) {
                    exp = (int)Math.Floor((double)exp / 3) * 3;
                }
                var frac = val * RMath.Pow10(-exp);
                return frac.ToString("0.############################") + "e" + exp;
            }
            else {
                return val.ToString("0.############################");
            }
        }

        public static void Test(EvalContext e, Val val, string str) {
            Assert.Equal(nameof(NumberFormatter), val.ToString(e), str);
        }

        public static void Test() {
            var cint = new ValFormatHint(CStyleInt);
            {
                EvalContext e = new EvalContext();
                e.Settings.ENotationEnabled = false;
                e.Settings.ENotationExpPositiveMin = 4;
                e.Settings.ENotationExpNegativeMax = -3;
                e.Settings.ENotationAlignment = false;
                Test(e, new RealVal(0, cint), "0");
                Test(e, new RealVal(1, cint), "1");
                Test(e, new RealVal(12345, cint), "12345");
                Test(e, new RealVal(1234500000000000, cint), "1234500000000000");
                Test(e, new RealVal(-1, cint), "-1");
                Test(e, new RealVal(-10, cint), "-10");
                Test(e, new RealVal(-12345, cint), "-12345");
                Test(e, new RealVal(-1234500000000000, cint), "-1234500000000000");
                Test(e, new RealVal(0.1m, cint), "0.1");
                Test(e, new RealVal(0.01m, cint), "0.01");
                Test(e, new RealVal(0.001m, cint), "0.001");
                Test(e, new RealVal(0.0000000000000012345m, cint), "0.0000000000000012345");
                Test(e, new RealVal(-0.1m, cint), "-0.1");
                Test(e, new RealVal(-0.01m, cint), "-0.01");
                Test(e, new RealVal(-0.001m, cint), "-0.001");
                Test(e, new RealVal(-0.0000000000000012345m, cint), "-0.0000000000000012345");
            }
            {
                EvalContext e = new EvalContext();
                e.Settings.ENotationEnabled = true;
                e.Settings.ENotationExpPositiveMin = 4;
                e.Settings.ENotationExpNegativeMax = -3;
                e.Settings.ENotationAlignment = false;
                Test(e, new RealVal(0, cint), "0");
                Test(e, new RealVal(1, cint), "1");
                Test(e, new RealVal(1000, cint), "1000");
                Test(e, new RealVal(9999, cint), "9999");
                Test(e, new RealVal(10000, cint), "1e+4");
                Test(e, new RealVal(1234500000000000, cint), "1.2345e+15");
                Test(e, new RealVal(-1, cint), "-1");
                Test(e, new RealVal(-10, cint), "-10");
                Test(e, new RealVal(-1000, cint), "-1000");
                Test(e, new RealVal(-9999, cint), "-9999");
                Test(e, new RealVal(-10000, cint), "-1e+4");
                Test(e, new RealVal(-1234500000000000, cint), "-1.2345e+15");
                Test(e, new RealVal(0.1m, cint), "0.1");
                Test(e, new RealVal(0.01m, cint), "0.01");
                Test(e, new RealVal(0.00999m, cint), "9.99e-3");
                Test(e, new RealVal(0.001m, cint), "1e-3");
                Test(e, new RealVal(0.0000000000000012345m, cint), "1.2345e-15");
                Test(e, new RealVal(-0.1m, cint), "-0.1");
                Test(e, new RealVal(-0.01m, cint), "-0.01");
                Test(e, new RealVal(-0.00999m, cint), "-9.99e-3");
                Test(e, new RealVal(-0.001m, cint), "-1e-3");
                Test(e, new RealVal(-0.0000000000000012345m, cint), "-1.2345e-15");
            }
            {
                EvalContext e = new EvalContext();
                e.Settings.ENotationEnabled = true;
                e.Settings.ENotationExpPositiveMin = 4;
                e.Settings.ENotationExpNegativeMax = -3;
                e.Settings.ENotationAlignment = true;
                Test(e, new RealVal(0, cint), "0");
                Test(e, new RealVal(1, cint), "1");
                Test(e, new RealVal(1000, cint), "1000");
                Test(e, new RealVal(9999, cint), "9999");
                Test(e, new RealVal(10000, cint), "10e+3");
                Test(e, new RealVal(12345, cint), "12.345e+3");
                Test(e, new RealVal(123456, cint), "123.456e+3");
                Test(e, new RealVal(1234567, cint), "1.234567e+6");
                Test(e, new RealVal(1234500000000000, cint), "1.2345e+15");
                Test(e, new RealVal(-1, cint), "-1");
                Test(e, new RealVal(-1000, cint), "-1000");
                Test(e, new RealVal(-9999, cint), "-9999");
                Test(e, new RealVal(-10000, cint), "-10e+3");
                Test(e, new RealVal(-12345, cint), "-12.345e+3");
                Test(e, new RealVal(-123456, cint), "-123.456e+3");
                Test(e, new RealVal(-1234567, cint), "-1.234567e+6");
                Test(e, new RealVal(-1234500000000000, cint), "-1.2345e+15");
                Test(e, new RealVal(0.1m, cint), "0.1");
                Test(e, new RealVal(0.01m, cint), "0.01");
                Test(e, new RealVal(0.00999m, cint), "9.99e-3");
                Test(e, new RealVal(0.001m, cint), "1e-3");
                Test(e, new RealVal(0.0012345m, cint), "1.2345e-3");
                Test(e, new RealVal(0.00012345m, cint), "123.45e-6");
                Test(e, new RealVal(0.000012345m, cint), "12.345e-6");
                Test(e, new RealVal(0.0000000000000012345m, cint), "1.2345e-15");
                Test(e, new RealVal(-0.1m, cint), "-0.1");
                Test(e, new RealVal(-0.01m, cint), "-0.01");
                Test(e, new RealVal(-0.00999m, cint), "-9.99e-3");
                Test(e, new RealVal(-0.001m, cint), "-1e-3");
                Test(e, new RealVal(-0.0012345m, cint), "-1.2345e-3");
                Test(e, new RealVal(-0.00012345m, cint), "-123.45e-6");
                Test(e, new RealVal(-0.000012345m, cint), "-12.345e-6");
                Test(e, new RealVal(-0.0000000000000012345m, cint), "-1.2345e-15");
            }
        }
    }

    class RealFormatter : NumberFormatter {
        public string Prefix;

        public RealFormatter(string prefix, Regex regex, int groupIndex) : base(regex, groupIndex) {
            this.Prefix = prefix;
        }

        public override Val Parse(Match m) {
            System.Diagnostics.Debug.Assert(m.Groups[CaptureGroupIndex].Length > 0);
            var tok = m.Groups[CaptureGroupIndex].Value;
            return new RealVal(real.Parse(tok), new ValFormatHint(this));
        }

        protected override string OnFormat(Val val, EvalContext e) {
            if (val is RealVal) {
                return RealToString(val.AsReal, e);
            }
            else {
                return base.OnFormat(val, e);
            }
        }
    }

    class IntFormatter : NumberFormatter {
        public readonly int Radix;
        public string Prefix;

        public IntFormatter(int radix, string prefix, Regex regex, int groupIndex) : base(regex, groupIndex) {
            this.Radix = radix;
            this.Prefix = prefix;
        }

        public override Val Parse(Match m) {
            System.Diagnostics.Debug.Assert(m.Groups[CaptureGroupIndex].Length > 0);
            var tok = m.Groups[CaptureGroupIndex].Value;
            if (Radix == 10) {
                return new RealVal(real.Parse(tok), new ValFormatHint(this));
            }
            else {
                return new RealVal(Convert.ToInt32(tok, Radix), new ValFormatHint(this));
            }
        }

        protected override string OnFormat(Val val, EvalContext e) {
            if (val is RealVal) {
                var fval = val.AsReal;
                var i64val = val.AsLong;

                // 10進表記、かつ指数表記対象に該当する場合はデフォルトの数値表現を使う
                int exp = RMath.FLog10(val.AsReal);
                var s = e.Settings;
                bool enotation = 
                    Radix == 10 && 
                    s.ENotationEnabled && 
                    (exp >= s.ENotationExpPositiveMin || exp <= s.ENotationExpNegativeMax);

                if (fval != i64val || i64val < int.MinValue || int.MaxValue < i64val || enotation) {
                    // デフォルトの数値表現
                    return base.OnFormat(val, e);
                }
                else if (Radix == 10) {
                    // 10進表現
                    var abs64val = i64val >= 0 ? i64val : -i64val;
                    var ret = Convert.ToString(abs64val, Radix);
                    ret = Prefix + ret;
                    if (i64val < 0) ret = "-" + ret;
                    return ret;
                }
                else {
                    // 10進以外
                    return Prefix + Convert.ToString((Int32)i64val, Radix);
                }
            }
            else {
                return base.OnFormat(val, e);
            }
        }
    }
}
