using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Evaluations;
using Shapoco.Calctus.Model.Mathematics;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Formats;
using Shapoco.Calctus.Model.Graphs;

namespace Shapoco.Calctus.Model.Expressions {
    internal class PlotExpr : Expr {
        public readonly Expr WindowName;
        public readonly Expr Expression;
        public readonly Token Variant;

        public PlotExpr(Token keyword, Expr windowName, Expr expr, Token variant) : base(keyword) {
            WindowName = windowName;
            Expression = expr;
            Variant = variant;
        }

        protected override Val OnEval(EvalContext e) {
            string windowName = PlotCall.DefaultWindowName;
            if (WindowName != null) {
                windowName = WindowName.Eval(e).AsString;
            }
            var req = new PlotCall(e, windowName, Expression, new string[] { Variant.Text });
            e.PlotCalls.Add(req);
            return new BoolVal(true);
        }
    }
}
