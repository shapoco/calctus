using System.Linq;
using Shapoco.Calctus.Model.Values;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Expressions {
    class ArrayExpr : Expr {
        public Token Name => Token;
        public readonly Expr[] Elements;

        public ArrayExpr(Token openBracket, Expr[] elms) : base(openBracket) {
            this.Elements = elms;
        }

        public override bool CausesValueChange() => Elements.Any(p => p.CausesValueChange());

        protected override Val OnEval(EvalContext ctx) {
            return new ListVal(Elements.Select(p => p.Eval(ctx)).ToArray());
        }
    }
}
