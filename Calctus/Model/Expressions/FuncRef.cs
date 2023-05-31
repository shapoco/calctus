using System.Linq;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Expressions {
    class FuncRef : Expr {
        public Token Name => Token;
        public readonly Expr[] Args;
        
        public FuncRef(Token name, Expr[] args) : base(name) {
            this.Args = args;
        }

        protected override Val OnEval(EvalContext ctx) {
            var f = FuncDef.Match(Name, Args.Length, ctx.Settings.AllowExternalFunctions);
            var args = Args.Select(p => p.Eval(ctx)).ToArray();
            return f.Call(ctx, args);
        }
    }
}
