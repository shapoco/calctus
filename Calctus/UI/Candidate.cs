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

        public Candidate(string id, string label, string desc) {
            Id = id;
            Label = label;
            Description = desc;
        }

        public override string ToString() => Label;
    }
}
