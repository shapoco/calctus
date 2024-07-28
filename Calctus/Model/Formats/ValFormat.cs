using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Maths;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Formats {
    abstract class ValFormat {
        public readonly TokenType TokenType;
        public readonly Regex Pattern;
        public readonly FormatPriority Priority;

        public ValFormat(TokenType tokenType, Regex pattern, FormatPriority priority) {
            this.TokenType = tokenType;
            this.Pattern = pattern;
            this.Priority = priority;
        }

        public Val Parse(Match m) => OnParse(m);
        protected abstract Val OnParse(Match m);

        public string Format(Val val, FormatSettings fs) => OnFormat(val, fs);
        protected virtual string OnFormat(Val val, FormatSettings fs) {
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
                return BoolFormat.FormatAsStringLiteral(val.AsBool);
            }
            else if (val is StrVal) {
                return StringFormat.FormatAsStringLiteral(val.AsString);
            }
            else {
                return RealFormat.RealToString(val.AsDecimal, fs, true);
            }
        }

        public static IntFormat CStyleInt => IntFormat.Instance_CStyleInt;
        public static RealFormat CStyleReal => RealFormat.Instance;
        public static IntFormat CStyleHex => IntFormat.Instance_CStyleHex;
        public static IntFormat CStyleOct => IntFormat.Instance_CStyleOct;
        public static IntFormat CStyleBin => IntFormat.Instance_CStyleBin;
        public static CharFormat CStyleChar => CharFormat.Instance;
        public static StringFormat CStyleString => StringFormat.Instance;
        public static BoolFormat CStyleBool => BoolFormat.Instance;
        public static SiPrefixFormat SiPrefixed => SiPrefixFormat.Instance;
        public static BinaryPrefixFormat BinaryPrefixed => BinaryPrefixFormat.Instance;
        public static DateTimeFormat DateTime => DateTimeFormat.Instance;
        public static RelativeTimeFormat RelativeTime => RelativeTimeFormat.Instance;
        public static WeekdayFormat Weekday => WeekdayFormat.Instance;
        public static WebColorFormat WebColor => WebColorFormat.Instance;

        public static readonly ValFormat[] NativeFormats = {
            CStyleInt,
            CStyleReal,
            CStyleHex,
            CStyleOct,
            CStyleBin,
            CStyleChar,
            CStyleString,
            CStyleBool,
            SiPrefixed,
            BinaryPrefixed,
            DateTime,
            RelativeTime,
            Weekday,
            WebColor,
        };

        public static void Test(FormatSettings s, Val val, string str) {
            Assert.Equal(nameof(ValFormat), val.ToString(s), str);
        }

        public static void Test() {
            var cint = new FormatHint(CStyleInt);
            {
                var fs = new FormatSettings();
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
                var fs = new FormatSettings();
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
                var fs = new FormatSettings();
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
                var fs = new FormatSettings();
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
