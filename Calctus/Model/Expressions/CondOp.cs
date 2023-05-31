using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Expressions {
    /// <summary>条件演算子</summary>
    class CondOp : Expr {
        public readonly Expr Cond;
        public readonly Expr TrueVal;
        public readonly Expr FalseVal;
        public CondOp(Token t, Expr cond, Expr trueVal, Expr falseVal) : base(t) {
            Cond = cond;
            TrueVal = trueVal;
            FalseVal = falseVal;
        }
        protected override Val OnEval(EvalContext ctx) {
            if (Cond.Eval(ctx).AsBool) {
                return TrueVal.Eval(ctx);
            }
            else {
                return FalseVal.Eval(ctx);
            }
        }
    }
}
