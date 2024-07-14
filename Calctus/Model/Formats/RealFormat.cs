using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Mathematics;
using Shapoco.Calctus.Model.Parsers;

namespace Shapoco.Calctus.Model.Formats {
    class RealFormat : ValFormat {
        private static readonly Regex pattern = new Regex(@"([1-9][0-9]*(_[0-9]+)*|0?)\.[0-9]+(_[0-9]+)*([eE][+-]?[0-9]+(_[0-9]+)*)?");

        private static RealFormat _instance;
        public static RealFormat Instance => (_instance != null) ? _instance : (_instance = new RealFormat());

        private RealFormat() : base(TokenType.NumericLiteral, pattern, FormatPriority.Weak) { }

        protected override Val OnParse(Match m) {
            return new RealVal(real.Parse(m.Value), new FormatHint(this));
        }

        protected override string OnFormat(Val val, FormatSettings fs) {
            if (val is RealVal) {
                return RealToString(val.AsReal, fs, true);
            }
            else {
                return base.OnFormat(val, fs);
            }
        }

        public static string RealToString(real val, FormatSettings fs, bool allowENotation) {
            if (val == 0.0m) return "0";

            var sbDecFormat = new StringBuilder("0.");
            for (int i = 0; i < fs.DecimalLengthToDisplay; i++) {
                sbDecFormat.Append('#');
            }
            var decFormat = sbDecFormat.ToString();

            int exp = RMath.FLog10Abs(val);
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

    }
}
