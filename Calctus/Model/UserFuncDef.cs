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
        private string[] _argNames;

        public UserFuncDef(string name, string[] args, Expr expr) : base(name, args.Length, null, "User-defined function") {
            _expr = expr;
            _argNames = args;
            Call = (e, a) => Exec(e, a);
        }

        public Val Exec(EvalContext e, Val[] args) {
            var scope = new EvalContext(e);
            scope.Undef(Name, true); // 再帰呼び出しを防ぐために自分自身の定義をスコープから抹消する
            for(int i = 0; i < _argNames.Length; i++) {
                scope.Ref(_argNames[i], true).Value = args[i];
            }
            return _expr.Eval(scope);
        }
    }
}
