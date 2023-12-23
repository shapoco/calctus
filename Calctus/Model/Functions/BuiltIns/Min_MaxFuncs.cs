using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    static class Min_MaxFuncs {
        public static readonly BuiltInFuncDef max = new BuiltInFuncDef("max(array...)", (e, a) => new RealVal(a.Max(p => p.AsReal), a[0].FormatHint), "Maximum value of elements of the `array`");
        public static readonly BuiltInFuncDef min = new BuiltInFuncDef("min(array...)", (e, a) => new RealVal(a.Min(p => p.AsReal), a[0].FormatHint), "Minimum value of elements of the `array`");
    }
}
