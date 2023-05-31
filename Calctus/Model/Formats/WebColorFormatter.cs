using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Standards;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Mathematics;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Formats {
    class WebColorFormatter : NumberFormatter {
        public WebColorFormatter() : base(new Regex(@"#([0-9a-fA-F]+)"), FormatPriority.LeftPriority) { }

        public override Val Parse(Match m) {
            var tok = m.Groups[1].Value;
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

        protected override string OnFormat(Val val, EvalContext e) {
            if (!(val is RealVal)) {
                return base.OnFormat(val, e);
            }

            var fval = val.AsReal;
            var ival = RMath.Truncate(fval);
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
