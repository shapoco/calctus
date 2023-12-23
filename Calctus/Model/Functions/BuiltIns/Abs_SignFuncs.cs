using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Mathematics;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    static class Abs_SignFuncs {
        public static readonly BuiltInFuncDef abs = new BuiltInFuncDef("abs(*x)", (e, a) => new RealVal(RMath.Abs(a[0].AsReal), a[0].FormatHint), "Absolute value of `x`");
        public static readonly BuiltInFuncDef sign = new BuiltInFuncDef("sign(*x)", (e, a) => new RealVal(RMath.Sign(a[0].AsReal)).FormatInt(), "Returns 1 for positives, -1 for negatives, 0 otherwise.");
        public static readonly BuiltInFuncDef mag = new BuiltInFuncDef("mag(x[]...)", (e, a) => new RealVal(RMath.Sqrt(a[0].AsRealArray.Sum(p => p * p)), a[0].FormatHint), "Magnitude of vector `x`.");
    }
}
