using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.UnitSystem {
    /*
    /// <summary>基本単位</summary>
    abstract class _old_ScalarUnit : Unit {
        public abstract _old_BaseUnit Base { get; }
        public abstract Dim Dimension { get; }
        public abstract int Exponent { get; }
        public abstract double Offset { get; }
        public abstract double Mult { get; }
        public abstract double Div { get; }
        public readonly char[] Prefixies;

        public _old_ScalarUnit(UnitSyntax syntax, params char[] prefixes) : base(syntax) {
            this.Prefixies = prefixes;
        }

        public _old_ScalarUnit(UnitSyntax syntax, string prefixes = "") : this(syntax, prefixes.ToArray()) { }

        public override bool _old_HasDimension(string id) => (id == Dimension.Id);

        public override _old_ScalarUnit this[string id] {
            get {
                if (_old_HasDimension(id))
                    return this;
                else
                    throw new UnitException(this, id + " is not part of " + this.ToString());
            }
        }

        public override IEnumerable<_old_ScalarUnit> _old_BaseUnits { get { yield return this; } }

        public _old_ScalarUnit GetFrom(double mult, double div, int exp) {
            if (Base.Mult == mult && Base.Div == div && Base.Exponent == exp) {
                return Base;
            }
            else {
                var ret = new _old_ScaledBaseUnit(UnitSyntax.Unnamed, Base, mult, div, exp); 
                return ret;
            }
        }
    }

    */
}
