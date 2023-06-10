using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.Graphs {
    class PlotSettings : ICloneable {
        public decimal XMin = -10.5m;
        public decimal XMax = 10.5m;
        public int XNumSteps = 100;
        public decimal YMin = -10.5m;
        public decimal YMax = 10.5m;

        public object Clone() => MemberwiseClone();
    }
}
