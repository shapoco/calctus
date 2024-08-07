﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Formats {
    class RealFormatter : NumberFormatter {
        public RealFormatter() : base(new Regex(@"([1-9][0-9]*(_[0-9]+)*|0?)\.[0-9]+(_[0-9]+)*([eE][+-]?[0-9]+(_[0-9]+)*)?"), FormatPriority.Weak) { }

        public override Val Parse(Match m) {
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
    }
}
