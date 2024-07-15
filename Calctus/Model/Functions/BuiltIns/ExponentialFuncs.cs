using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Mathematics;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    class ExponentialFuncs : BuiltInFuncCategory {
        private static ExponentialFuncs _instance = null;
        public static ExponentialFuncs Instance => _instance != null ? _instance : _instance = new ExponentialFuncs();
        private ExponentialFuncs() { }

        public readonly BuiltInFuncDef pow = new BuiltInFuncDef("pow(*x@, y)",
            "`y` power of `x`",
            FuncDef.ArgToReal((e, a) => RMath.Pow(a[0], a[1])));

        public readonly BuiltInFuncDef exp = new BuiltInFuncDef("exp(*x)",
            "Exponential of `x`",
            FuncDef.ArgToReal((e, a) => RMath.Exp(a[0])));

        public readonly BuiltInFuncDef sqrt = new BuiltInFuncDef("sqrt(*x@)",
            "Square root of `x`",
            FuncDef.ArgToReal((e, a) => RMath.Sqrt(a[0])));

        public readonly BuiltInFuncDef log = new BuiltInFuncDef("log(*x)",
            "Logarithm of `x`",
            FuncDef.ArgToReal((e, a) => RMath.Log(a[0])));

        public readonly BuiltInFuncDef log2 = new BuiltInFuncDef("log2(*x)",
            "Binary logarithm of `x`",
            FuncDef.ArgToReal((e, a) => RMath.Log2(a[0], e.EvalSettings.AccuracyPriority)));

        public readonly BuiltInFuncDef log10 = new BuiltInFuncDef("log10(*x)",
            "Common logarithm of `x`",
            FuncDef.ArgToReal((e, a) => RMath.Log10(a[0])));

        public readonly BuiltInFuncDef clog2 = new BuiltInFuncDef("clog2(*x)",
            "Ceiling of binary logarithm of `x`",
            (e, a) => RMath.Ceiling(RMath.Log2(a[0].AsReal, e.EvalSettings.AccuracyPriority)).ToIntVal());

        public readonly BuiltInFuncDef clog10 = new BuiltInFuncDef("clog10(*x)",
            "Ceiling of common logarithm of `x`",
            (e, a) => RMath.Ceiling(RMath.Log10(a[0].AsReal)).ToIntVal());
    }
}

