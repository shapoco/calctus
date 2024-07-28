using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Maths;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    class GrayCodeFuncs : BuiltInFuncCategory {
        private static GrayCodeFuncs _instance = null;
        public static GrayCodeFuncs Instance => _instance != null ? _instance : _instance = new GrayCodeFuncs();
        private GrayCodeFuncs() { }

        public readonly BuiltInFuncDef toGray = new BuiltInFuncDef("toGray(*x@)",
            "Converts the value from binary to gray-code.",
            FuncDef.ArgToLong((e, a) => LMath.ToGray(a[0])));

        public readonly BuiltInFuncDef fromGray = new BuiltInFuncDef("fromGray(*x@)",
            "Converts the value from gray-code to binary.",
            FuncDef.ArgToLong((e, a) => LMath.FromGray(a[0])));
    }
}
