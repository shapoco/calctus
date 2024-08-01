using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Values;

namespace Shapoco.Calctus.Model.Evaluations {
    class ValComparer : IComparer<Val> {
        public readonly EvalContext Context;

        public ValComparer(EvalContext e) {
            Context = e;
        }

        public int Compare(Val x, Val y) {
            if (x.Equals(Context, y)) return 0;
            else if (x.Grater(Context, y)) return 1;
            else return -1;
        }
    }
}
