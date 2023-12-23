using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Mathematics;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    static class ExponentialFuncs {
        public static readonly BuiltInFuncDef pow = new BuiltInFuncDef("pow(*x, y)", (e, a) => new RealVal(RMath.Pow(a[0].AsReal, a[1].AsReal), a[0].FormatHint), "Power");
        public static readonly BuiltInFuncDef exp = new BuiltInFuncDef("exp(*x)", (e, a) => new RealVal(RMath.Exp(a[0].AsReal)), "Exponential");
        public static readonly BuiltInFuncDef sqrt = new BuiltInFuncDef("sqrt(*x)", (e, a) => new RealVal(RMath.Sqrt(a[0].AsReal)), "Square root");
        public static readonly BuiltInFuncDef log = new BuiltInFuncDef("log(*x)", (e, a) => new RealVal(RMath.Log(a[0].AsReal)), "Logarithm");
        public static readonly BuiltInFuncDef log2 = new BuiltInFuncDef("log2(*x)", (e, a) => new RealVal(RMath.Log2(a[0].AsReal, e.EvalSettings.AccuracyPriority)), "Binary logarithm");
        public static readonly BuiltInFuncDef log10 = new BuiltInFuncDef("log10(*x)", (e, a) => new RealVal(RMath.Log10(a[0].AsReal)), "Common logarithm");
        public static readonly BuiltInFuncDef clog2 = new BuiltInFuncDef("clog2(*x)", (e, a) => new RealVal(RMath.Ceiling(RMath.Log2(a[0].AsReal, e.EvalSettings.AccuracyPriority))).FormatInt(), "Ceiling of binary logarithm");
        public static readonly BuiltInFuncDef clog10 = new BuiltInFuncDef("clog10(*x)", (e, a) => new RealVal(RMath.Ceiling(RMath.Log10(a[0].AsReal))).FormatInt(), "Ceiling of common logarithm");
    }
}
