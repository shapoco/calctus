using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Sheets;

namespace Shapoco.Calctus.Model.Graphs {
    class PlotRequest {
        public readonly Sheet Sheet;
        public readonly PlotCall[] Calls;
        public readonly PlotSettings Settings;
        public PlotRequest(Sheet sheet, PlotCall[] calls, PlotSettings settings) {
            Sheet = sheet;
            Calls = calls;
            Settings = (PlotSettings)(settings.Clone());
        }
    }
}
