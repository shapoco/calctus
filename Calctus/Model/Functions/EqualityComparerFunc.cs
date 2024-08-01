using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Values;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Functions {
    class EqualityComparerFunc : IEqualityComparer<Val> {
        public readonly EvalContext Context;
        public readonly FuncDef Func;

        public EqualityComparerFunc(EvalContext e, FuncDef f) {
            Context = e;
            Func = f;
        }

        public bool Equals(Val x, Val y) => Func.Call(Context, x, y).ToBool(); 

        public int GetHashCode(Val obj) {
            // ハッシュコードが違った時点で値が異なると判定されてしまい Equals() が使われないので常に 0 を返す
            return 0;
        }
    }
}
