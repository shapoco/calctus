using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Evaluations;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Functions;

namespace Shapoco.Calctus.Model.Expressions {
    internal class DefExpr : Expr {
        public readonly Token Name;
        public readonly ArgDefList Args;
        public readonly Expr Body;

        public DefExpr(Token keyword, Token name, ArgDefList args, Expr body) : base(keyword) {
            Name = name;
            Args = args;
            Body = body;
            for (int i = 0; i < args.Count - 1; i++) {
                for (int j = i + 1; j < args.Count; j++) {
                    if (args[i].Name.Text == args[j].Name.Text) {
                        throw new ParserError(args[j].Name, "Duplicate argument name");
                    }
                }
            }
        }

        public override bool CausesValueChange() => false;

        protected override Val OnEval(EvalContext e) {
            e.Ref(Name, true).Value = new FuncVal(new UserFuncDef(Name, Args, Body));
            return NullVal.Instance;
        }
    }
}
