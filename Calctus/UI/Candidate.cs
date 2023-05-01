using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Shapoco.Calctus.UI {
    class Candidate {
        public readonly string Id;
        public readonly string Label;
        public readonly string Description;
        public readonly Color Color;

        public Candidate(string id, string label, string desc, Color color) {
            Id = id;
            Label = label;
            Description = desc;
            Color = color;
        }

        public override string ToString() => Label;
    }
}
