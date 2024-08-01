using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Values;
using Shapoco.Calctus.Model.Maths;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    class Absolute_SignFuncs : BuiltInFuncCategory {
        private static Absolute_SignFuncs _instance = null;
        public static Absolute_SignFuncs Instance => _instance != null ? _instance : _instance = new Absolute_SignFuncs();
        private Absolute_SignFuncs() { }

        public readonly BuiltInFuncDef abs = new BuiltInFuncDef("abs(*x@)",
            "Absolute value of `x`",
            FuncDef.ArgToDecimal((e, a) => {
                return Math.Abs(a[0]);
            }),
            new FuncTest("-12.34", "12.34"),
            new FuncTest("0", "0"),
            new FuncTest("56.78", "56.78"));

        public readonly BuiltInFuncDef sign = new BuiltInFuncDef("sign(*x)",
            "Returns 1 for positives, -1 for negatives, 0 otherwise.",
            FuncDef.ArgToDecimal((e, a) => {
                return Math.Sign(a[0]);
            }),
            new FuncTest("-12.34", "-1"),
            new FuncTest("0", "0"),
            new FuncTest("56.78", "1"));

        public readonly BuiltInFuncDef mag = new BuiltInFuncDef("mag(x[]...)",
            "Magnitude of vector `x`",
            (e, a) => {
                var arrayVal = (ListVal)a[0];
                var fmt = arrayVal.Length > 0 ? arrayVal[0].FormatFlags : Formats.FormatFlags.Default;
                return MathEx.Sqrt(arrayVal.AsDecimalArray.Sum(p => p * p)).ToVal(fmt);
            },
            new FuncTest("3,4", "5"),
            new FuncTest("3,4,5", "sqrt(50)"));
    }
}
