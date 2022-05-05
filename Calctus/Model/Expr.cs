using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Shapoco.Calctus.Model.UnitSystem;

namespace Shapoco.Calctus.Model {
    /// <summary>式</summary>
    abstract class Expr {
        public Token Token { get; private set; }
        public Expr(Token t = null) {
            this.Token = t;
        }

        /// <summary>この式を評価して値を返す</summary>
        public abstract Val Eval(EvalContext ctx);
    }

    /// <summary>演算子</summary>
    abstract class Op : Expr {
        public OpDef Method { get; private set; }
        public Op(OpDef s, Token t = null) : base(t) {
            this.Method = s;
        }
    }

    /// <summary>単項演算</summary>
    class UnaryOp : Op {
        public Expr A { get; private set; }

        public UnaryOp(Expr a, Token t) : base(OpDef.Match(OpType.Unary, t), t) {
            this.A = a;
        }

        public override Val Eval(EvalContext e) {
            if (Method == OpDef.Plus) return A.Eval(e);
            if (Method == OpDef.ArithInv) return A.Eval(e).ArithInv(e);
            if (Method == OpDef.BitNot) return A.Eval(e).BitNot(e);
            throw new NotImplementedException();
        }

        public override string ToString() => "(" + this.Method.Symbol + A.ToString() + ")";
    }

    /// <summary>二項演算</summary>
    class BinaryOp : Op {
        public Expr A { get; private set; }
        public Expr B { get; private set; }
        
        public BinaryOp(Token t, Expr a, Expr b) : base(OpDef.Match(OpType.Binary, t), t) {
            this.A = a;
            this.B = b;
        }

        public override Val Eval(EvalContext e) {
            if (Method == OpDef.Assign) {
                if (A is VarRef aRef) {
                    var val = B.Eval(e);
                    e.Ref(aRef.RefName, allowCreate: true).Value = val;
                    return val;
                }
                else {
                    throw new EvalError(e, A.Token, "left hand of " + Token + " must be variant");
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
            if (Method == OpDef.BitAnd) return A.Eval(e).BitAnd(e, B.Eval(e));
            if (Method == OpDef.BitXor) return A.Eval(e).BitXor(e, B.Eval(e));
            if (Method == OpDef.BitOr) return A.Eval(e).BitOr(e, B.Eval(e));
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "(" + A.ToString() + " " + this.Method.Symbol + " " + B.ToString() + ")";
        }
    }

    /// <summary>リテラル</summary>
    abstract class Literal : Expr {
        public readonly Val Value;
        public Literal(Val v, Token t = null) : base(t) {
            this.Value = v;
        }
    }

    /// <summary>数値リテラル</summary>
    class Number : Literal {
        public Number(Token t) : base(((NumberTokenHint)t.Hint).Value, t) { }

        public override Val Eval(EvalContext ctx) => Value;
        public override string ToString() => Value.ToString();
    }

    class VarRef : Expr {
        public Token RefName => Token;
        public VarRef(Token name) : base(name) { }
        public override Val Eval(EvalContext ctx) => ctx.Ref(RefName).Value;
    }

    class FuncRef : Expr {
        public Token Name => Token;
        public readonly Expr[] Args;
        
        public FuncRef(Token name, Expr[] args) : base(name) {
            this.Args = args;
        }

        public override Val Eval(EvalContext ctx) {
            var f = FuncDef.Match(Name, Args.Length);
            var args = Args.Select(p => p.Eval(ctx)).ToArray();
            return f.Call(ctx, args);
        }
    }

    /// <summary>単位の付加</summary>
    class UnitifyOp : Expr {
        public Expr A { get; private set; }
        public Expr B { get; private set; }

        public UnitifyOp(Token tok, Expr a, Expr b) : base(tok) {
            this.A = a;
            this.B = b;
        }

        public override Val Eval(EvalContext e) => A.Eval(e).Mul(e, B.Eval(e));
        public override string ToString() => "(" + A.ToString() + "[" + B.ToString() + "])";
    }

    /// <summary>単位同士の乗算</summary>
    class UnitMultOp : Expr {
        public Expr A { get; private set; }
        public Expr B { get; private set; }

        public UnitMultOp(Token tok, Expr a, Expr b) : base(tok) {
            this.A = a;
            this.B = b;
        }

        public override Val Eval(EvalContext e) => A.Eval(e).Mul(e, B.Eval(e));
        public override string ToString() => A.ToString() + "*" + B.ToString();
    }

    /// <summary>単位同士の除算</summary>
    class UnitDivOp : Expr {
        public Expr A { get; private set; }
        public Expr B { get; private set; }

        public UnitDivOp(Token tok, Expr a, Expr b) : base(tok) {
            this.A = a;
            this.B = b;
        }

        public override Val Eval(EvalContext e) => A.Eval(e).Div(e, B.Eval(e));
        public override string ToString() => A.ToString() + "/" + B.ToString();
    }

    class UnitRef : Expr {
        public Token Name => Token;
        public readonly Unit Unit;

        public UnitRef(Token tok) : base(tok) {
            this.Unit = UnitFactory.Default.Solve(tok.Text);
        }

        public override Val Eval(EvalContext e) => new RealVal(Unit.UnscaleValue(e, 1), null, Unit);
        public override string ToString() => Unit.ToString();
    }

}
