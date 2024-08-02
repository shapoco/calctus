using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Values {
    interface ICollectionVal {
        Array ToRawArray();
        Val[] ToValArray();
        ListVal ToListVal();
        int Length { get; }
        Val GetElement(EvalContext e, int index);
        Val GetSlice(EvalContext e, int from, int to);
        Val SetSelement(EvalContext e, int index, Val val);
        Val SetRange(EvalContext e, int from, int to, Val val);
    }
}
