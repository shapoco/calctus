using System;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Expressions {
    class PartRef : Expr {
        public Token Name => Token;
        public readonly Expr Target;
        public readonly Expr IndexFrom;
        public readonly Expr IndexTo;

        public PartRef(Token startBracket, Expr target, Expr from, Expr to) : base(startBracket) {
            Target = target;
            IndexFrom = from;
            IndexTo = to;
        }

        protected override Val OnEval(EvalContext ctx) {
            var from = IndexFrom.Eval(ctx).AsInt;
            var to = IndexTo.Eval(ctx).AsInt;
            var obj = Target.Eval(ctx);
            if (obj is ArrayVal array) {
                if (from == to) {
                    return array[from];
                }
                else {
                    return array.Slice(from, to);
                }
            }
            else {
                if (from < to) throw new ArgumentOutOfRangeException();
                if (from < 0 || 63 <= from) throw new ArgumentOutOfRangeException();
                if (to < 0 || 63 <= to) throw new ArgumentOutOfRangeException();
                var val = obj.AsLong;
                val >>= to;
                val &= (1L << (from - to + 1)) - 1L;
                return new RealVal(val, obj.FormatHint);
            }
        }
    }
}
