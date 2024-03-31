using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Graphs;
using Shapoco.Calctus.Model.Types;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    static class PlottingFuncs {
        public static readonly BuiltInFuncDef plot = new BuiltInFuncDef("plot(func)", (e, a) => {
            var func = (FuncDef)a[0].Raw;
            var req = new PlotCall(e, PlotCall.DefaultWindowName, func);
            e.PlotCalls.Add(req);
            return NullVal.Instance;
        }, "Plot graph of `func(x)`.");
    }
}
