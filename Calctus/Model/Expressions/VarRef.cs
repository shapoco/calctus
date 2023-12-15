using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Evaluations;
using Shapoco.Calctus.Model.Functions;

namespace Shapoco.Calctus.Model.Expressions {
    class VarRef : Expr {
        public Token RefName => Token;
        public VarRef(Token name) : base(name) { }

        public override bool CausesValueChange() => true;

        protected override Val OnEval(EvalContext ctx) {
            if (ctx.Ref(RefName, false, out Var v)) {
                return v.Value;
            }
            else if (ctx.SolveFunc(RefName.Text, out FuncDef f)) {
                return new FuncVal(f);
            }
            else {
                throw new EvalError(ctx, RefName, "variant or function not found.");
            }
        }
    }
}
