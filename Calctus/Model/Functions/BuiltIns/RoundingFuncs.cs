using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Maths;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    class RoundingFuncs : BuiltInFuncCategory {
        private static RoundingFuncs _instance = null;
        public static RoundingFuncs Instance => _instance != null ? _instance : _instance = new RoundingFuncs();
        private RoundingFuncs() { }

        public readonly BuiltInFuncDef floor = new BuiltInFuncDef("floor(*x)",
            "Largest integral value less than or equal to `x`",
            (e, a) => Math.Floor(a[0].AsDecimal).ToIntVal());

        public readonly BuiltInFuncDef ceil = new BuiltInFuncDef("ceil(*x)",
            "Smallest integral value greater than or equal to `x`",
            (e, a) => Math.Ceiling(a[0].AsDecimal).ToIntVal());

        public readonly BuiltInFuncDef trunc = new BuiltInFuncDef("trunc(*x)",
            "Integral part of `x`",
            (e, a) => Math.Truncate(a[0].AsDecimal).ToIntVal());

        public readonly BuiltInFuncDef round = new BuiltInFuncDef("round(*x)",
            "Nearest integer to `x`",
            (e, a) => Math.Round(a[0].AsDecimal).ToIntVal());
    }
}
