using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Graphs;
using Shapoco.Calctus.Model.Types;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    class PlottingFuncs : BuiltInFuncCategory {
        private static PlottingFuncs _instance = null;
        public static PlottingFuncs Instance => _instance != null ? _instance : _instance = new PlottingFuncs();
        private PlottingFuncs() { }

        public readonly BuiltInFuncDef plot = new BuiltInFuncDef("plot(func)",
            "Plot graph of `func(x)`.",
            (e, a) => {
                var func = (FuncDef)a[0].Raw;
                var req = new PlotCall(e, PlotCall.DefaultWindowName, func);
                e.PlotCalls.Add(req);
                return NullVal.Instance;
            });
    }
}
