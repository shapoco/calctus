using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Values {
    interface IScalarVal {
        // 単項演算
        Val UnaryPlus(EvalContext e);
        Val ArithInv(EvalContext e);
        Val BitNot(EvalContext e);
        Val LogicNot(EvalContext e);

        // 算術演算
        Val Add(EvalContext e, Val b);
        Val Sub(EvalContext e, Val b);
        Val Mul(EvalContext e, Val b);
        Val Div(EvalContext e, Val b);
        Val IDiv(EvalContext e, Val b);
        Val Mod(EvalContext e, Val b);
    }
}
