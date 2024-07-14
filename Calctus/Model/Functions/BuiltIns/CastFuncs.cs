using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Mathematics;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    static class CastFuncs {
        public static readonly BuiltInFuncDef real = new BuiltInFuncDef("real(*x)", (e, a) => a[0].AsRealVal().FormatReal(), "Converts the `x` to a real number.");
        public static readonly BuiltInFuncDef rat = new BuiltInFuncDef("rat(*x)", (e, a) => new FracVal(RMath.FindFrac(a[0].AsReal)), "Rational fraction approximation of `x`.");
        public static readonly BuiltInFuncDef rat_2 = new BuiltInFuncDef("rat(*x, max)", (e, a) => new FracVal(RMath.FindFrac(a[0].AsReal, a[1].AsReal, a[1].AsReal), a[0].FormatHint), "Rational fraction approximation of `x`.");

        public static readonly BuiltInFuncDef array = new BuiltInFuncDef("array(s)", (e, a) => {
            if (a[0] is StrVal strVal) {
                return strVal.AsArrayVal();
            }
            else if (a[0] is ArrayVal) {
                return a[0];
            }
            else {
                throw new EvalError(e, Token.Empty, "Value is not string or array.");
            }
        }, "Converts string `s` to an array of character code.");

        public static readonly BuiltInFuncDef str = new BuiltInFuncDef("str(x)", (e, a) => {
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
        }, "Converts `x` to a string.");

    }
}
