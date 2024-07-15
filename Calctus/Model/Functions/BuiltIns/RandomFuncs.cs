﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    class RandomFuncs : BuiltInFuncCategory {
        private static RandomFuncs _instance = null;
        public static RandomFuncs Instance => _instance != null ? _instance : _instance = new RandomFuncs();
        private RandomFuncs() { }

        private static readonly Random rng = new Random((int)DateTime.Now.Ticks);

        public readonly BuiltInFuncDef rand_0 = new BuiltInFuncDef("rand()",
            "Generates a random value between 0.0 and 1.0.",
            (e, a) => ((real)rng.NextDouble()).ToRealVal());

        public readonly BuiltInFuncDef rand_2 = new BuiltInFuncDef("rand(min,max)",
            "Generates a random value between min and max.",
            (e, a) => {
                var min = a[0].AsReal;
                var max = a[1].AsReal;
                return (min + (real)rng.NextDouble() * (max - min)).ToRealVal(a[0].FormatHint);
            });

        public readonly BuiltInFuncDef rand32 = new BuiltInFuncDef("rand32()",
            "Generates a 32bit random integer.",
            (e, a) => rng.Next().ToIntVal());

        public readonly BuiltInFuncDef rand64 = new BuiltInFuncDef("rand64()",
            "Generates a 64bit random integer.",
            (e, a) => {
                long lo = rng.Next();
                long hi = rng.Next();
                return ((hi << 32) | lo).ToIntVal();
            });
    }
}
