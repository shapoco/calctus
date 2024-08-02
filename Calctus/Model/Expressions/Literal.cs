using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Parsers;

namespace Shapoco.Calctus.Model.Expressions {
    abstract class Literal : Expr {
        public readonly Val Value;
        public Literal(Val v, Token t = null) : base(t) {
            this.Value = v;
        }

        public override bool CausesValueChange() => false;
    }
}
