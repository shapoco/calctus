using Shapoco.Calctus.Model.Parsers;

namespace Shapoco.Calctus.Model.Expressions {
    /// <summary>演算子</summary>
    abstract class OpExpr : Expr {
        public OpCodes OpCode { get; private set; }
        public OpExpr(OpCodes op, Token t = null) : base(t) {
            this.OpCode = op;
        }
    }
}
