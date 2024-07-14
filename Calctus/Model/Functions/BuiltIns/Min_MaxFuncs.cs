using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Mathematics;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    static class Min_MaxFuncs {
        public static readonly BuiltInFuncDef max = new BuiltInFuncDef("max(array...)", (e, a) => {
            return new RealVal(a.Max(p => p.AsReal), a[0].FormatHint);
        }, "Maximum value of elements of the `array`."); 
        
        public static readonly BuiltInFuncDef min = new BuiltInFuncDef("min(array...)", (e, a) => {
            return new RealVal(a.Min(p => p.AsReal), a[0].FormatHint);
        }, "Minimum value of elements of the `array`.");

        public static readonly BuiltInFuncDef clip = new BuiltInFuncDef("clip(a, b, *x)", (e, a) => {
            return new RealVal(RMath.Clip(a[0].AsReal, a[1].AsReal, a[2].AsReal), a[0].FormatHint);
        }, "Clips `x` to a range from `a` to `b`. Same as `max(a, min(b, x))`.");
    }
}
