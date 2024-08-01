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
    class ParenthesisExpr : Expr {
        public readonly Expr[] Exprs;
        public readonly Token Comma;

        public override bool CausesValueChange() => Exprs.Any(p => p.CausesValueChange());

        public ParenthesisExpr(Token token, Token firstComma, Expr[] exprs) : base(token) {
            this.Exprs = exprs;
            this.Comma = firstComma;
        }

        protected override Val OnEval(EvalContext e) {
            if (Exprs.Length == 0) {
                throw new EvalError(e, Token, "Empty expression");
            }
            else if (Exprs.Length >=2) {
                throw new EvalError(e, Comma, "')' is expected.");
            }
            else {
                return Exprs[0].Eval(e);
            }
        }
    }
}
