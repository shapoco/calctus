using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Maths;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    class SystemFuncs : BuiltInFuncCategory {
        private static SystemFuncs _instance = null;
        public static SystemFuncs Instance => _instance != null ? _instance : _instance = new SystemFuncs();
        private SystemFuncs() { }

        public readonly BuiltInFuncDef version = new BuiltInFuncDef("version()",
            "Returns current version of " + Application.ProductName + ".",
            (e, a) => Application.ProductVersion.ToStrVal()
#if !DEBUG
            , new FuncTest("", "\"" + Application.ProductVersion + "\"")
#endif
            );

        public readonly BuiltInFuncDef isDebugBuild = new BuiltInFuncDef("isDebugBuild()",
            "Whether or not the running " + Application.ProductName + " is a debug build.",
#if DEBUG
            (e, a) => BoolVal.True);
#else
            (e, a) => BoolVal.False);
#endif
    }
}
