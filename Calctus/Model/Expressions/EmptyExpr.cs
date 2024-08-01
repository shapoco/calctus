﻿using Shapoco.Calctus.Model.Values;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Expressions {
    /// <summary>空の式</summary>
    class EmptyExpr : Expr {
        public EmptyExpr() : base(Token.Empty) { }

        public override bool CausesValueChange() => false;

        protected override Val OnEval(EvalContext ctx) {
            throw new CalctusError("empty expression");
        }
    }
}
