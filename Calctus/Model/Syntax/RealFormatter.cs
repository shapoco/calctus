using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.Syntax {
    class RealFormatter : NumberFormatter {
        public RealFormatter(Regex regex, int groupIndex) : base(regex, groupIndex) { }

        public override Val Parse(Match m) {
            System.Diagnostics.Debug.Assert(m.Groups[CaptureGroupIndex].Length > 0);
            var tok = m.Groups[CaptureGroupIndex].Value;
            return new RealVal(real.Parse(tok), new ValFormatHint(this));
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
