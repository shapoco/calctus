using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Maths;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    class PrimeNumberFuncs : BuiltInFuncCategory {
        private static PrimeNumberFuncs _instance = null;
        public static PrimeNumberFuncs Instance => _instance != null ? _instance : _instance = new PrimeNumberFuncs();
        private PrimeNumberFuncs() { }

        public readonly BuiltInFuncDef prime = new BuiltInFuncDef("prime(*x)",
            "`x`-th prime number.",
            (e, a) => DMath.Prime(a[0].AsInt).ToIntVal());

        public readonly BuiltInFuncDef isPrime = new BuiltInFuncDef("isPrime(*x)",
            "Returns whether `x` is prime or not.",
            (e, a) => BoolVal.FromBool(DMath.IsPrime(a[0].AsDecimal)));

        public readonly BuiltInFuncDef primeFact = new BuiltInFuncDef("primeFact(*x)",
            "Returns prime factors of `x`.",
            (e, a) => DMath.PrimeFactors(a[0].AsDecimal).ToArrayVal(a[0].FormatHint, null));
    }
}
