using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
        public static readonly CharFormatter CStyleChar = new CharFormatter();
        public static readonly DateTimeFormatter DateTime = new DateTimeFormatter();
        public static readonly WebColorFormatter WebColor = new WebColorFormatter();

        public static NumberFormatter[] NativeFormats => new NumberFormatter[] {
            CStyleInt, 
            CStyleReal, 
            CStyleHex, 
            CStyleOct, 
            CStyleBin,
            CStyleChar,
            DateTime,
            WebColor,
        };

        public static string RealToString(real val, EvalContext e) {
            if (val == 0.0m) return 0.0m.ToString("n");

            var s = e.Settings;

            var sbDecFormat = new StringBuilder("0.");
            for (int i = 0; i < s.DecimalLengthToDisplay; i++) {
                sbDecFormat.Append('#');
            }
            var decFormat = sbDecFormat.ToString();

            int exp = RMath.FLog10(val);
            if (s.ENotationEnabled && exp >= s.ENotationExpPositiveMin) {
                if (s.ENotationAlignment) {
                    exp = (int)Math.Floor((double)exp / 3) * 3;
                }
                var frac = val / RMath.Pow10(exp);
                return frac.ToString(decFormat) + "e+" + exp;
            }
            else if (s.ENotationEnabled && exp <= s.ENotationExpNegativeMax) {
                if (s.ENotationAlignment) {
                    exp = (int)Math.Floor((double)exp / 3) * 3;
                }
                var frac = val * RMath.Pow10(-exp);
                return frac.ToString(decFormat) + "e" + exp;
            }
            else {
                decFormat = "n" + s.DecimalLengthToDisplay;

                return val.ToString(decFormat);
            }
        }

        public static void Test(EvalContext e, Val val, string str) {
            Assert.Equal(nameof(NumberFormatter), val.ToString(e), str);
        }

        public static void Test() {
            var cint = new ValFormatHint(CStyleInt);
            {
                EvalContext e = new EvalContext();
                e.Settings.DecimalLengthToDisplay = 28;
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
                e.Settings.DecimalLengthToDisplay = 28;
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
                e.Settings.DecimalLengthToDisplay = 28;
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
            {
                EvalContext e = new EvalContext();
                e.Settings.DecimalLengthToDisplay = 5;
                e.Settings.ENotationEnabled = false;
                Test(e, new RealVal(10000, cint), "10000");
                Test(e, new RealVal(100000, cint), "100000");
                Test(e, new RealVal(1000000, cint), "1000000");
                Test(e, new RealVal(0.0001m, cint), "0.0001");
                Test(e, new RealVal(0.00001m, cint), "0.00001");
                Test(e, new RealVal(0.000009m, cint), "0.00001");
                Test(e, new RealVal(0.000005m, cint), "0.00001");
                Test(e, new RealVal(0.000004999m, cint), "0");
            }
        }
    }
}
