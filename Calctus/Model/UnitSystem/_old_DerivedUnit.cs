using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.UnitSystem {
    /*
    /// <summary>組立単位</summary>
    class _old_DerivedUnit : Unit {
        private readonly UnitVector _set;

        public _old_DerivedUnit(UnitSyntax syntax, UnitVector dic) : base(syntax) {
            this._set = dic;
        }

        public _old_DerivedUnit(UnitSyntax syntax, params _old_ScalarUnit[] elms) : this(syntax, new UnitVector(elms)) { }

        public override IEnumerable<_old_ScalarUnit> _old_BaseUnits => _set.Values;
        public override UnitVector _old_Dictionary => _set;
        public override _old_ScalarUnit this[string id] => _set[id];
        public override bool _old_HasDimension(string id) => _set.ContainsKey(id);
    }
    */
}
