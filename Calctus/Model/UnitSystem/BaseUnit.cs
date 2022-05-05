using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.UnitSystem {

    /// <summary>基本単位</summary>
    class BaseUnit : Unit {
        public readonly Dim Dimension;

        public BaseUnit(Dim dim, UnitSyntax syn = null) : base(syn) {
            if (!this.HasSymbol) {
                throw new UnitException(this, "Internal Error: Base unit has to be named.");
            }
            this.Dimension = dim;
        }

        public override double ScaleValue(EvalContext e, double val) => val;
        public override double UnscaleValue(EvalContext e, double val) => val;

        protected override IEnumerable<UnitElement> OnEnumElements() { yield break; }

        //protected override Unit OnMul(EvalContext e, Unit b);
        //protected override Unit OnDiv(EvalContext e, Unit b);
        //protected override Unit OnPow(EvalContext e, int exp);
        //protected override Unit OnInvert(EvalContext e);
        protected override Unit OnSqrt(EvalContext e) => throw new UnitException(this, "Base unit can not be square root.");

    }
}
