using System;
using System.Collections.Generic;
using System.Linq;
using Shapoco.Calctus.Model.Values;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Evaluations;
using Shapoco.Calctus.Model.Maths;
using Shapoco.Calctus.Model.Functions.BuiltIns;

namespace Shapoco.Calctus.Model.Expressions {
    /// <summary>二項演算</summary>
    class BinaryOpExpr : OpExpr {
        public Expr A { get; private set; }
        public Expr B { get; private set; }

        public BinaryOpExpr(Token t, Expr a, Expr b) : base(OpInfo.From(OpType.Binary, t).Code, t) {
#if DEBUG
            //enable
            System.Diagnostics.Debug.Assert(OpCode.Type() == OpType.Binary);
#endif
            this.A = a;
            this.B = b;
        }

        public override bool CausesValueChange() => OpCode != OpCodes.Assign || B.CausesValueChange();

        protected override Val OnEval(EvalContext e) {
            if (OpCode == OpCodes.Assign) {
                // 代入
                if (A is IdExpr aRef) {
                    // 変数の参照
                    var val = B.Eval(e);
                    e.Ref(aRef.Id, allowCreate: true).Value = val;
                    return val;
                }
                else if (A is ArrayExpr aExpr) {
                    // 配列形式の代入
                    int n = aExpr.Elements.Length;
                    if (n <= 0) throw new EvalError(e, aExpr.Token, "At least one variable name is required.");
                    var refs = new IdExpr[n];
                    for (int i = 0; i < n; i++) {
                        if (aExpr.Elements[i] is IdExpr id) {
                            refs[i] = id;
                        }
                        else {
                            throw new EvalError(e, aExpr.Elements[i].Token, "Identifier is expected.");
                        }
                    }
                    var val = B.Eval(e);
                    if (!(val is ListVal arrayVal)) {
                        throw new EvalError(e, B.Token, "Array is required.");
                    }
                    if (refs.Length != arrayVal.Length) {
                        throw new EvalError(e, B.Token, "Array size mismatch.");
                    }
                    for (int i = 0; i < n; i++) {
                        e.Ref(refs[i].Id, allowCreate: true).Value = arrayVal[i];
                    }
                    return val;
                }
                else if (A is PartRefExpr aPart && aPart.Target is IdExpr aPartTarget) {
                    // Part Select を使った参照
                    var single = aPart.IsSingleIndex;
                    var aNear = aPart.IndexFrom.Eval(e).AsInt;
                    var aFar = single ? aNear : aPart.IndexTo.Eval(e).AsInt;

                    var bVal = B.Eval(e);
                    var aVarRef = e.Ref(aPartTarget.Id, allowCreate: false);
                    var aVal = aVarRef.Value;
                    if (aVal is ListVal aArray) {
                        // 配列の書き換え
                        if (aNear < 0) aNear = aArray.Length + aNear;
                        if (aFar < 0) aFar = aArray.Length + aFar;
                        if (aNear > aFar) throw new ArgumentOutOfRangeException();
                        if (aNear == aFar) {
                            aArray = aArray.Modify(aNear, aFar, new Val[] { bVal });
                        }
                        else {
                            aArray = aArray.Modify(aNear, aFar, (Val[])bVal.Raw);
                        }
                        aVal = aArray;
                    }
                    else if (aVal is StrVal strVal) {
                        // 部分文字列の書き換え
                        var str = strVal.ToStringForValue(e);
                        if (aNear < 0) aNear = str.Length + aNear;
                        if (aFar < 0) aFar = str.Length + aFar;
                        if (aNear > aFar) throw new ArgumentOutOfRangeException();
                        if (aNear < 0) throw new ArgumentOutOfRangeException();
                        if (aFar > str.Length) throw new ArgumentOutOfRangeException();
                        aVal = new StrVal(str.Substring(0, aNear) + bVal.ToStringForValue(e) + str.Substring(aFar));
                    }
                    else {
                        // ビットフィールドの書き換え
                        if (aNear < aFar) throw new ArgumentOutOfRangeException();
                        if (aNear < 0) throw new ArgumentOutOfRangeException();
                        if (aFar > 63) throw new ArgumentOutOfRangeException();
                        var w = aNear - aFar + 1;
                        var mask = w < 64 ? ((1L << w) - 1L) : unchecked((long)0xffffffffffffffff);
                        mask <<= aFar;
                        var buff = aVal.AsLong;
                        buff &= ~mask;
                        buff |= (bVal.AsLong << aFar) & mask;
                        aVal = new RealVal(buff, aVal.FormatFlags);
                    }
                    aVarRef.Value = aVal;
                    return bVal;
                }
                else {
                    throw new InvalidOperationException("Left hand of " + Token + " must be variant");
                }
            }
            else if (OpCode == OpCodes.InclusiveRange || OpCode == OpCodes.ExclusiveRange) {
                // range(a, b)
                bool inclusive = (OpCode == OpCodes.InclusiveRange);
                var aVal = A.Eval(e);
                var bVal = B.Eval(e);
                if (!aVal.IsInteger || !bVal.IsInteger) throw new CalctusError("Operand must be integer.");
                var a = aVal.AsDecimal;
                var b = bVal.AsDecimal;
                var step = a < b ? 1 : -1;
                return MathEx.Range(a, b, step, inclusive).ToVal(aVal.FormatFlags);
            }
            else if (OpCode == OpCodes.Frac) {
                // 分数の生成
                var a = A.Eval(e);
                var b = B.Eval(e);
                if (!e.EvalSettings.FractionEnabled) {
                    return A.Eval(e).Div(e, B.Eval(e));
                }
                else if (b is FracVal) {
                    return a.AsRealVal().Div(e, b);
                }
                else {
                    return FracVal.Normalize(new frac(a.AsDecimal, b.AsDecimal));
                }
            }
            else if (OpCode == OpCodes.Pow) {
                // pow(a, b)
                return ExponentialFuncs.Instance.pow.Call(e, new Val[] { A.Eval(e), B.Eval(e) });
            }
            else {
                var a = A.Eval(e);
                var b = B.Eval(e);
                if (a is ListVal aArray && !(b is ListVal)) {
                    // 配列とスカラ値のベクトル演算
                    var aVals = (Val[])aArray.Raw;
                    var results = new Val[aVals.Length];
                    for (int i = 0; i < aVals.Length; i++) {
                        results[i] = scalarOperation(e, aVals[i], b);
                    }
                    return new ListVal(results);
                }
                else if (!(a is ListVal) && b is ListVal bArray) {
                    // スカラ値と配列のベクトル演算
                    var bVals = (Val[])bArray.Raw;
                    var results = new Val[bVals.Length];
                    for (int i = 0; i < bVals.Length; i++) {
                        results[i] = scalarOperation(e, a, bVals[i]);
                    }
                    return new ListVal(results);
                }
                else if (a is ListVal aArray1 && b is ListVal bArray1) {
                    // 配列同士のベクトル演算
                    var aVals = aArray1.Raw;
                    var bVals = bArray1.Raw;
                    if (aVals.Length != bVals.Length) throw new CalctusError("Array size mismatch.");
                    var results = new Val[aVals.Length];
                    for (int i = 0; i < aVals.Length; i++) {
                        results[i] = scalarOperation(e, aVals[i], bVals[i]);
                    }
                    return new ListVal(results);
                }
                else {
                    // スカラ値同士の演算
                    return scalarOperation(e, a, b);
                }
            }
        }

