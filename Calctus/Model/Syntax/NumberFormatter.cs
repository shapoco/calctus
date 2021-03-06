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

        public virtual string Format(Val val) => OnFormat(val);
        protected virtual string OnFormat(Val val) => DoubleToString(val.AsDouble);

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

        public static string DoubleToString(double val) {
#if true
            if (double.IsNaN(val)) return "NaN";
            if (double.IsNegativeInfinity(val)) return "-∞";
            if (double.IsInfinity(val)) return "∞";
            if (val == 0.0) return "0.0";

            var s = Settings.Instance;
            var exp = (int)Math.Truncate(Math.Log10(val));
            if (s.NumberFormat_Exp_Enabled && exp >= s.NumberFormat_Exp_PositiveMin) {
                if (s.NumberFormat_Exp_Alignment) {
                    exp = (int)Math.Floor((double)exp / 3) * 3;
                }
                var frac = val / Math.Pow(10.0, exp);
                return frac.ToString("0.##############") + "e+" + exp;
            }
            else if (s.NumberFormat_Exp_Enabled && exp <= s.NumberFormat_Exp_NegativeMax) {
                if (s.NumberFormat_Exp_Alignment) {
                    exp = (int)Math.Floor((double)exp / 3) * 3;
                }
                var frac = val * Math.Pow(10.0, -exp);
                return frac.ToString("0.##############") + "e" + exp;
            }
            else {
                return val.ToString("0.##############");
            }
#else
            return val.ToString();
#endif
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
            return new RealVal(double.Parse(tok), new ValFormatHint(this));
        }

        protected override string OnFormat(Val val) {
            if (val is RealVal) {
                return DoubleToString(val.AsDouble);
            }
            else {
                return base.OnFormat(val);
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
                return new RealVal(double.Parse(tok), new ValFormatHint(this));
            }
            else {
                return new RealVal(Convert.ToInt32(tok, Radix), new ValFormatHint(this));
            }
        }

        protected override string OnFormat(Val val) {
            if (val is RealVal) {
                var fval = val.AsDouble;
                var i64val = val.AsLong;
                if (fval != i64val || i64val < int.MinValue || int.MaxValue < i64val) {
                    return base.OnFormat(val);
                }
                if (Radix == 10) {
                    var abs64val = i64val >= 0 ? i64val : -i64val;
                    var ret = Convert.ToString(abs64val, Radix);
                    ret = Prefix + ret;
                    if (i64val < 0) ret = "-" + ret;
                    return ret;
                }
                else {
                    return Prefix + Convert.ToString((Int32)i64val, Radix);
                }
            }
            else {
                return base.OnFormat(val);
            }
        }
    }
}
