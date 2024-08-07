using System;
using Shapoco.Calctus.Model.Values;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Expressions {
    class PartRefExpr : Expr {
        public Token Name => Token;
        public readonly Expr Target;
        public readonly Expr IndexLeft;
        public readonly Expr IndexRight;
        public bool IsSingleIndex => IndexRight == null;

        public PartRefExpr(Token startBracket, Expr target, Expr index) : this(startBracket, target, index, null) { }

        public PartRefExpr(Token startBracket, Expr target, Expr iLeft, Expr iRight) : base(startBracket) {
            Target = target;
            IndexLeft = iLeft;
            IndexRight = iRight;
        }

        public override bool CausesValueChange() => true;

        protected override Val OnEval(EvalContext e) {
            bool single = IsSingleIndex;
            var iLeft = IndexLeft.Eval(e).AsInt;
            var iRight = IndexRight == null ? iLeft : IndexRight.Eval(e).AsInt;
            var obj = Target.Eval(e);
            if (obj is ICollectionVal collection) {
                if (single) {
                    return collection.GetElement(e, iLeft);
                }
                else {
                    return collection.GetSlice(e, iLeft, iRight);
                }
            }
            else {
                if (iLeft < iRight) throw new ArgumentOutOfRangeException();
                if (iLeft < 0 || 63 < iLeft) throw new ArgumentOutOfRangeException();
                if (iRight < 0 || 63 < iRight) throw new ArgumentOutOfRangeException();
                var val = obj.AsLong;
                val >>= iRight;
                int w = iLeft - iRight + 1;
                if (w < 64) {
                    val &= (1L << w) - 1L;
                }
                return new RealVal(val, obj.FormatHint);
            }
        }
    }
}
