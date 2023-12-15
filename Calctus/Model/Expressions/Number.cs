using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Expressions {
    /// <summary>数値リテラル</summary>
    class Number : Literal {
        public Number(Token t) : base(((NumberTokenHint)t.Hint).Value, t) { }
        public Number(Val val) : base(val) { }

        protected override Val OnEval(EvalContext ctx) => Value;
        public override string ToString() => Value.ToString();
    }
}
