using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.UnitSystem {
    /*
    class _old_ScaledBaseUnit : ScalarUnit {
        private readonly _old_BaseUnit _base;
        private readonly double _mult;
        private readonly double _div;
        private readonly int _exp;
        private readonly double _offset;
        private readonly UnitVector _dic;

        public override _old_BaseUnit Base => _base;
        public override Dim Dimension => _base.Dimension;

        public override double Mult => _mult;
        public override double Div => _div;
        public override int Exponent => _exp;
        public override double Offset => _offset;

        public override UnitVector _old_Dictionary => _dic;

        public _old_ScaledBaseUnit(UnitSyntax syn, ScalarUnit unit, double mult = 1, double div = 1, int exp = 1, double offset = 0) : base(syn) {
            this._base = unit.Base;
            this._mult = mult;
            this._div = div;
            this._exp = exp;
            this._offset = offset;
            this._dic = unit._old_Dictionary;
        }
    }
    */
}
