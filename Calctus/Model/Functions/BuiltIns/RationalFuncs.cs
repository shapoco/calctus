using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Maths;
using Shapoco.Calctus.Model.Values;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    class RationalFuncs : BuiltInFuncCategory {
        private static RationalFuncs _instance = null;
        public static RationalFuncs Instance => _instance != null ? _instance : _instance = new RationalFuncs();
        private RationalFuncs() { }

        public readonly BuiltInFuncDef rat_1 = new BuiltInFuncDef("rat(*x)",
            "Rational fraction approximation of `x`.",
            (e, a) => a[0].CastTo(e, typeof(FracVal)));

        public readonly BuiltInFuncDef rat_2 = new BuiltInFuncDef("rat(*x, max)",
            "Rational fraction approximation of `x`.",
            (e, a) => {
                var x = a[0].CastTo<RealVal>(e).Raw;
                var max = a[1].CastTo<RealVal>(e).Raw;
                var ret = new FracVal(rational.FindFrac(x, out bool degr, max, max));
                if (degr) e.ReportDegrade();
                return ret;
            });

    }
}
