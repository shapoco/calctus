using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Evaluations;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Types;

namespace Shapoco.Calctus.Model.Expressions {
    internal class UserFuncExpr : Expr {
        public readonly Token Name;
        public readonly Token[] Args;
        public readonly Expr Body;

        public UserFuncExpr(Token name, Token[] args, Expr body) {
            Name = name;
            Args = args;
            Body = body;
        }

        protected override Val OnEval(EvalContext e) {
            var args = Args.Select(p => p.Text).ToArray();
            e.Ref(Name.Text, true).Value = new FuncVal(new UserFuncDef(Name.Text, args, Body));
            return new BoolVal(true);
        }
    }
}
