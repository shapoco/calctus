using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Expressions {
    class Literal : Expr {
        public readonly Val Value;

        public Literal(Val v) : base(null) {
            this.Value = v;
        }
        public Literal(Token t) : base(t) {
            this.Value = ((LiteralTokenHint)t.Hint).Value;
        }

        public override bool CausesValueChange() => false;

        protected override Val OnEval(EvalContext ctx) => Value;
    }
}
