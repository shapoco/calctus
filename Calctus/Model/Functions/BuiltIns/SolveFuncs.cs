using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Maths;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    class SolveFuncs : BuiltInFuncCategory {
        private static SolveFuncs _instance = null;
        public static SolveFuncs Instance => _instance != null ? _instance : _instance = new SolveFuncs();
        private SolveFuncs() { }

        public readonly BuiltInFuncDef solve_1 = new BuiltInFuncDef("solve(func)",
            "Returns solutions of `func(x)=0` using Newton's Method.",
            (e, a) => NewtonsMethod.Solve(e, (FuncDef)a[0].Raw));

        public readonly BuiltInFuncDef solve_2 = new BuiltInFuncDef("solve(func,array)",
            "Returns solutions of `func(x)=0` using Newton's Method with initial value in `array`.",
            (e, a) => NewtonsMethod.Solve(e, (FuncDef)a[0].Raw, a[1]));

        public readonly BuiltInFuncDef solve_3 = new BuiltInFuncDef("solve(func,min,max)",
            "Returns solutions of `func(x)=0` using Newton's Method with initial value between `min` and `max`.",
            (e, a) => NewtonsMethod.Solve(e, (FuncDef)a[0].Raw, a[1], a[2]));
    }
}
