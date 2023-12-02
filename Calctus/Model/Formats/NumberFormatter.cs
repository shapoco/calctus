using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Mathematics;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Formats {
    abstract class NumberFormatter {
        public readonly Regex Pattern;
        public readonly FormatPriority Priority;

        public NumberFormatter(Regex ptn, FormatPriority priority) {
            Pattern = ptn;
            Priority = priority;
        }

        public abstract Val Parse(Match m);

        public virtual string Format(Val val, FormatSettingss fs) => OnFormat(val, fs);
        protected virtual string OnFormat(Val val, FormatSettingss fs) {
            if (val is ArrayVal aval) {
                var raw = (Val[])aval.Raw;
                var sb = new StringBuilder();
                sb.Append("[");
                for (int i = 0; i < raw.Length; i++) {
                    if (i > 0) sb.Append(", ");
                    sb.Append(raw[i].ToString(fs));
                }
                sb.Append("]");
                return sb.ToString();
            }
            else if (val is BoolVal) {
                return val.AsBool ? BoolVal.TrueKeyword : BoolVal.FalseKeyword;
            }
            else {
                return RealToString(val.AsReal, fs, true);
            }
        }

        public static readonly IntFormatter CStyleInt = new IntFormatter(10, "", new Regex(@"(?<digits>([1-9][0-9]*(_[0-9]+)*|0)([eE][+-]?[0-9]+(_[0-9]+)*)?)"), FormatPriority.Weak);
        public static readonly RealFormatter CStyleReal = new RealFormatter();
        public static readonly IntFormatter CStyleHex = new IntFormatter(16, "0x", new Regex(@"0[xX](?<digits>[0-9a-fA-F]+(_[0-9a-fA-F]+)*)"), FormatPriority.AlwaysLeft);
        public static readonly IntFormatter CStyleOct = new IntFormatter(8, "0", new Regex(@"0(?<digits>[0-7]+(_[0-7]+)*)"), FormatPriority.AlwaysLeft);
        public static readonly IntFormatter CStyleBin = new IntFormatter(2, "0b", new Regex(@"0[bB](?<digits>[01]+(_[01]+)*)"), FormatPriority.AlwaysLeft);
        public static readonly CharFormatter CStyleChar = new CharFormatter();
        public static readonly StringFormatter CStyleString = new StringFormatter();
        public static readonly SiPrefixFormatter SiPrefixed = new SiPrefixFormatter();
        public static readonly BinaryPrefixFormatter BinaryPrefixed = new BinaryPrefixFormatter();
        public static readonly DateTimeFormatter DateTime = new DateTimeFormatter();
        public static readonly WebColorFormatter WebColor = new WebColorFormatter();

        public static NumberFormatter[] NativeFormats => new NumberFormatter[] {
            CStyleInt, 
            CStyleReal, 
            CStyleHex, 
            CStyleOct, 
            CStyleBin,
            CStyleChar,
            CStyleString,
            SiPrefixed,
            BinaryPrefixed,
            DateTime,
            WebColor,
        };

        public static string RealToString(real val, FormatSettingss fs, bool allowENotation) {
            if (val == 0.0m) return "0";

            var sbDecFormat = new StringBuilder("0.");
            for (int i = 0; i < fs.DecimalLengthToDisplay; i++) {
                sbDecFormat.Append('#');
            }
            var decFormat = sbDecFormat.ToString();

            int exp = RMath.FLog10(val);
            if (allowENotation && fs.ENotationEnabled && exp >= fs.ENotationExpPositiveMin) {
                if (fs.ENotationAlignment) {
                    exp = (int)Math.Floor((double)exp / 3) * 3;
                }
                var frac = val / RMath.Pow10(exp);
                return frac.ToString(decFormat) + "e" + exp;
            }
            else if (allowENotation && fs.ENotationEnabled && exp <= fs.ENotationExpNegativeMax) {
                if (fs.ENotationAlignment) {
                    exp = (int)Math.Floor((double)exp / 3) * 3;
                }
                var frac = val * RMath.Pow10(-exp);
                return frac.ToString(decFormat) + "e" + exp;
            }
            else {
                return val.ToString(decFormat);
            }
        }

        public static void Test(FormatSettingss s, Val val, string str) {
            Assert.Equal(nameof(NumberFormatter), val.ToString(s), str);
        }

        public static void Test() {
            var cint = new FormatHint(CStyleInt);
            {
                var fs = new FormatSettingss();
                fs.DecimalLengthToDisplay = 28;
                fs.ENotationEnabled = false;
                fs.ENotationExpPositiveMin = 4;
                fs.ENotationExpNegativeMax = -3;
                fs.ENotationAlignment = false;
                Test(fs, new RealVal(0, cint), "0");
                Test(fs, new RealVal(1, cint), "1");
                Test(fs, new RealVal(12345, cint), "12345");
                Test(fs, new RealVal(1234500000000000, cint), "1234500000000000");
                Test(fs, new RealVal(-1, cint), "-1");
                Test(fs, new RealVal(-10, cint), "-10");
                Test(fs, new RealVal(-12345, cint), "-12345");
                Test(fs, new RealVal(-1234500000000000, cint), "-1234500000000000");
                Test(fs, new RealVal(0.1m, cint), "0.1");
                Test(fs, new RealVal(0.01m, cint), "0.01");
                Test(fs, new RealVal(0.001m, cint), "0.001");
                Test(fs, new RealVal(0.0000000000000012345m, cint), "0.0000000000000012345");
                Test(fs, new RealVal(-0.1m, cint), "-0.1");
                Test(fs, new RealVal(-0.01m, cint), "-0.01");
                Test(fs, new RealVal(-0.001m, cint), "-0.001");
                Test(fs, new RealVal(-0.0000000000000012345m, cint), "-0.0000000000000012345");
            }
            {
                var fs = new FormatSettingss();
                fs.DecimalLengthToDisplay = 28;
                fs.ENotationEnabled = true;
                fs.ENotationExpPositiveMin = 4;
                fs.ENotationExpNegativeMax = -3;
                fs.ENotationAlignment = false;
                Test(fs, new RealVal(0, cint), "0");
                Test(fs, new RealVal(1, cint), "1");
                Test(fs, new RealVal(1000, cint), "1000");
                Test(fs, new RealVal(9999, cint), "9999");
                Test(fs, new RealVal(10000, cint), "1e4");
                Test(fs, new RealVal(1234500000000000, cint), "1.2345e15");
                Test(fs, new RealVal(-1, cint), "-1");
                Test(fs, new RealVal(-10, cint), "-10");
                Test(fs, new RealVal(-1000, cint), "-1000");
                Test(fs, new RealVal(-9999, cint), "-9999");
                Test(fs, new RealVal(-10000, cint), "-1e4");
                Test(fs, new RealVal(-1234500000000000, cint), "-1.2345e15");
                Test(fs, new RealVal(0.1m, cint), "0.1");
                Test(fs, new RealVal(0.01m, cint), "0.01");
                Test(fs, new RealVal(0.00999m, cint), "9.99e-3");
                Test(fs, new RealVal(0.001m, cint), "1e-3");
                Test(fs, new RealVal(0.0000000000000012345m, cint), "1.2345e-15");
                Test(fs, new RealVal(-0.1m, cint), "-0.1");
                Test(fs, new RealVal(-0.01m, cint), "-0.01");
                Test(fs, new RealVal(-0.00999m, cint), "-9.99e-3");
                Test(fs, new RealVal(-0.001m, cint), "-1e-3");
                Test(fs, new RealVal(-0.0000000000000012345m, cint), "-1.2345e-15");
            }
            {
                var fs = new FormatSettingss();
                fs.DecimalLengthToDisplay = 28;
                fs.ENotationEnabled = true;
                fs.ENotationExpPositiveMin = 4;
                fs.ENotationExpNegativeMax = -3;
                fs.ENotationAlignment = true;
                Test(fs, new RealVal(0, cint), "0");
                Test(fs, new RealVal(1, cint), "1");
                Test(fs, new RealVal(1000, cint), "1000");
                Test(fs, new RealVal(9999, cint), "9999");
                Test(fs, new RealVal(10000, cint), "10e3");
                Test(fs, new RealVal(12345, cint), "12.345e3");
                Test(fs, new RealVal(123456, cint), "123.456e3");
                Test(fs, new RealVal(1234567, cint), "1.234567e6");
                Test(fs, new RealVal(1234500000000000, cint), "1.2345e15");
                Test(fs, new RealVal(-1, cint), "-1");
                Test(fs, new RealVal(-1000, cint), "-1000");
                Test(fs, new RealVal(-9999, cint), "-9999");
                Test(fs, new RealVal(-10000, cint), "-10e3");
                Test(fs, new RealVal(-12345, cint), "-12.345e3");
                Test(fs, new RealVal(-123456, cint), "-123.456e3");
                Test(fs, new RealVal(-1234567, cint), "-1.234567e6");
                Test(fs, new RealVal(-1234500000000000, cint), "-1.2345e15");
                Test(fs, new RealVal(0.1m, cint), "0.1");
                Test(fs, new RealVal(0.01m, cint), "0.01");
                Test(fs, new RealVal(0.00999m, cint), "9.99e-3");
                Test(fs, new RealVal(0.001m, cint), "1e-3");
                Test(fs, new RealVal(0.0012345m, cint), "1.2345e-3");
                Test(fs, new RealVal(0.00012345m, cint), "123.45e-6");
                Test(fs, new RealVal(0.000012345m, cint), "12.345e-6");
                Test(fs, new RealVal(0.0000000000000012345m, cint), "1.2345e-15");
                Test(fs, new RealVal(-0.1m, cint), "-0.1");
                Test(fs, new RealVal(-0.01m, cint), "-0.01");
                Test(fs, new RealVal(-0.00999m, cint), "-9.99e-3");
                Test(fs, new RealVal(-0.001m, cint), "-1e-3");
                Test(fs, new RealVal(-0.0012345m, cint), "-1.2345e-3");
                Test(fs, new RealVal(-0.00012345m, cint), "-123.45e-6");
                Test(fs, new RealVal(-0.000012345m, cint), "-12.345e-6");
                Test(fs, new RealVal(-0.0000000000000012345m, cint), "-1.2345e-15");
            }
            {
                var fs = new FormatSettingss();
                fs.DecimalLengthToDisplay = 5;
                fs.ENotationEnabled = false;
                Test(fs, new RealVal(10000, cint), "10000");
                Test(fs, new RealVal(100000, cint), "100000");
                Test(fs, new RealVal(1000000, cint), "1000000");
                Test(fs, new RealVal(0.0001m, cint), "0.0001");
                Test(fs, new RealVal(0.00001m, cint), "0.00001");
                Test(fs, new RealVal(0.000009m, cint), "0.00001");
                Test(fs, new RealVal(0.000005m, cint), "0.00001");
                Test(fs, new RealVal(0.000004999m, cint), "0");
            }
        }
    }
}
