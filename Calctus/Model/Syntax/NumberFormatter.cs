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
        protected virtual string OnFormat(Val val) => val.AsDouble.ToString();

        public static readonly IntFormatter CStyleInt = new IntFormatter(10, "", new Regex(@"([1-9][0-9]*|0)([eE][+-]?[0-9]+)?"), 0 );
        public static readonly RealFormatter CStyleReal = new RealFormatter("", new Regex(@"([1-9][0-9]*|0)+\.[0-9]+([eE][+-]?[0-9]+)?"), 0);
        public static readonly IntFormatter CStyleHex = new IntFormatter(16, "0x", new Regex(@"0[xX]([0-9a-fA-F]+)"), 1);
        public static readonly IntFormatter CStyleOct = new IntFormatter(8, "0", new Regex(@"0([0-7]+)"), 1);
        public static readonly IntFormatter CStyleBin = new IntFormatter(2, "0b", new Regex(@"0[bB]([01]+)"), 1);

        //public static readonly IntFormatter VerilogInt = new IntFormatter(10, "'d", new Regex(@"([1-9][0-9]*)?'s?d([0-9]+)"), 2);
        //public static readonly IntFormatter VerilogHex = new IntFormatter(16, "'h", new Regex(@"([1-9][0-9]*)?'s?h([0-9a-fA-F]+)"), 2);
        //public static readonly IntFormatter VerilogOct = new IntFormatter(2, "'o", new Regex(@"([1-9][0-9]*)?'s?h([0-7]+)"), 2);
        //public static readonly IntFormatter VerilogBin = new IntFormatter(8, "'b", new Regex(@"([1-9][0-9]*)?'s?b([01]+)"), 2);

        public static NumberFormatter[] NativeFormats => new NumberFormatter[] {
            CStyleInt, 
            CStyleReal, 
            CStyleHex, 
            CStyleOct, 
            CStyleBin,
            //VerilogInt,
            //VerilogHex,
            //VerilogOct,
            //VerilogBin,
        };

        // private static readonly Regex IntNumber = new Regex(@"([1-9][0-9]*|0)([eE][+-]?[0-9]+)?");
        // private static readonly Regex RealNumber = new Regex(@"([1-9][0-9]*|0)+\.[0-9]+([eE][+-]?[0-9]+)?");
        // private static readonly Regex VerilogIntNumber = new Regex(@"([1-9][0-9]*)?'s?d[0-9]+");
        // private static readonly Regex VerilogHexNumber = new Regex(@"([1-9][0-9]*)?'s?h[0-9a-fA-F]+");
        // private static readonly Regex VerilogBinNumber = new Regex(@"([1-9][0-9]*)?'s?b[01]+");
        // private static readonly Regex VerilogOctNumber = new Regex(@"([1-9][0-9]*)?'s?h[0-7]+");
        // private static readonly Regex CStyleHexNumber = new Regex(@"0[xX][0-9a-fA-F]+");
        // private static readonly Regex CStyleBinNumber = new Regex(@"0[bB][01]+");
        // private static readonly Regex CStyleOctNumber = new Regex(@"0[0-7]+");
        private static readonly Regex TimeHMS = new Regex(@"[0-9]+h([0-9]+m)?([0-9]+(\.[0-9]+)?s)?");
        private static readonly Regex TimeMS = new Regex(@"[0-9]+m[0-9]+(\.[0-9]+)?s");
        private static readonly Regex TimeH = new Regex(@"[0-9]+\.[0-9]+h");
        // private static readonly Regex TimeM = new Regex(@"[0-9]+\.[0-9]+m"); // メートルと区別つかないので m のみはダメ
        private static readonly Regex TimeS = new Regex(@"[0-9]+(\.[0-9]+)?s");
        private static readonly Regex TimeHHMMSS = new Regex(@"([0-9]+:[0-9]+:[0-9]+(\.[0-9]+)?)?");
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
                var fval = val.AsDouble;
                var ival = val.AsLong;
                var ret = Math.Abs(fval).ToString();
                if (fval == ival) ret += ".0";
                ret = Prefix + ret;
                if (fval < 0.0) ret = "-" + ret;
                return ret;
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
