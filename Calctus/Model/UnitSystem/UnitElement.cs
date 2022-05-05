using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.UnitSystem {
    class UnitElement {
        public readonly Unit Unit;
        public readonly int Exp;

        public UnitElement(Unit unit, int exp) {
            this.Unit = unit;
            this.Exp = exp;
        }

        public UnitElement Pow(int exp) => new UnitElement(this.Unit, this.Exp * exp);

        public override string ToString() {
            if (Exp >= 2 || Exp == 0)
                return Unit.ToString() + Exp;
            else if (Exp == 1)
                return Unit.ToString();
            else
                return "/" + Unit.ToString() + Exp;
        }
    }
}
