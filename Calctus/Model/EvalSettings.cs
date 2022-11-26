using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model {
    class EvalSettings {
        public bool ENotationEnabled { get; set; } = true;
        public int ENotationExpPositiveMin { get; set; } = 15;
        public int ENotationExpNegativeMax { get; set; } = -5;
        public bool ENotationAlignment { get; set; } = false;
    }
}
