using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Standards;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Maths;
using Shapoco.Calctus.Model.Parsers;

namespace Shapoco.Calctus.Model.Formats {
    class WebColorFormat : ValFormat {
        private static readonly Regex pattern = new Regex(@"#(?<hex>[0-9a-fA-F]+)");

        private static WebColorFormat _instance;
        public static WebColorFormat Instance => (_instance != null) ? _instance : (_instance = new WebColorFormat());

        private WebColorFormat() : base(TokenType.NumericLiteral, pattern, FormatPriority.AlwaysLeft) { }

        protected override Val OnParse(Match m) {
            var tok = m.Groups["hex"].Value;
            if (tok.Length == 3) {
                var rgb = ColorSpace.Rgb444ToRgb888(Convert.ToInt32(tok, 16));
                return new RealVal(rgb, new FormatHint(this));
            }
            else if (tok.Length == 6) {
                var rgb = Convert.ToInt32(tok, 16);
                return new RealVal(rgb, new FormatHint(this));
            }
            else {
                throw new CalctusError("Invalid color format.");
            }
        }

        protected override string OnFormat(Val val, FormatSettings fs) {
            if (!(val is RealVal)) {
                return base.OnFormat(val, fs);
            }

            var fval = val.AsDecimal;
            var ival = Math.Truncate(fval);
            if (fval != ival || ival < long.MinValue || long.MaxValue < ival) {
                // 小数やlongの範囲外の値はデフォルトの数値表現を使用
                return base.OnFormat(val, fs);
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
