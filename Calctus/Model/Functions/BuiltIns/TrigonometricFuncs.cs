using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Mathematics;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    static class TrigonometricFuncs {
        public static readonly BuiltInFuncDef sin = new BuiltInFuncDef("sin(*x)", (e, a) => new RealVal(RMath.Sin(a[0].AsReal)), "Sine");
        public static readonly BuiltInFuncDef cos = new BuiltInFuncDef("cos(*x)", (e, a) => new RealVal(RMath.Cos(a[0].AsReal)), "Cosine");
        public static readonly BuiltInFuncDef tan = new BuiltInFuncDef("tan(*x)", (e, a) => new RealVal(RMath.Tan(a[0].AsReal)), "Tangent");
        public static readonly BuiltInFuncDef asin = new BuiltInFuncDef("asin(*x)", (e, a) => new RealVal(RMath.Asin(a[0].AsReal)), "Arcsine");
        public static readonly BuiltInFuncDef acos = new BuiltInFuncDef("acos(*x)", (e, a) => new RealVal(RMath.Acos(a[0].AsReal)), "Arccosine");
        public static readonly BuiltInFuncDef atan = new BuiltInFuncDef("atan(*x)", (e, a) => new RealVal(RMath.Atan(a[0].AsReal)), "Arctangent");
        public static readonly BuiltInFuncDef atan2 = new BuiltInFuncDef("atan2(a, b)", (e, a) => new RealVal(RMath.Atan2(a[0].AsReal, a[1].AsReal)), "Arctangent of a / b");
        public static readonly BuiltInFuncDef sinh = new BuiltInFuncDef("sinh(*x)", (e, a) => new RealVal(RMath.Sinh(a[0].AsReal)), "Hyperbolic sine");
        public static readonly BuiltInFuncDef cosh = new BuiltInFuncDef("cosh(*x)", (e, a) => new RealVal(RMath.Cosh(a[0].AsReal)), "Hyperbolic cosine");
        public static readonly BuiltInFuncDef tanh = new BuiltInFuncDef("tanh(*x)", (e, a) => new RealVal(RMath.Tanh(a[0].AsReal)), "Hyperbolic tangent");
    }
}
