using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Functions {
    class EqualityComparerFunc : IEqualityComparer<Val> {
        public readonly EvalContext Context;
        public readonly FuncDef Func;

        public EqualityComparerFunc(EvalContext e, FuncDef f) {
            Context = e;
            Func = f;
        }

        public bool Equals(Val x, Val y) => Func.Call(Context, x, y).AsBool; 

        public int GetHashCode(Val obj) => obj.GetHashCode();
    }
}
