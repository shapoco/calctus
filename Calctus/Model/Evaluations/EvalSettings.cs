using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.Evaluations {
    class EvalSettings : ICloneable {
        public int DecimalLengthToDisplay { get; set; } = 9;
        public bool ENotationEnabled { get; set; } = true;
        public int ENotationExpPositiveMin { get; set; } = 15;
        public int ENotationExpNegativeMax { get; set; } = -5;
        public bool ENotationAlignment { get; set; } = false;
        public bool AllowExternalFunctions { get; set; } = false;

        public object Clone() => MemberwiseClone();
    }
}
