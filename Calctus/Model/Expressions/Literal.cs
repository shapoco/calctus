using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Parsers;

namespace Shapoco.Calctus.Model.Expressions {
    /// <summary>リテラル</summary>
    abstract class Literal : Expr {
        public readonly Val Value;
        public Literal(Val v, Token t = null) : base(t) {
            this.Value = v;
        }
    }
}
