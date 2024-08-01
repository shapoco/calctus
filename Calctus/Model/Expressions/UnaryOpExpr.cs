using System;
using Shapoco.Calctus.Model.Values;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Expressions {
    class UnaryOpExpr : OpExpr {
        public Expr A { get; private set; }

        public UnaryOpExpr(Token t, Expr a) : base(OpInfo.From(OpType.Unary, t).Code, t) {
#if DEBUG
            System.Diagnostics.Debug.Assert(OpCode.Type() == OpType.Unary);
#endif
            this.A = a;
        }

        public override bool CausesValueChange() => true;

        protected override Val OnEval(EvalContext e) {
            var a = A.Eval(e);
            if (a is ListVal aArray) {
                var aVals = (Val[])aArray.Raw;
                var results = new Val[aVals.Length];
                for (int i = 0; i < aVals.Length; i++) {
                    results[i] = scalarOperation(e, aVals[i]);
                }
                return new ListVal(results);
            }
            else {
                return scalarOperation(e, a);
            }
        }

        private Val scalarOperation(EvalContext e, Val a) {
            if (OpCode == OpCodes.Plus) return a;
            if (OpCode == OpCodes.ArithInv) return a.ArithInv(e);
            if (OpCode == OpCodes.BitNot) return a.BitNot(e);
            if (OpCode == OpCodes.LogicNot) return a.LogicNot(e);
            throw new NotImplementedException();
        }

        public override string ToString() => "(" + this.OpCode.GetSymbol() + A.ToString() + ")";
    }
}
