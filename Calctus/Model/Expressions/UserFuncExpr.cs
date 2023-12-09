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
        public readonly ArgDef[] ArgDefs;
        public readonly int VectorizableArgIndex;
        public readonly Expr Body;

        public UserFuncExpr(Token keyword, Token name, ArgDef[] args, int vecArgIndex, Expr body) : base(keyword) {
            Name = name;
            ArgDefs = args;
            VectorizableArgIndex = vecArgIndex;
            Body = body;
            for (int i = 0; i < args.Length - 1; i++) {
                for (int j = i + 1; j < args.Length; j++) {
                    if (args[i].Name.Text == args[j].Name.Text) {
                        throw new ParserError(args[j].Name, "Duplicate argument name");
                    }
                }
            }
        }

        protected override Val OnEval(EvalContext e) {
            e.Ref(Name.Text, true).Value = new FuncVal(new UserFuncDef(Name.Text, ArgDefs, VectorizableArgIndex, Body));
            return new BoolVal(true);
        }
    }
}
