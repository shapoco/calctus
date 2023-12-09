using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Evaluations;
using Shapoco.Calctus.Model.Expressions;

namespace Shapoco.Calctus.Model {
    class UserFuncDef : FuncDef {
        private Expr _expr;

        public UserFuncDef(string name, ArgDef[] argDefs, int vecArgIndex, Expr expr)
            : base(name, argDefs, null, VariadicAragumentMode.None, vecArgIndex, "User-defined function") {
            _expr = expr;
            Method = (e, a) => Exec(e, a);
        }

        public Val Exec(EvalContext e, Val[] args) {
            var scope = new EvalContext(e);
            scope.Undef(Name, true); // 再帰呼び出しを防ぐために自分自身の定義をスコープから抹消する
            for(int i = 0; i < ArgDefs.Length; i++) {
                scope.Ref(ArgDefs[i].Name, true).Value = args[i];
            }
            return _expr.Eval(scope);
        }
    }
}
