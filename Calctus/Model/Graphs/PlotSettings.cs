using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.Graphs {
    class PlotSettings : ICloneable {
        public event EventHandler Changed;

        public readonly AxisSettings XAxis = new AxisSettings();
        public readonly AxisSettings YAxis = new AxisSettings();
        private int _xNumSteps = 100;

        public PlotSettings() {
            XAxis.Changed += (sender, e) => { PerformChanged(); };
            YAxis.Changed += (sender, e) => { PerformChanged(); };
        }

        public int NumSamples {
            get => _xNumSteps;
            set {
                if (value == _xNumSteps) return;
                _xNumSteps = value;
                PerformChanged();
            }
        }

        public void PerformChanged() => Changed?.Invoke(this, EventArgs.Empty);

        public object Clone() {
            var clone = new PlotSettings();
            clone.NumSamples = NumSamples;
            XAxis.CopyTo(clone.XAxis);
            YAxis.CopyTo(clone.YAxis);
            return clone;
        }
    }
}
