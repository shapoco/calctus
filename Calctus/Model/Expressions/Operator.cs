using Shapoco.Calctus.Model.Parsers;

namespace Shapoco.Calctus.Model.Expressions {
    /// <summary>演算子</summary>
    abstract class Operator : Expr {
        public OpDef Method { get; private set; }
        public Operator(OpDef s, Token t = null) : base(t) {
            this.Method = s;
        }
    }
}
