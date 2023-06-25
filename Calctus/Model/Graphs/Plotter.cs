using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Shapoco.Calctus.Model.Sheets;

namespace Shapoco.Calctus.Model.Graphs {
    class Plotter {
        public event PlottedEventHandler Plotted;
        public ISynchronizeInvoke SynchronizingObject;

        private List<PlotRequest> _waitList = new List<PlotRequest>();
        private Task _task = null;
        private object _syncObject = new object();

        public void StartPlot(PlotRequest req) {
            lock(_syncObject) {
                _waitList.RemoveAll(p => p.Sheet == req.Sheet);
                _waitList.Add(req);
                if (_task == null) {
                    _task = Task.Factory.StartNew(PlotTask);
                }
            }
        }

        public void PlotTask() {
            while (true) {
                PlotRequest req = null;
                lock (_syncObject) {
                    if (_waitList.Count > 0) {
                        req = _waitList[0];
                        _waitList.RemoveAt(0);
                    }
                    if (req == null) {
                        _task = null;
                        break;
                    }
                }

                var graphs = new List<Graph>();
                foreach (var call in req.Calls) {
                    graphs.Add(Graph.Plot(call, req.Settings));
                }

                var e = new PlottedEventArgs(req.Sheet, graphs.ToArray());
                if (Plotted != null) {
                    if (SynchronizingObject != null) {
                        SynchronizingObject.Invoke(Plotted, new object[] { this, e });
                    }
                    else {
                        Plotted.Invoke(this, e);
                    }
                }
            }
        }
    }
}
