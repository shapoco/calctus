using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Mathematics;
using Shapoco.Calctus.Model.Standards;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    class ESeriesFuncs : BuiltInFuncCategory {
        private static ESeriesFuncs _instance = null;
        public static ESeriesFuncs Instance => _instance != null ? _instance : _instance = new ESeriesFuncs();
        private ESeriesFuncs() { }

        public readonly BuiltInFuncDef esFloor = new BuiltInFuncDef("esFloor(series, *x@)",
            "Nearest E-series value less than or equal to `x` (`series`=3, 6, 12, 24, 48, 96, or 192).",
            (e, a) => PreferredNumbers.Floor(ESeries.GetSeries(a[0].AsInt), a[1].AsReal).ToRealVal());

        public readonly BuiltInFuncDef esCeil = new BuiltInFuncDef("esCeil(series, *x@)",
            "Nearest E-series value greater than or equal to `x` (`series`=3, 6, 12, 24, 48, 96, or 192).",
            (e, a) => PreferredNumbers.Ceiling(ESeries.GetSeries(a[0].AsInt), a[1].AsReal).ToRealVal());

        public readonly BuiltInFuncDef esRound = new BuiltInFuncDef("esRound(series, *x@)",
            "Nearest E-series value (`series`=3, 6, 12, 24, 48, 96, or 192).",
            (e, a) => PreferredNumbers.Round(ESeries.GetSeries(a[0].AsInt), a[1].AsReal).ToRealVal());

        public readonly BuiltInFuncDef esRatio = new BuiltInFuncDef("esRatio(series, *x)",
            "Two E-series resistor values that provide the closest value to the voltage divider ratio `x` (`series`=3, 6, 12, 24, 48, 96, or 192).",
            (e, a) => PreferredNumbers.FindSplitPair(ESeries.GetSeries(a[0].AsInt), a[1].AsReal).ToArrayVal(a[1].FormatHint, null));
    }
}
