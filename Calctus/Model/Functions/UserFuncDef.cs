using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Shapoco.Calctus.Model.Values;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Evaluations;
using Shapoco.Calctus.Model.Expressions;
using Shapoco.Calctus.Model;

namespace Shapoco.Calctus.Model.Functions {
    class UserFuncDef : FuncDef {
        private Expr _expr;

        public UserFuncDef(Token name, ArgDefList args, Expr expr)
            : base(name, args, "User-defined function") {
            _expr = expr;
        }

        protected override Val OnCall(EvalContext e, Val[] args) {
            var scope = new EvalContext(e);
            for (int i = 0; i < Args.Count; i++) {
                scope.Ref(Args[i].Name, true).Value = args[i];
            }
            return _expr.Eval(scope);
        }
    }
}
