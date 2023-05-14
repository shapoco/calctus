using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.Syntax {
    class RealFormatter : NumberFormatter {
        public RealFormatter() : base(new Regex(@"([1-9][0-9]*|0)\.[0-9]+([eE][+-]?[0-9]+)?"), FormatPriority.Neutral) { }

        public override Val Parse(Match m) {
            return new RealVal(real.Parse(m.Value), new ValFormatHint(this));
        }

        protected override string OnFormat(Val val, EvalContext e) {
            if (val is RealVal) {
                return RealToString(val.AsReal, e, true);
            }
            else {
                return base.OnFormat(val, e);
            }
        }
    }
}
