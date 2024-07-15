using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Mathematics;
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
            (e, a) => a[0].AsRealVal().Format(FormatHint.CStyleReal));

        public readonly BuiltInFuncDef rat_1 = new BuiltInFuncDef("rat(*x)",
            "Rational fraction approximation of `x`.",
            (e, a) => RMath.FindFrac(a[0].AsReal).ToFracVal());

        public readonly BuiltInFuncDef rat_2 = new BuiltInFuncDef("rat(*x, max)",
            "Rational fraction approximation of `x`.",
            (e, a) => RMath.FindFrac(a[0].AsReal, a[1].AsReal, a[1].AsReal).ToFracVal());

        public readonly BuiltInFuncDef array = new BuiltInFuncDef("array(s)",
            "Converts string `s` to an array of character code.",
            (e, a) => {
                if (a[0] is StrVal strVal) {
                    return strVal.AsArrayVal();
                }
                else if (a[0] is ArrayVal) {
                    return a[0];
                }
                else {
                    throw new EvalError(e, Token.Empty, "Value is not string or array.");
                }
            });

        public readonly BuiltInFuncDef str = new BuiltInFuncDef("str(x)",
            "Converts `x` to a string.",
            (e, a) => {
                if (a[0] is ArrayVal arrayVal) {
                    var vals = (Val[])arrayVal.Raw;
                    var sb = new StringBuilder();
                    foreach (var val in vals) {
                        sb.Append(val.AsChar);
                    }
                    return new StrVal(sb.ToString());
                }
                else if (a[0] is StrVal) {
                    return a[0];
                }
                else {
                    return new StrVal(a[0].AsString);
                }
            });
    }
}
