using System;
using System.Collections.Generic;
using System.Linq;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Evaluations;
using Shapoco.Calctus.Model.Mathematics;
using Shapoco.Calctus.Model.Functions;

namespace Shapoco.Calctus.Model.Expressions {
    /// <summary>二項演算</summary>
    class BinaryOp : Operator {
        public Expr A { get; private set; }
        public Expr B { get; private set; }
        
        public BinaryOp(Token t, Expr a, Expr b) : base(OpDef.Match(OpType.Binary, t), t) {
            this.A = a;
            this.B = b;
        }

        public override bool CausesValueChange() => Method != OpDef.Assign || B.CausesValueChange();

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
                    var from = pRef.IndexFrom.Eval(e).AsInt;
                    var to = pRef.IndexTo.Eval(e).AsInt;
                    if (from < to) throw new ArgumentOutOfRangeException();

                    var val = B.Eval(e);
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
                        if (from < 0 || 63 < from) throw new ArgumentOutOfRangeException();
                        if (to < 0 || 63 < to) throw new ArgumentOutOfRangeException();
                        var w = from - to + 1;
                        var mask = w < 64 ? ((1L << w) - 1L) : unchecked((long)0xffffffffffffffff);
                        mask <<= to;
                        var buff = varVal.AsLong;
                        buff &= ~mask;
                        buff |= (val.AsLong << to) & mask;
                        varVal = new RealVal(buff, varVal.FormatHint);
                    }
                    varRef.Value = varVal;
                    return val;
                }
                else {
                    throw new InvalidOperationException("Left hand of " + Token + " must be variant");
                }
            }
            else if (Method == OpDef.InclusiveRange || Method == OpDef.ExclusiveRange) {
                bool inclusive = (Method == OpDef.InclusiveRange);
                var aVal = A.Eval(e);
                var bVal = B.Eval(e);
                if (!aVal.IsInteger || !bVal.IsInteger) throw new CalctusError("Operand must be integer.");
                var a = aVal.AsReal;
                var b = bVal.AsReal;
                return new ArrayVal(RMath.Range(a, b, a < b ? 1 : -1, inclusive));
            }
            else if (Method == OpDef.Frac) {
                var a = A.Eval(e);
                var b = B.Eval(e);
                if (!e.EvalSettings.FractionEnabled) {
                    return A.Eval(e).Div(e, B.Eval(e));
                }
                else if (b is FracVal) {
                    return a.AsRealVal().Div(e, b);
                }
                else {
                    return FracVal.Normalize(new frac(a.AsReal, b.AsReal), a.FormatHint);
                }
            }
            else if (Method == OpDef.Pow) {
                return EmbeddedFuncDef.pow.Call(e, new Val[] { A.Eval(e), B.Eval(e) });
            }
            else {
                var a = A.Eval(e);
                var b = B.Eval(e);
                if (a is ArrayVal aArray && !(b is ArrayVal)) {
                    var aVals = (Val[])aArray.Raw;
                    var results = new Val[aVals.Length];
                    for (int i = 0; i < aVals.Length; i++) {
                        results[i] = scalarOperation(e, aVals[i], b);
                    }
                    return new ArrayVal(results).Format(a.FormatHint);
                }
                else if (!(a is ArrayVal) && b is ArrayVal bArray) {
                    var bVals = (Val[])bArray.Raw;
                    var results = new Val[bVals.Length];
                    for (int i = 0; i < bVals.Length; i++) {
                        results[i] = scalarOperation(e, a, bVals[i]);
                    }
                    return new ArrayVal(results).Format(b.FormatHint);
                }
                else if (a is ArrayVal aArray1 && b is ArrayVal bArray1) {
                    var aVals = (Val[])aArray1.Raw;
                    var bVals = (Val[])bArray1.Raw;
                    if (aVals.Length != bVals.Length) throw new CalctusError("Array size mismatch.");
                    var results = new Val[aVals.Length];
                    for (int i = 0; i < aVals.Length; i++) {
                        results[i] = scalarOperation(e, aVals[i], bVals[i]);
                    }
                    return new ArrayVal(results).Format(a.FormatHint);
                }
                else {
                    return scalarOperation(e, a, b);
                }
            }
        }

        private Val scalarOperation(EvalContext e, Val a, Val b) {
            if (Method == OpDef.Mul) return a.Mul(e, b);
            if (Method == OpDef.Div) return a.Div(e, b);
            if (Method == OpDef.IDiv) return a.IDiv(e, b);
            if (Method == OpDef.Mod) return a.Mod(e, b);
            if (Method == OpDef.Add) return a.Add(e, b);
            if (Method == OpDef.Sub) return a.Sub(e, b);
            if (Method == OpDef.LogicShiftL) return a.LogicShiftL(e, b);
            if (Method == OpDef.LogicShiftR) return a.LogicShiftR(e, b);
            if (Method == OpDef.ArithShiftL) return a.ArithShiftL(e, b);
            if (Method == OpDef.ArithShiftR) return a.ArithShiftR(e, b);
            if (Method == OpDef.Grater) return a.Grater(e, b);
            if (Method == OpDef.GraterEqual) return a.GraterEqual(e, b);
            if (Method == OpDef.Less) return a.Less(e, b);
            if (Method == OpDef.LessEqual) return a.LessEqual(e, b);
            if (Method == OpDef.Equal) return a.Equals(e, b);
            if (Method == OpDef.NotEqual) return a.NotEqual(e, b);
            if (Method == OpDef.BitAnd) return a.BitAnd(e, b);
            if (Method == OpDef.BitXor) return a.BitXor(e, b);
            if (Method == OpDef.BitOr) return a.BitOr(e, b);
            if (Method == OpDef.LogicAnd) return a.LogicAnd(e, b);
            if (Method == OpDef.LogicOr) return a.LogicOr(e, b);
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "(" + A.ToString() + " " + this.Method.Symbol + " " + B.ToString() + ")";
        }
    }
}
