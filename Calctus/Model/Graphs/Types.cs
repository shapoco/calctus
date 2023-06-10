using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Shapoco.Calctus.Model.Sheets;

namespace Shapoco.Calctus.Model.Graphs {
    delegate void PlottedEventHandler(Plotter sender, PlottedEventArgs e);
    
    class PlottedEventArgs {
        public readonly Sheet Sheet;
        public readonly Graph[] Graphs;
        public PlottedEventArgs(Sheet sheet, Graph[] graphs) {
            Sheet = sheet;
            Graphs = graphs;
        }
    }

    struct PointD {
        public static readonly PointD Empty = new PointD(0, 0);
        public decimal X;
        public decimal Y;
        public PointD(decimal x, decimal y) {
            X = x;
            Y = y;
        }
    }

    class Polyline {
        public readonly PointD[] Points;
        public Polyline(PointD[] points) {
            Points = points;
        }
    }
}
