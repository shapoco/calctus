using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.UnitSystem {
    class UnitException : Exception {
        public readonly Unit Unit;
        
        public UnitException(Unit unit, string msg) : base(msg) {
            this.Unit = unit;
        }

        public UnitException(string msg) : this(null, msg) { }
    }
}
