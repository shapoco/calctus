using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model {
    class Var {
        public readonly Token Name;
        public Val Value { get; set; }

        public Var(Token name) {
            this.Name = name;
            this.Value = new RealVal(0.0);
        }
    }
}
