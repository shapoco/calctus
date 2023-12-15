using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Evaluations;
using Shapoco.Calctus.Model.Expressions;
using Shapoco.Calctus.Model.Functions;

namespace Shapoco.Calctus.Model.Graphs {
    class PlotCall {
        public const string DefaultWindowName = "plot";

        public readonly EvalContext Context;
        public readonly string WindowName;
        public readonly FuncDef Function;

        public PlotCall(EvalContext e, string windowName, FuncDef func) {
            Context = new EvalContext(e);
            WindowName = windowName;
            Function = func;
        }
    }
}
