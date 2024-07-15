using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Mathematics;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    class TrigonometricFuncs : BuiltInFuncCategory {
        private static TrigonometricFuncs _instance = null;
        public static TrigonometricFuncs Instance => _instance != null ? _instance : _instance = new TrigonometricFuncs();
        private TrigonometricFuncs() { }

        public readonly BuiltInFuncDef sin = new BuiltInFuncDef("sin(*x)", "Sine", FuncDef.ArgToReal((e, a) => RMath.Sin(a[0])));
        public readonly BuiltInFuncDef cos = new BuiltInFuncDef("cos(*x)", "Cosine", FuncDef.ArgToReal((e, a) => RMath.Cos(a[0])));
        public readonly BuiltInFuncDef tan = new BuiltInFuncDef("tan(*x)", "Tangent", FuncDef.ArgToReal((e, a) => RMath.Tan(a[0])));
        public readonly BuiltInFuncDef asin = new BuiltInFuncDef("asin(*x)", "Arcsine", FuncDef.ArgToReal((e, a) => RMath.Asin(a[0])));
        public readonly BuiltInFuncDef acos = new BuiltInFuncDef("acos(*x)", "Arccosine", FuncDef.ArgToReal((e, a) => RMath.Acos(a[0])));
        public readonly BuiltInFuncDef atan = new BuiltInFuncDef("atan(*x)", "Arctangent", FuncDef.ArgToReal((e, a) => RMath.Atan(a[0])));
        public readonly BuiltInFuncDef atan2 = new BuiltInFuncDef("atan2(a, b)", "Arctangent of a / b", FuncDef.ArgToReal((e, a) => RMath.Atan2(a[0], a[1])));
        public readonly BuiltInFuncDef sinh = new BuiltInFuncDef("sinh(*x)", "Hyperbolic sine", FuncDef.ArgToReal((e, a) => RMath.Sinh(a[0])));
        public readonly BuiltInFuncDef cosh = new BuiltInFuncDef("cosh(*x)", "Hyperbolic cosine", FuncDef.ArgToReal((e, a) => RMath.Cosh(a[0])));
        public readonly BuiltInFuncDef tanh = new BuiltInFuncDef("tanh(*x)", "Hyperbolic tangent", FuncDef.ArgToReal((e, a) => RMath.Tanh(a[0])));
    }
}
