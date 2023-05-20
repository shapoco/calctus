using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model {
    /// <summary>式</summary>
    abstract class Expr {
        public Token Token { get; private set; }
        public Expr(Token t = null) {
            this.Token = t;
        }

        public Val Eval(EvalContext ctx) {
            try {
                return OnEval(ctx);
            }
            catch (EvalError ex) {
                throw ex;
            }
            catch (Exception ex) {
                throw new EvalError(ctx, Token, ex.Message);
            }
        }

        /// <summary>この式を評価して値を返す</summary>
        protected abstract Val OnEval(EvalContext ctx);
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

        protected override Val OnEval(EvalContext e) {
            if (Method == OpDef.Plus) return A.Eval(e);
            if (Method == OpDef.ArithInv) return A.Eval(e).ArithInv(e);
            if (Method == OpDef.BitNot) return A.Eval(e).BitNot(e);
            if (Method == OpDef.LogicNot) return A.Eval(e).LogicNot(e);
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

        protected override Val OnEval(EvalContext e) {
            if (Method == OpDef.Assign) {
                if (A is VarRef aRef) {
                    var val = B.Eval(e);
                    e.Ref(aRef.RefName, allowCreate: true).Value = val;
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

        protected override Val OnEval(EvalContext ctx) => Value;
        public override string ToString() => Value.ToString();
    }

    /// <summary>真偽値リテラル</summary>
    class BoolLiteral : Literal {
        public BoolLiteral(Token t) : base(new BoolVal(bool.Parse(t.Text)), t) { }

        protected override Val OnEval(EvalContext ctx) => Value;
        public override string ToString() => Value.ToString();
    }

    class VarRef : Expr {
        public Token RefName => Token;
        public VarRef(Token name) : base(name) { }
        protected override Val OnEval(EvalContext ctx) => ctx.Ref(RefName, false).Value;
    }

    class FuncRef : Expr {
        public Token Name => Token;
        public readonly Expr[] Args;
        
        public FuncRef(Token name, Expr[] args) : base(name) {
            this.Args = args;
        }

        protected override Val OnEval(EvalContext ctx) {
            var f = FuncDef.Match(Name, Args.Length, ctx.Settings.AllowExternalFunctions);
            var args = Args.Select(p => p.Eval(ctx)).ToArray();
            return f.Call(ctx, args);
        }
    }

    class ArrayExpr : Expr {
        public Token Name => Token;
        public readonly Expr[] Elements;

        public ArrayExpr(Token startBracket, Expr[] elms) : base(startBracket) {
            this.Elements = elms;
        }

        protected override Val OnEval(EvalContext ctx) {
            return new ArrayVal(Elements.Select(p => p.Eval(ctx)).ToArray());
        }
    }

    class ElemRef : Expr {
        public Token Name => Token;
        public readonly Expr Array;
        public readonly Expr Index;

        public ElemRef(Token startBracket, Expr array, Expr index) : base(startBracket) {
            Array = array;
            Index = index;
        }

        protected override Val OnEval(EvalContext ctx) {
            var idx = Index.Eval(ctx);
            var obj = Array.Eval(ctx);
            if (obj is ArrayVal array) {
                return array[idx.AsInt];
            }
            else {
                throw new InvalidOperationException();
            }
        }
    }
}
