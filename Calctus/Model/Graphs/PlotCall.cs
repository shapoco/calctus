using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Evaluations;
using Shapoco.Calctus.Model.Expressions;

namespace Shapoco.Calctus.Model.Graphs {
    class PlotCall {
        public const string DefaultWindowName = "plot";

        public readonly EvalContext Context;
        public readonly string WindowName;
        public readonly Expr Expression;
        public readonly string[] Variants;

        public PlotCall(EvalContext e, string windowName, Expr expr, string[] vars) {
            Context = new EvalContext(e);
            WindowName = windowName;
            Expression = expr;
            Variants = vars;
        }
    }
}
