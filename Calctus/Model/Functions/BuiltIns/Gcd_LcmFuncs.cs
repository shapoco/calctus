using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Maths;
using Shapoco.Calctus.Model.Values;
using Shapoco.Calctus.Model.Maths;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    class Gcd_LcmFuncs : BuiltInFuncCategory {
        private static Gcd_LcmFuncs _instance = null;
        public static Gcd_LcmFuncs Instance => _instance != null ? _instance : _instance = new Gcd_LcmFuncs();
        private Gcd_LcmFuncs() { }

        public readonly BuiltInFuncDef gcd = new BuiltInFuncDef("gcd(array...)",
            "Greatest common divisor of elements of the `array`.",
            (e, a) => MathEx.Gcd(a.ToDecimalArray()).ToVal(a[0].FormatFlags));

        public readonly BuiltInFuncDef lcm = new BuiltInFuncDef("lcm(array...)",
            "Least common multiple of elements of the `array`.",
            (e, a) => MathEx.Lcm(a.ToDecimalArray()).ToVal(a[0].FormatFlags));
    }
}
