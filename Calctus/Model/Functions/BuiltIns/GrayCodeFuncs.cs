using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Mathematics;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    static class GrayCodeFuncs {
        public static readonly BuiltInFuncDef toGray = new BuiltInFuncDef("toGray(*x)", (e, a) => new RealVal(LMath.ToGray(a[0].AsLong), a[0].FormatHint), "Converts the value from binary to gray-code.");
        public static readonly BuiltInFuncDef fromGray = new BuiltInFuncDef("fromGray(*x)", (e, a) => new RealVal(LMath.FromGray(a[0].AsLong), a[0].FormatHint), "Converts the value from gray-code to binary.");
    }
}
