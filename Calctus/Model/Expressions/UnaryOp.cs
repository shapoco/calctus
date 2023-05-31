using System;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Expressions {
    /// <summary>単項演算</summary>
    class UnaryOp : Operator {
        public Expr A { get; private set; }

        public UnaryOp(Expr a, Token t) : base(OpDef.Match(OpType.Unary, t), t) {
            this.A = a;
        }

        protected override Val OnEval(EvalContext e) {
            if (Method == OpDef.Plus) return A.Eval(e);
            if (Method == OpDef.ArithInv) return A.Eval(e).ArithInv(e);
            if (Method == OpDef.BitNot) return A.Eval(e).BitNot(e);
            if (Method == OpDef.LogicNot) return A.Eval(e).LogicNot(e);
            throw new NotImplementedException();
        }

        public override string ToString() => "(" + this.Method.Symbol + A.ToString() + ")";
    }
}
