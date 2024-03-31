using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    static class RandomFuncs {
        private static readonly Random rng = new Random((int)DateTime.Now.Ticks);

        public static readonly BuiltInFuncDef rand = new BuiltInFuncDef("rand()", (e, a) => new RealVal((real)rng.NextDouble()), "Generates a random value between 0.0 and 1.0.");
        public static readonly BuiltInFuncDef rand_2 = new BuiltInFuncDef("rand(min,max)", (e, a) => {
            var min = a[0].AsReal;
            var max = a[1].AsReal;
            return new RealVal(min + (real)rng.NextDouble() * (max - min));
        }, "Generates a random value between min and max.");
        public static readonly BuiltInFuncDef rand32 = new BuiltInFuncDef("rand32()", (e, a) => new RealVal(rng.Next()), "Generates a 32bit random integer.");
        public static readonly BuiltInFuncDef rand64 = new BuiltInFuncDef("rand64()", (e, a) => new RealVal((((long)rng.Next()) << 32) | ((long)rng.Next())), "Generates a 64bit random integer.");
    }
}
