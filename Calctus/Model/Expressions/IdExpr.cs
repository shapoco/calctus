using Shapoco.Calctus.Model.Values;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Evaluations;
using Shapoco.Calctus.Model.Functions;

namespace Shapoco.Calctus.Model.Expressions {
    class IdExpr : Expr {
        public Token Id => Token;
        public IdExpr(Token id) : base(id) { }

        public override bool CausesValueChange() => true;

        protected override Val OnEval(EvalContext ctx) {
            if (ctx.Ref(Id, false, out Var v)) {
                return v.Value;
            }
            else if (ctx.SolveFunc(Id.Text, out FuncDef f)) {
                return new FuncVal(f);
            }
            else {
                throw new EvalError(ctx, Id, "Variant or function '" + Id.Text + "' not found.");
            }
        }
    }
}
