using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.UnitSystem {
    class ScaledUnit : Unit {
        public readonly Unit Base;
        public readonly real Multiplyer;
        public readonly real Divisor;

        public ScaledUnit(Unit baseUnit, real mult, real div, UnitSyntax syn) : base(syn) {
            if (!this.HasSymbol) {
                throw new UnitException(this, "Internal Error: Scaled unit has to be named.");
            }
            this.Base = baseUnit;
            this.Multiplyer = mult;
            this.Divisor = div;
        }

        public ScaledUnit(Unit baseUnit, real mult, UnitSyntax syn) : this(baseUnit, mult, 1, syn) { }

        public override real UnscaleValue(EvalContext e, real val) => Base.UnscaleValue(e, val) * Multiplyer / Divisor;
        public override real ScaleValue(EvalContext e, real val) => Base.ScaleValue(e, val) * Divisor / Multiplyer;

        protected override Unit OnSqrt(EvalContext e) => Base.Sqrt(e);

        protected override IEnumerable<UnitElement> OnEnumElements() {
            yield return new UnitElement(Base, 1);
        }
    }
}
