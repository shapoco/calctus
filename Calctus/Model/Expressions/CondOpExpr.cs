using Shapoco.Calctus.Model.Values;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Expressions {
    /// <summary>条件演算子</summary>
    class CondOpExpr : Expr {
        public readonly Expr Cond;
        public readonly Expr TrueVal;
        public readonly Expr FalseVal;
        public CondOpExpr(Token t, Expr cond, Expr trueVal, Expr falseVal) : base(t) {
            Cond = cond;
            TrueVal = trueVal;
            FalseVal = falseVal;
        }

        public override bool CausesValueChange() => true;

        protected override Val OnEval(EvalContext ctx) {
            if (((BoolVal)Cond.Eval(ctx)).Raw) {
                return TrueVal.Eval(ctx);
            }
            else {
                return FalseVal.Eval(ctx);
            }
        }
    }
}
