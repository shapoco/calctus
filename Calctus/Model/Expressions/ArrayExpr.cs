using System.Linq;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Expressions {
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
}
