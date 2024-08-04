using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Maths;
using Shapoco.Calctus.Model.Values;
using Shapoco.Calctus.Model.Maths;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    class Min_MaxFuncs : BuiltInFuncCategory {
        private static Min_MaxFuncs _instance = null;
        public static Min_MaxFuncs Instance => _instance != null ? _instance : _instance = new Min_MaxFuncs();
        private Min_MaxFuncs() { }

        public readonly BuiltInFuncDef max = new BuiltInFuncDef("max(array...)",
            "Maximum value of elements of the `array`.",
            (e, a) => a.ToDecimalArray().Max().ToVal(a[0].FormatFlags));

        public readonly BuiltInFuncDef min = new BuiltInFuncDef("min(array...)",
            "Minimum value of elements of the `array`.",
            (e, a) => a.ToDecimalArray().Min().ToVal(a[0].FormatFlags));

        public readonly BuiltInFuncDef clip = new BuiltInFuncDef("clip(a, b, *x@)",
            "Clips `x` to a range from `a` to `b`. Same as `max(a, min(b, x))`.",
            (e, a) => MathEx.Clip(a[0].AsDecimal, a[1].AsDecimal, a[2].AsDecimal).ToVal(a[2].FormatFlags));
    }
}
