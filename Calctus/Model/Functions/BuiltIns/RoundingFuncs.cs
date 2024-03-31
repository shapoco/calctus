using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Mathematics;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    static class RoundingFuncs {
        public static readonly BuiltInFuncDef floor = new BuiltInFuncDef("floor(*x)", (e, a) => new RealVal(RMath.Floor(a[0].AsReal), a[0].FormatHint).FormatInt(), "Largest integral value less than or equal to `x`");
        public static readonly BuiltInFuncDef ceil = new BuiltInFuncDef("ceil(*x)", (e, a) => new RealVal(RMath.Ceiling(a[0].AsReal), a[0].FormatHint).FormatInt(), "Smallest integral value greater than or equal to `x`");
        public static readonly BuiltInFuncDef trunc = new BuiltInFuncDef("trunc(*x)", (e, a) => new RealVal(RMath.Truncate(a[0].AsReal), a[0].FormatHint).FormatInt(), "Integral part of `x`");
        public static readonly BuiltInFuncDef round = new BuiltInFuncDef("round(*x)", (e, a) => new RealVal(RMath.Round(a[0].AsReal), a[0].FormatHint).FormatInt(), "Nearest integer to `x`");
    }
}
