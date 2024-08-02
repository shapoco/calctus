using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Mathematics;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    static class PrimeNumberFuncs {
        public static readonly BuiltInFuncDef prime = new BuiltInFuncDef("prime(*x)", (e, a) => new RealVal(RMath.Prime(a[0].AsInt)), "`x`-th prime number.");
        public static readonly BuiltInFuncDef isPrime = new BuiltInFuncDef("isPrime(*x)", (e, a) => BoolVal.FromBool(RMath.IsPrime(a[0].AsReal)), "Returns whether `x` is prime or not.");
        public static readonly BuiltInFuncDef primeFact = new BuiltInFuncDef("primeFact(*x)", (e, a) => new ArrayVal(RMath.PrimeFactors(a[0].AsReal), a[0].FormatHint), "Returns prime factors of `x`.");
    }
}
