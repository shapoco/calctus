using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Functions {
    class BuiltInFuncLibrary {
        private static BuiltInFuncLibrary _instance = null;
        public static BuiltInFuncLibrary Instance => _instance != null ? _instance : _instance = new BuiltInFuncLibrary();
        private BuiltInFuncLibrary() { }

        private BuiltInFuncCategory[] _categories = null;
        public BuiltInFuncCategory[] Categories => _categories != null ? _categories : _categories = enumCategories();
        private BuiltInFuncCategory[] enumCategories() =>
            Assembly.GetExecutingAssembly().GetTypes()
            .Where(p => p.BaseType == typeof(BuiltInFuncCategory))
            .Select(p => (BuiltInFuncCategory)(p.GetProperty("Instance").GetValue(null)))
            .ToArray();

        /// <summary>ネイティブ関数の一覧</summary>
        private BuiltInFuncDef[] _allFunctions = null;
        public BuiltInFuncDef[] NativeFunctions => _allFunctions != null ? _allFunctions : _allFunctions = enumEmbeddedFunctions().ToArray();
        private IEnumerable<BuiltInFuncDef> enumEmbeddedFunctions() {
            foreach (var cat in Categories) {
                foreach (var func in cat.Functions) {
                    yield return func;
                }
            }
        }

#if DEBUG
        public void DoTest() {
            var e = new EvalContext();
            foreach (var func in NativeFunctions) {
                func.DoTest(e);
            }
        }
#endif
    }
}

