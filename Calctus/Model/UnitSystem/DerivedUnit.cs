using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.UnitSystem {
    class DerivedUnit : Unit {
        private readonly IReadOnlyCollection<UnitElement> Elements;
        public DerivedUnit(UnitElement[] units, UnitSyntax syn = null) : base(syn) {
            this.Elements = Array.AsReadOnly(units);
        }

        public DerivedUnit(IEnumerable<UnitElement> units) : this(units.ToArray(), null) { }
        public DerivedUnit(params UnitElement[] units) : this(units, null) { }

        public override real UnscaleValue(EvalContext e, real val) {
            foreach (var elm in Elements) {
                val = val * RMath.Pow(elm.Unit.UnscaleValue(e, 1), elm.Exp);
            }
            return val;
        }

        public override real ScaleValue(EvalContext e, real val) {
            foreach (var elm in Elements) {
                val = val / RMath.Pow(elm.Unit.UnscaleValue(e, 1), elm.Exp);
            }
            return val;
        }

        protected override IEnumerable<UnitElement> OnEnumElements() => Elements;

        protected override Unit OnSqrt(EvalContext e) => throw new NotImplementedException(); // todo: impl
    }

}
