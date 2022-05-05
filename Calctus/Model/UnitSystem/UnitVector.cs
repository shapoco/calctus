using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.UnitSystem {
    /*
    class UnitVector : ReadOnlyDictionary<string, _old_ScalarUnit> {
        private static Dictionary<string, _old_ScalarUnit> dictionarize(IEnumerable<_old_ScalarUnit> units) {
            var dic = new Dictionary<string, _old_ScalarUnit>();
            foreach (var unit in units) {
                dic.Add(unit.Dimension.Id, unit);
            }
            return dic;
        }

        private static Dictionary<string, _old_ScalarUnit> dictionarize(params _old_ScalarUnit[] units) {
            return dictionarize(units.AsEnumerable());
        }

        public UnitVector(IDictionary<string, _old_ScalarUnit> dic) : base(dic) { }
        public UnitVector(IEnumerable<_old_ScalarUnit> units) : base(dictionarize(units)) { }
        public UnitVector(params _old_ScalarUnit[] units) : base(dictionarize(units)) { }
    }
    */
}
