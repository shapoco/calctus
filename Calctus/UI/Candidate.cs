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
        public readonly bool IsFunction;

        public Candidate(string id, string label, string desc, bool isFunc) {
            Id = id;
            Label = label;
            Description = desc;
            IsFunction = isFunc;
        }

        public override string ToString() => Label;
    }
}
