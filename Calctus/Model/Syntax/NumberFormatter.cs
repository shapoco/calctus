using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.UnitSystem;
using Shapoco.Calctus.Model.Standard;

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
        public static readonly WebColorFormatter WebColor = new WebColorFormatter();

        public static NumberFormatter[] NativeFormats => new NumberFormatter[] {
            CStyleInt, 
            CStyleReal, 
            CStyleHex, 
            CStyleOct, 
            CStyleBin,
            CStyleChar,
            WebColor,
        };

        public static string RealToString(real val, EvalContext e) {
            if (val == 0.0m) return "0.0";

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
                return new RealVal(Convert.ToInt64(tok, Radix), new ValFormatHint(this));
            }
        }

        protected override string OnFormat(Val val, EvalContext e) {
            if (val is RealVal) {
                var fval = val.AsReal;
                var ival = RMath.Truncate(fval);

                // 10進表記、かつ指数表記対象に該当する場合はデフォルトの数値表現を使う
                int exp = RMath.FLog10(val.AsReal);
                var s = e.Settings;
                bool enotation = 
                    Radix == 10 && 
                    s.ENotationEnabled && 
                    (exp >= s.ENotationExpPositiveMin || exp <= s.ENotationExpNegativeMax);

                if (fval != ival || ival < long.MinValue || long.MaxValue < ival || enotation) {
                    // デフォルトの数値表現
                    return base.OnFormat(val, e);
                }
                else if (Radix == 10) {
                    // 10進表現
                    var abs64val = ival >= 0 ? (decimal)ival : -(decimal)ival;
                    var ret = Convert.ToString(abs64val);
                    ret = Prefix + ret;
                    if (ival < 0) ret = "-" + ret;
                    return ret;
                }
                else {
                    // 10進以外
                    return Prefix + Convert.ToString((Int64)ival, Radix);
                }
            }
            else {
                return base.OnFormat(val, e);
            }
        }
    }

    class CharFormatter : NumberFormatter {
        public CharFormatter() : base(new Regex("'([^'\\\\]|\\\\[abfnrtv\\\\\'0]|\\\\o[0-7]{3}|\\\\x[0-9a-fA-F]{2}|\\\\u[0-9a-fA-F]{4})'"), 1) { }

        public override Val Parse(Match m) {
            System.Diagnostics.Debug.Assert(m.Groups[CaptureGroupIndex].Length > 0);
            var tok = m.Groups[CaptureGroupIndex].Value;
            char c;
            switch (tok) {
                case "\\a": c = '\a'; break;
                case "\\b": c = '\b'; break;
                case "\\f": c = '\f'; break;
                case "\\n": c = '\n'; break;
                case "\\r": c = '\r'; break;
                case "\\t": c = '\t'; break;
                case "\\v": c = '\v'; break;
                case "\\\\": c = '\\'; break;
                case "\\'": c = '\''; break;
                case "\\0": c = '\0'; break;
                default:
                    if (tok.StartsWith("\\o")) {
                        var code = Convert.ToUInt64(tok.Substring(2), 8);
                        if (code < char.MinValue || char.MaxValue < code) {
                            throw new CalctusError("Char code out of range.");
                        }
                        c = (char)code;
                    }
                    else if (tok.StartsWith("\\x") || tok.StartsWith("\\u")) {
                        var code = Convert.ToUInt64(tok.Substring(2), 16);
                        if (code < char.MinValue || char.MaxValue < code) {
                            throw new CalctusError("Char code out of range.");
                        }
                        c = (char)code;
                    }
                    else {
                        c = tok[0];
                    }
                    break;
            }
            return new RealVal(c, new ValFormatHint(this));
        }

        protected override string OnFormat(Val val, EvalContext e) {
            if (!val.IsInteger) {
                // 整数でない場合はデフォルトの数値表現を使用
                return base.OnFormat(val, e);
            }

            var ival = val.AsReal;
            if (ival < char.MinValue || char.MaxValue < ival) {
                // 小数やcharの範囲外の値はデフォルトの数値表現を使用
                return base.OnFormat(val, e);
            }

            var cval = (char)ival;
            switch (cval) {
                case '\a': return "'\\a'";
                case '\b': return "'\\b'";
                case '\f': return "'\\f'";
                case '\n': return "'\\n'";
                case '\r': return "'\\r'";
                case '\t': return "'\\t'";
                case '\v': return "'\\v'";
                case '\\': return "'\\\\'";
                case '\'': return "'\\''";
                case '\0': return "'\\0'";
                default:
                    if (char.IsLetter(cval) || cval == ' ') {
                        return "'" + cval + "'";
                    }
                    else {
                        var hex = "0000" + Convert.ToString(cval, 16);
                        return "'\\u" + hex.Substring(hex.Length - 4, 4) + "'";
                    }
            }
        }
    }

    class WebColorFormatter : NumberFormatter {
        public WebColorFormatter() : base(new Regex(@"#([0-9a-fA-F]+)"), 1) { }

        public override Val Parse(Match m) {
            System.Diagnostics.Debug.Assert(m.Groups[CaptureGroupIndex].Length > 0);
            var tok = m.Groups[CaptureGroupIndex].Value;
            if (tok.Length == 3) {
                var rgb = ColorSpace.Rgb444ToRgb888(Convert.ToInt32(tok, 16));
                return new RealVal(rgb, new ValFormatHint(this));
            }
            else if (tok.Length == 6) {
                var rgb = Convert.ToInt32(tok, 16);
                return new RealVal(rgb, new ValFormatHint(this));
            }
            else {
                throw new CalctusError("Invalid color format.");
            }
        }

        protected override string OnFormat(Val val, EvalContext e) {
            if (!(val is RealVal)) {
                return base.OnFormat(val, e);
            }

            var fval = val.AsReal;
            var ival = RMath.Truncate(fval);

            int exp = RMath.FLog10(val.AsReal);
            var s = e.Settings;
            if (fval != ival || ival < long.MinValue || long.MaxValue < ival) {
                // 小数やlongの範囲外の値はデフォルトの数値表現を使用
                return base.OnFormat(val, e);
            }
            else if (ival < 0 || 0xffffff < ival) {
                // RGB空間の範囲外は通常の16進数で表現
                return "0x" + Convert.ToString((int)ival, 16);
            }
            else {
                var hexStr = "000000" + Convert.ToString((int)ival, 16);
                return "#" + hexStr.Substring(hexStr.Length - 6);
            }
        }
    }
}
