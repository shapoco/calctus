using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Values;

namespace Shapoco.Calctus.Model.Evaluations {
    class ValEqualityComparer : IEqualityComparer<Val> {
        public readonly EvalContext Context;

        public ValEqualityComparer(EvalContext e) {
            Context = e;
        }

        public bool Equals(Val x, Val y) => x.Equals(Context, y);

        public int GetHashCode(Val obj) => obj.GetHashCode();
    }
}
