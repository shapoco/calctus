using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Mathematics;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    static class SolveFuncs {
        public static readonly BuiltInFuncDef solve_1 = new BuiltInFuncDef(
            "solve(func)", (e, a) => NewtonsMethod.Solve(e, (FuncDef)a[0].Raw), "Solve func(x)=0 using Newton's Method.");
        
        public static readonly BuiltInFuncDef solve_2 = new BuiltInFuncDef(
            "solve(func,init)", (e, a) => NewtonsMethod.Solve(e, (FuncDef)a[0].Raw, a[1]), "Solve func(x)=0 using Newton's Method with initial value.");
        
        public static readonly BuiltInFuncDef solve_3 = new BuiltInFuncDef(
            "solve(func,min,max)", (e, a) => NewtonsMethod.Solve(e, (FuncDef)a[0].Raw, a[1], a[2]), "Solve func(x)=0 using Newton's Method with range of initial value.");
    }
}
