﻿using System.Linq;
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

        protected override Val OnEval(EvalContext e) {
            // ユーザ定義関数で一致するものを探す
            var args = Args.Select(p => p.Eval(e)).ToArray();
            foreach (var fDef in e.EnumUserFuncs()) {
                if (fDef.Match(Name.Text, args)) {
                    return fDef.Call(e, args);
                }
            }

            // ユーザ定義関数に一致しなければ組み込み関数を探す
            {
                var f = FuncDef.Match(Name, args, e.Settings.AllowExternalFunctions);
                return f.Call(e, args);
            }
        }
    }
}