        public static bool TryAutoCast<TIn, TOut>(EvalContext e, ref Val a, ref Val b, Func<EvalContext, TIn, TOut> convert) where TIn : Val where TOut : Val {
            if (a is TIn aTIn && b is TOut) {
                a = convert(e, aTIn);
                return true;
            }
            else if (a is TOut && b is TIn bTIn) {
                b = convert(e, bTIn);
                return true;
            }
            else {
                return false;
            }
        }

        private Val scalarOperation(EvalContext e, Val a, Val b) {
            if (!a.GetType().Equals(b.GetType())) {
                if (TryAutoCast<RealVal, FracVal>(e, ref a, ref b, (e, p) => new FracVal(p.Raw))) { }
                else if (TryAutoCast<Val, StrVal>(e, ref a, ref b, (e, p) => p.ToStringForValue(e).ToVal())) { }
                else {
                    throw new InvalidCastException(OpCode + " cannot be applied for " + a.CalctusTypeName + " and " + b.CalctusTypeName);
                }
            }

            // todo cast str --> array
            switch (OpCode) {
                case OpCodes.Mul: return a.Mul(e, b);
                case OpCodes.Div: return a.Div(e, b);
                case OpCodes.IDiv: return a.IDiv(e, b);
                case OpCodes.Mod: return a.Mod(e, b);
                case OpCodes.Add: return a.Add(e, b);
                case OpCodes.Sub: return a.Sub(e, b);
                case OpCodes.LogicShiftL: return a.LogicShiftL(e, b);
                case OpCodes.LogicShiftR: return a.LogicShiftR(e, b);
                case OpCodes.ArithShiftL: return a.ArithShiftL(e, b);
                case OpCodes.ArithShiftR: return a.ArithShiftR(e, b);
                case OpCodes.Grater: return a.Grater(e, b).ToVal();
                case OpCodes.GraterEqual: return a.GraterEqual(e, b).ToVal();
                case OpCodes.Less: return a.Less(e, b).ToVal();
                case OpCodes.LessEqual: return a.LessEqual(e, b).ToVal();
                case OpCodes.Equal: return a.Equals(e, b).ToVal();
                case OpCodes.NotEqual: return a.NotEqual(e, b).ToVal();
                case OpCodes.BitAnd: return a.BitAnd(e, b);
                case OpCodes.BitXor: return a.BitXor(e, b);
                case OpCodes.BitOr: return a.BitOr(e, b);
                case OpCodes.LogicAnd: return a.LogicAnd(e, b);
                case OpCodes.LogicOr: return a.LogicOr(e, b);
                default: throw new NotImplementedException("Operator not implemented: " + OpCode);
            }
        }

        public override string ToString() {
            return "(" + A.ToString() + " " + this.OpCode.GetSymbol() + " " + B.ToString() + ")";
        }
    }
}
