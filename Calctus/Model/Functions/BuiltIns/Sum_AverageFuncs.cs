using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Maths;
using Shapoco.Calctus.Model.Maths.Types;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    class Sum_AverageFuncs : BuiltInFuncCategory {
        private static Sum_AverageFuncs _instance = null;
        public static Sum_AverageFuncs Instance => _instance != null ? _instance : _instance = new Sum_AverageFuncs();
        private Sum_AverageFuncs() { }

        public readonly BuiltInFuncDef sum = new BuiltInFuncDef("sum(array...)",
            "Sum of elements of the `array`.",
            (e, a) => (a.Sum(p => p.AsDecimal)).ToRealVal(a[0].FormatHint));

        public readonly BuiltInFuncDef ave = new BuiltInFuncDef("ave(array...)",
            "Arithmetic mean of elements of the `array`.",
            (e, a) => (a.Average(p => p.AsDecimal)).ToRealVal(a[0].FormatHint));

        public readonly BuiltInFuncDef invSum = new BuiltInFuncDef("invSum(array...)",
            "Inverse of the sum of the inverses. Composite resistance of parallel resistors.",
            (e, a) => (1m / a.Sum(p => 1m / p.AsDecimal)).ToRealVal(a[0].FormatHint));

        public readonly BuiltInFuncDef harMean = new BuiltInFuncDef("harMean(array...)",
            "Harmonic mean of elements of the `array`.",
            (e, a) => ((decimal)a.Length / a.Sum(p => 1m / p.AsDecimal)).ToRealVal(a[0].FormatHint));

        public readonly BuiltInFuncDef geoMean = new BuiltInFuncDef("geoMean(array...)",
            "Geometric mean of elements of the `array`.",
            (e, a) => {
                var prod = 1m;
                foreach (var p in a) prod *= p.AsDecimal;
                return MathEx.Pow(prod, 1m / a.Length).ToRealVal(a[0].FormatHint);
            });
    }
}
