using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Values {
    interface IBitArrayVal {
        Val LogicShiftL(EvalContext ctx, Val b);
        Val LogicShiftR(EvalContext ctx, Val b);
        Val ArithShiftL(EvalContext ctx, Val b);
        Val ArithShiftR(EvalContext ctx, Val b);
        Val BitAnd(EvalContext ctx, Val b);
        Val BitXor(EvalContext ctx, Val b);
        Val BitOr(EvalContext ctx, Val b);
    }
}
