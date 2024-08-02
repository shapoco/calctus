using System;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Expressions {
    class UnaryOp : Operator {
        public Expr A { get; private set; }

        public UnaryOp(Expr a, Token t) : base(OpDef.Match(OpType.Unary, t), t) {
            this.A = a;
        }

        public override bool CausesValueChange() => true;

        protected override Val OnEval(EvalContext e) {
            var a = A.Eval(e);
            if (a is ArrayVal aArray) {
                var aVals = (Val[])aArray.Raw;
                var results = new Val[aVals.Length];
                for (int i = 0; i < aVals.Length; i++) {
                    results[i] = scalarOperation(e, aVals[i]);
                }
                return new ArrayVal(results).Format(a.FormatHint);
            }
            else {
                return scalarOperation(e, a);
            }
        }

        private Val scalarOperation(EvalContext e, Val a) {
            if (Method == OpDef.Plus) return a;
            if (Method == OpDef.ArithInv) return a.ArithInv(e);
            if (Method == OpDef.BitNot) return a.BitNot(e);
            if (Method == OpDef.LogicNot) return a.LogicNot(e);
            throw new NotImplementedException();
        }

        public override string ToString() => "(" + this.Method.Symbol + A.ToString() + ")";
    }
}
