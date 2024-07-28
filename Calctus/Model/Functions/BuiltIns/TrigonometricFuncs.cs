using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Maths;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    class TrigonometricFuncs : BuiltInFuncCategory {
        private static TrigonometricFuncs _instance = null;
        public static TrigonometricFuncs Instance => _instance != null ? _instance : _instance = new TrigonometricFuncs();
        private TrigonometricFuncs() { }

        public readonly BuiltInFuncDef sin = new BuiltInFuncDef("sin(*x)", "Sine", FuncDef.ArgToDecimal((e, a) => DMath.Sin(a[0])));
        public readonly BuiltInFuncDef cos = new BuiltInFuncDef("cos(*x)", "Cosine", FuncDef.ArgToDecimal((e, a) => DMath.Cos(a[0])));
        public readonly BuiltInFuncDef tan = new BuiltInFuncDef("tan(*x)", "Tangent", FuncDef.ArgToDecimal((e, a) => DMath.Tan(a[0])));
        public readonly BuiltInFuncDef asin = new BuiltInFuncDef("asin(*x)", "Arcsine", FuncDef.ArgToDecimal((e, a) => DMath.Asin(a[0])));
        public readonly BuiltInFuncDef acos = new BuiltInFuncDef("acos(*x)", "Arccosine", FuncDef.ArgToDecimal((e, a) => DMath.Acos(a[0])));
        public readonly BuiltInFuncDef atan = new BuiltInFuncDef("atan(*x)", "Arctangent", FuncDef.ArgToDecimal((e, a) => DMath.Atan(a[0])));
        public readonly BuiltInFuncDef atan2 = new BuiltInFuncDef("atan2(a, b)", "Arctangent of a / b", FuncDef.ArgToDecimal((e, a) => DMath.Atan2(a[0], a[1])));
        public readonly BuiltInFuncDef sinh = new BuiltInFuncDef("sinh(*x)", "Hyperbolic sine", FuncDef.ArgToDecimal((e, a) => DMath.Sinh(a[0])));
        public readonly BuiltInFuncDef cosh = new BuiltInFuncDef("cosh(*x)", "Hyperbolic cosine", FuncDef.ArgToDecimal((e, a) => DMath.Cosh(a[0])));
        public readonly BuiltInFuncDef tanh = new BuiltInFuncDef("tanh(*x)", "Hyperbolic tangent", FuncDef.ArgToDecimal((e, a) => DMath.Tanh(a[0])));
    }
}
