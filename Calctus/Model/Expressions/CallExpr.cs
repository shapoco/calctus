using System.Linq;
using Shapoco.Calctus.Model.Values;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Evaluations;
using Shapoco.Calctus.Model.Functions;

namespace Shapoco.Calctus.Model.Expressions {
    class CallExpr : Expr {
        public Token Name => Token;
        public readonly Expr[] Args;

        public CallExpr(Token name, Expr[] args) : base(name) {
            this.Args = args;
        }

        public override bool CausesValueChange() => true;

        protected override Val OnEval(EvalContext e) {
            var args = Args.Select(p => p.Eval(e)).ToArray();
            if (e.SolveFunc(Name.Text, args, out FuncDef f, out string msg)) {
                return f.Call(e, args);
            }
            else {
                throw new CalctusError(msg);
            }
        }
    }
}
