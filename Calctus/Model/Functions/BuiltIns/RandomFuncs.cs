using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Values;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    class RandomFuncs : BuiltInFuncCategory {
        private static RandomFuncs _instance = null;
        public static RandomFuncs Instance => _instance != null ? _instance : _instance = new RandomFuncs();
        private RandomFuncs() { }

        private static readonly Random rng = new Random((int)DateTime.Now.Ticks);

        public readonly BuiltInFuncDef rand_0 = new BuiltInFuncDef("rand()",
            "Generates a random value between 0.0 and 1.0.",
            (e, a) => ((decimal)rng.NextDouble()).ToVal());

        public readonly BuiltInFuncDef rand_2 = new BuiltInFuncDef("rand(min,max)",
            "Generates a random value between min and max.",
            (e, a) => {
                var min = a[0].AsDecimal;
                var max = a[1].AsDecimal;
                return (min + (decimal)rng.NextDouble() * (max - min)).ToVal(a[0].FormatFlags);
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
