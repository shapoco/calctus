﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Mathematics;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    static class Gcd_LcmFuncs {
        public static readonly BuiltInFuncDef gcd = new BuiltInFuncDef("gcd(array...)", (e, a) => new RealVal(RMath.Gcd(a.Select(p => (decimal)p.AsReal).ToArray()), a[0].FormatHint), "Greatest common divisor of elements of the `array`.");
        public static readonly BuiltInFuncDef lcm = new BuiltInFuncDef("lcm(array...)", (e, a) => new RealVal(RMath.Lcm(a.Select(p => (decimal)p.AsReal).ToArray()), a[0].FormatHint), "Least common multiple of elements of the `array`.");
    }
}
