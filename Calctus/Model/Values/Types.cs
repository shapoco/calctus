using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Values {
    delegate TDest CastFunc<TSrc, TDest>(TSrc val, out bool degraded, object tag) where TSrc : Val where TDest : Val;
    delegate bool EqualFunc<TA, TB>(TA a, TB b, out bool degraded) where TA : Val where TB : Val;
    delegate int CompareFunc<TA, TB>(TA a, TB b, out bool degraded) where TA : Val where TB : Val;

    [Flags]
    enum ValCastRuleFlags {
        Degrading = (1 << 0),
    }
}
