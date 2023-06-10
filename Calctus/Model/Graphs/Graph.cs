using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Graphs {
    class Graph {
        public static Graph Plot(PlotCall call, PlotSettings ps) {
            return new Graph(call, ps);
        }

        public readonly PlotCall Call;
        private Polyline[] _lines;

        public IEnumerable<Polyline> Polylines => _lines;

        private Graph(PlotCall call, PlotSettings ps) {
            Call = call;

            var lines = new List<Polyline>();
            var points = new List<PointD>();
            for (int i = 0; i <= ps.XNumSteps; i++) {
                var x = ps.XMin + (ps.XMax - ps.XMin) * i / ps.XNumSteps;
                var y = 0m;
                bool success = false;
                try {
                    call.Context.Ref(call.Variants[0], true).Value = new RealVal(x);
                    y = call.Expression.Eval(call.Context).AsReal;
                    success = true;
                }
                catch { }

                if (success) {
                    points.Add(new PointD(x, y));
                }

                if (!success || i >= ps.XNumSteps - 1) {
                    if (points.Count > 0) {
                        lines.Add(new Polyline(points.ToArray()));
                        points.Clear();
                    }
                }
            }

            _lines = lines.ToArray();
        }

        
    }
}
