using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Evaluations;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Functions;

namespace Shapoco.Calctus.Model.Expressions {
    class AsterExpr : Expr{
        public readonly IdExpr Id;
        public AsterExpr(Token t, IdExpr id) : base(t) {
            Id = id;
        }

        public override bool CausesValueChange() => true;

        protected override Val OnEval(EvalContext e) {
            throw new EvalError(e, Token, "Unexpected asterisk.");
        }
    }
}
