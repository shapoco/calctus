﻿using System;
using Shapoco.Calctus.Model.Values;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Expressions {
    class PartRefExpr : Expr {
        public Token Name => Token;
        public readonly Expr Target;
        public readonly Expr IndexFrom;
        public readonly Expr IndexTo;
        public bool IsSingleIndex => IndexTo == null;

        public PartRefExpr(Token startBracket, Expr target, Expr index) : this(startBracket, target, index, null) { }

        public PartRefExpr(Token startBracket, Expr target, Expr from, Expr to) : base(startBracket) {
            Target = target;
            IndexFrom = from;
            IndexTo = to;
        }

        public override bool CausesValueChange() => true;

        protected override Val OnEval(EvalContext ctx) {
            var from = IndexFrom.Eval(ctx).AsInt;
            var to = IndexTo == null ? from : IndexTo.Eval(ctx).AsInt;
            var obj = Target.Eval(ctx);
            if (obj is ListVal array) {
                if (from < 0) from = array.Length + from;
                if (to < 0) to = array.Length + to;
                if (from == to) {
                    return array[from];
                }
                else {
                    return array.Slice(from, to);
                }
            }
            else if (obj is StrVal str) {
                if (from < 0) from = str.Length + from;
                if (to < 0) to = str.Length + to;
                if (from == to) {
                    return str.Raw[from].ToCharVal();
                }
                else {
                    return new StrVal(str.Raw.Substring(from, to - from));
                }
            }
            else {
                if (from < to) throw new ArgumentOutOfRangeException();
                if (from < 0 || 63 < from) throw new ArgumentOutOfRangeException();
                if (to < 0 || 63 < to) throw new ArgumentOutOfRangeException();
                var val = obj.AsLong;
                val >>= to;
                int w = from - to + 1;
                if (w < 64) {
                    val &= (1L << w) - 1L;
                }
                return new RealVal(val, obj.FormatFlags);
            }
        }
    }
}