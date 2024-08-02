using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Expressions {
    /// <summary>真偽値リテラル</summary>
    class BoolLiteral : Literal {
        public BoolLiteral(Token t) : base(BoolVal.FromBool(bool.Parse(t.Text)), t) { }

        protected override Val OnEval(EvalContext ctx) => Value;
        public override string ToString() => Value.ToString();
    }
}
