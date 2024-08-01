using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Evaluations;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Values;
using Shapoco.Calctus.Model.Functions;

namespace Shapoco.Calctus.Model.Expressions {
    class LambdaExpr : Expr {
        public readonly FuncVal Val;

        public LambdaExpr(Token arrow, UserFuncDef func) : base(arrow) {
            Val = new FuncVal(func);
        }

        public override bool CausesValueChange() => false;

        protected override Val OnEval(EvalContext e) {
            return Val;
        }
    }
}
