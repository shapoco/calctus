using Shapoco.Calctus.Model.Values;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Expressions {
    class LiteralExpr : Expr {
        public readonly Val Value;

        public LiteralExpr(Val v) : base(null) {
            this.Value = v;
        }
        public LiteralExpr(LiteralToken t) : base(t) {
            this.Value = t.Value;
        }

        public override bool CausesValueChange() => false;

        protected override Val OnEval(EvalContext ctx) => Value;
    }
}
