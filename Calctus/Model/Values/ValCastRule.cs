using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Maths;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Values {
    abstract class ValCastRule {
        public abstract Type SrcType { get; }
        public abstract Type DestType { get; }
        public Val Cast(EvalContext e, Val val, object tag) {
            var ret = OnCast(val, out bool degr, tag);
            if (degr) e.ReportDegrade(this);
            return ret;
        }
        protected abstract Val OnCast(Val val, out bool degrade, object tag);
    }

    class CastRule<TSrc, TDest> : ValCastRule where TSrc : Val where TDest : Val {
        public override Type SrcType => typeof(TSrc);
        public override Type DestType => typeof(TDest);
        public readonly ValCastRuleFlags Flags;
        public readonly CastFunc<TSrc, TDest> CastFunc;
        public CastRule(ValCastRuleFlags flags, CastFunc<TSrc, TDest> castFunc) {
            this.Flags = flags;
            this.CastFunc = castFunc;
        }
        protected override Val OnCast(Val val, out bool degrade, object tag)
            => CastFunc(((TSrc)val), out degrade, tag);
    }
}
