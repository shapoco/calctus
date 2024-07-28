using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Maths;
using Shapoco.Calctus.Model.Maths.Types;
using Shapoco.Calctus.Model.Parsers;

namespace Shapoco.Calctus.Model.Formats {
    class IntFormat : ValFormat {
        private static readonly Regex _patternInt = new Regex(@"(?<digits>([1-9][0-9]*(_[0-9]+)*|0)([eE][+-]?[0-9]+(_[0-9]+)*)?)");
        private static readonly Regex _patternHex = new Regex(@"0[xX](?<digits>[0-9a-fA-F]+(_[0-9a-fA-F]+)*)");
        private static readonly Regex _patternOct = new Regex(@"0(?<digits>[0-7]+(_[0-7]+)*)");
        private static readonly Regex _patternBin = new Regex(@"0[bB](?<digits>[01]+(_[01]+)*)");

        private static IntFormat _cStyleInt;
        private static IntFormat _cStyleHex;
        private static IntFormat _cStyleOct;
        private static IntFormat _cStyleBin;
        public static IntFormat Instance_CStyleInt => (_cStyleInt != null) ? _cStyleInt : (_cStyleInt = new IntFormat(10, "", _patternInt, FormatPriority.Weak));
        public static IntFormat Instance_CStyleHex => (_cStyleHex != null) ? _cStyleHex : (_cStyleHex = new IntFormat(16, "0x", _patternHex, FormatPriority.AlwaysLeft));
        public static IntFormat Instance_CStyleOct => (_cStyleOct != null) ? _cStyleOct : (_cStyleOct = new IntFormat(8, "0", _patternOct, FormatPriority.AlwaysLeft));
        public static IntFormat Instance_CStyleBin => (_cStyleBin != null) ? _cStyleBin : (_cStyleBin = new IntFormat(2, "0b", _patternBin, FormatPriority.AlwaysLeft));

        public readonly int Radix;
        public readonly string Prefix;

        private IntFormat(int radix, string prefix, Regex regex, FormatPriority priority)
                : base(TokenType.NumericLiteral, regex, priority) {
            this.Radix = radix;
            this.Prefix = prefix;
        }

        protected override Val OnParse(Match m) {
            System.Diagnostics.Debug.Assert(m.Groups["digits"].Length > 0);
            var tok = m.Groups["digits"].Value;
            if (Radix == 10) {
                return new RealVal(DecMath.Parse(tok.Replace("_", "")), new FormatHint(this));
            }
            else {
                return new RealVal(Convert.ToInt64(tok.Replace("_", ""), Radix), new FormatHint(this));
            }
        }

        protected override string OnFormat(Val val, FormatSettings fs) {
            if (val is RealVal) {
                var fval = val.AsDecimal;
                var ival = Math.Truncate(fval);

                // 10進表記、かつ指数表記対象に該当する場合はデフォルトの数値表現を使う
                int exp = MathEx.FLog10Abs(val.AsDecimal);
                bool enotation =
                    Radix == 10 &&
                    fs.ENotationEnabled &&
                    (exp >= fs.ENotationExpPositiveMin || exp <= fs.ENotationExpNegativeMax);

                if (fval != ival || ival < long.MinValue || long.MaxValue < ival || enotation) {
                    // デフォルトの数値表現
                    return base.OnFormat(val, fs);
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
                return base.OnFormat(val, fs);
            }
        }
    }
}

