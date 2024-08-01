using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    class AssertionFuncs : BuiltInFuncCategory {
        private static AssertionFuncs _instance = null;
        public static AssertionFuncs Instance => _instance != null ? _instance : _instance = new AssertionFuncs();
        private AssertionFuncs() { }

        public readonly BuiltInFuncDef assert = new BuiltInFuncDef("assert(x)",
            "Raises an error if the `x` is false.",
            (e, a) => {
                if (!a[0].ToBool()) {
                    throw new CalctusError("Assertion failed.");
                }
                return a[0];
            });
    }
}
