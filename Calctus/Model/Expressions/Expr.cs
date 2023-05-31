using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Expressions {
    /// <summary>式</summary>
    abstract class Expr {
        public static readonly EmptyExpr Empty = new EmptyExpr();

        public Token Token { get; private set; }
        public Expr(Token t = null) {
            this.Token = t;
        }

        public Val Eval(EvalContext ctx) {
            try {
                return OnEval(ctx);
            }
            catch (EvalError ex) {
                throw ex;
            }
            catch (Exception ex) {
                throw new EvalError(ctx, Token, ex.Message);
            }
        }

        /// <summary>この式を評価して値を返す</summary>
        protected abstract Val OnEval(EvalContext ctx);
    }
}
