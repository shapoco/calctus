using System;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Expressions {
    /// <summary>二項演算</summary>
    class BinaryOp : Operator {
        public Expr A { get; private set; }
        public Expr B { get; private set; }
        
        public BinaryOp(Token t, Expr a, Expr b) : base(OpDef.Match(OpType.Binary, t), t) {
            this.A = a;
            this.B = b;
        }

        protected override Val OnEval(EvalContext e) {
            if (Method == OpDef.Assign) {
                if (A is VarRef aRef) {
                    // 変数の参照
                    var val = B.Eval(e);
                    e.Ref(aRef.RefName, allowCreate: true).Value = val;
                    return val;
                }
                else if (A is PartRef pRef && pRef.Target is VarRef tRef) {
                    // Part Select を使った参照
                    var val = B.Eval(e);
                    var from = pRef.IndexFrom.Eval(e).AsInt;
                    var to = pRef.IndexTo.Eval(e).AsInt;
                    var varRef = e.Ref(tRef.RefName, allowCreate: false);
                    var varVal = varRef.Value;
                    if (varVal is ArrayVal array) {
                        // 配列の書き換え
                        if (from == to) {
                            array = array.Modify(from, to, new Val[] { val });
                        }
                        else {
                            array = array.Modify(from, to, (Val[])val.Raw);
                        }
                        varVal = array;
                    }
                    else {
                        // ビットフィールドの書き換え
                        var mask = ((1L << (from - to + 1)) - 1L) << to;
                        var buff = varVal.AsLong;
                        buff &= ~mask;
                        buff |= (val.AsLong << to) & mask;
                        varVal = new RealVal(buff, varVal.FormatHint);
                    }
                    varRef.Value = varVal;
                    return val;
                }
                else {
                    throw new InvalidOperationException("left hand of " + Token + " must be variant");
                }
            }
            if (Method == OpDef.Frac) {
                var a = A.Eval(e);
                var b = B.Eval(e);
                if (b is FracVal) {
                    return a.AsRealVal().Div(e, b);
                }
                else {
                    return FracVal.Normalize(new frac(a.AsReal, b.AsReal), a.FormatHint);
                }
            }
            if (Method == OpDef.Pow) return FuncDef.pow.Call(e, new Val[] { A.Eval(e), B.Eval(e) });
            if (Method == OpDef.Mul) return A.Eval(e).Mul(e, B.Eval(e));
            if (Method == OpDef.Div) return A.Eval(e).Div(e, B.Eval(e));
            if (Method == OpDef.IDiv) return A.Eval(e).IDiv(e, B.Eval(e));
            if (Method == OpDef.Mod) return A.Eval(e).Mod(e, B.Eval(e));
            if (Method == OpDef.Add) return A.Eval(e).Add(e, B.Eval(e));
            if (Method == OpDef.Sub) return A.Eval(e).Sub(e, B.Eval(e));
            if (Method == OpDef.LogicShiftL) return A.Eval(e).LogicShiftL(e, B.Eval(e));
            if (Method == OpDef.LogicShiftR) return A.Eval(e).LogicShiftR(e, B.Eval(e));
            if (Method == OpDef.ArithShiftL) return A.Eval(e).ArithShiftL(e, B.Eval(e));
            if (Method == OpDef.ArithShiftR) return A.Eval(e).ArithShiftR(e, B.Eval(e));
            if (Method == OpDef.Grater) return A.Eval(e).Grater(e, B.Eval(e));
            if (Method == OpDef.GraterEqual) return A.Eval(e).GraterEqual(e, B.Eval(e));
            if (Method == OpDef.Less) return A.Eval(e).Less(e, B.Eval(e));
            if (Method == OpDef.LessEqual) return A.Eval(e).LessEqual(e, B.Eval(e));
            if (Method == OpDef.Equal) return A.Eval(e).Equal(e, B.Eval(e));
            if (Method == OpDef.NotEqual) return A.Eval(e).NotEqual(e, B.Eval(e));
            if (Method == OpDef.BitAnd) return A.Eval(e).BitAnd(e, B.Eval(e));
            if (Method == OpDef.BitXor) return A.Eval(e).BitXor(e, B.Eval(e));
            if (Method == OpDef.BitOr) return A.Eval(e).BitOr(e, B.Eval(e));
            if (Method == OpDef.LogicAnd) return A.Eval(e).LogicAnd(e, B.Eval(e));
            if (Method == OpDef.LogicOr) return A.Eval(e).LogicOr(e, B.Eval(e));
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "(" + A.ToString() + " " + this.Method.Symbol + " " + B.ToString() + ")";
        }
    }
}
