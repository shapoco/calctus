using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Shapoco.Calctus.Model.Values;
using Shapoco.Calctus.Model.Maths;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    class SystemFuncs : BuiltInFuncCategory {
        private static SystemFuncs _instance = null;
        public static SystemFuncs Instance => _instance != null ? _instance : _instance = new SystemFuncs();
        private SystemFuncs() { }

        public readonly BuiltInFuncDef assert = new BuiltInFuncDef("assert(x)",
            "Raises an error if the `x` is false.",
            (e, a) => {
                if (!a[0].ToBool()) {
                    throw new CalctusError("Assertion failed.");
                }
                return a[0];
            });

        public readonly BuiltInFuncDef version = new BuiltInFuncDef("version()",
            "Returns current version of " + Application.ProductName + ".",
            (e, a) => Application.ProductVersion.ToVal()
#if !DEBUG
            , new FuncTest("", "\"" + Application.ProductVersion + "\"")
#endif
            );

        public readonly BuiltInFuncDef isDebugBuild = new BuiltInFuncDef("isDebugBuild()",
            "Whether or not the running " + Application.ProductName + " is a debug build.",
            (e, a) => Program.DebugMode.ToVal());
    }
}
