using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Values;
using Shapoco.Calctus.Model.Maths;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Evaluations;
using Shapoco.Calctus.Model.Formats;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    class CastFuncs : BuiltInFuncCategory {
        private static CastFuncs _instance = null;
        public static CastFuncs Instance => _instance != null ? _instance : _instance = new CastFuncs();
        private CastFuncs() { }

        public readonly BuiltInFuncDef real = new BuiltInFuncDef("real(*x)",
            "Converts the `x` to a real number.",
            (e, a) => a[0].AsRealVal());

        public readonly BuiltInFuncDef rat_1 = new BuiltInFuncDef("rat(*x)",
            "Rational fraction approximation of `x`.",
            (e, a) => FracMath.FindFrac(a[0].AsDecimal).ToVal());

        public readonly BuiltInFuncDef rat_2 = new BuiltInFuncDef("rat(*x, max)",
            "Rational fraction approximation of `x`.",
            (e, a) => FracMath.FindFrac(a[0].AsDecimal, a[1].AsDecimal, a[1].AsDecimal).ToVal());

        public readonly BuiltInFuncDef array = new BuiltInFuncDef("array(x)",
            "Converts value `x` to an list.",
            (e, a) => a[0].ToCollectionVal().ToListVal(),
            new FuncTest("[1,2,3]", "[1,2,3]"),
            new FuncTest("\"abc\"", "['a', 'b', 'c']"));

        public readonly BuiltInFuncDef str = new BuiltInFuncDef("str(x)",
            "Converts `x` to a string.",
            (e, a) => a[0].ToStringForValue(e).ToVal());
    
    }
}
