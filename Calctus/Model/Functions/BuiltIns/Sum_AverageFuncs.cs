﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Mathematics;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    static class Sum_AverageFuncs {
        public static readonly BuiltInFuncDef sum = new BuiltInFuncDef("sum(x...)", (e, a) => new RealVal(a.Sum(p => p.AsReal), a[0].FormatHint), "Sum of the arguments");
        public static readonly BuiltInFuncDef ave = new BuiltInFuncDef("ave(x...)", (e, a) => new RealVal(a.Average(p => p.AsReal), a[0].FormatHint), "Arithmetic mean of the arguments");
        public static readonly BuiltInFuncDef invSum = new BuiltInFuncDef("invSum(x...)", (e, a) => new RealVal(1m / a.Sum(p => 1m / p.AsReal), a[0].FormatHint), "Inverse of the sum of the inverses");
        public static readonly BuiltInFuncDef harMean = new BuiltInFuncDef("harMean(x...)", (e, a) => new RealVal((real)a.Length / a.Sum(p => 1m / p.AsReal), a[0].FormatHint), "Harmonic mean of the arguments");
        public static readonly BuiltInFuncDef geoMean = new BuiltInFuncDef("geoMean(x...)", (e, a) => {
            var prod = (real)1;
            foreach (var p in a) prod *= p.AsReal;
            return new RealVal(RMath.Pow(prod, 1m / a.Length), a[0].FormatHint);
        }, "Geometric mean of the arguments");
    }
}
