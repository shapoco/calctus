using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;

namespace Shapoco.Calctus.Model.Evaluations {
    class ValComparer : IComparer<Val> {
        public readonly EvalContext Context;

        public ValComparer(EvalContext e) {
            Context = e;
        }

        public int Compare(Val x, Val y) {
            bool equal = x.Equals(Context, y).AsBool;
            bool grater = x.Grater(Context, y).AsBool;
            if (equal) return 0;
            else if (grater) return 1;
            else return -1;
        }
    }
}
