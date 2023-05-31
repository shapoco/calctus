using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Expressions {
    class VarRef : Expr {
        public Token RefName => Token;
        public VarRef(Token name) : base(name) { }
        protected override Val OnEval(EvalContext ctx) => ctx.Ref(RefName, false).Value;
    }
}
