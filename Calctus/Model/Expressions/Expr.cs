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

        /// <summary>
        /// 式の評価が式の視覚的な変化を生じさせるか否か
        /// </summary>
        public abstract bool CausesValueChange();

        public Val Eval(EvalContext e) {
            try {
                return OnEval(e);
            }
            catch (EvalError ex) {
                throw ex;
            }
            catch (Exception ex) {
                throw new EvalError(e, Token, ex.Message);
            }
        }

        protected abstract Val OnEval(EvalContext e);
    }
}
