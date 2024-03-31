using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    class AssertionFuncs {
        public static readonly BuiltInFuncDef assert = new BuiltInFuncDef("assert(x)", (e, a) => {
            if (!a[0].AsBool) {
                throw new CalctusError("Assertion failed.");
            }
            return a[0];
        }, "Raises an error if the `x` is false.");
    }
}
