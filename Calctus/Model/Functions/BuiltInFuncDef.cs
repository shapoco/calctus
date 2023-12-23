using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Functions {
    class BuiltInFuncDef : FuncDef {
        public const string EmbeddedLibraryNamespace = "Shapoco.Calctus.Model.Functions.BuiltIns";

        public Func<EvalContext, Val[], Val> Method { get; protected set; }

        public BuiltInFuncDef(string prototype, Func<EvalContext, Val[], Val> method, string desc) 
            : base(prototype , desc){
            this.Method = method;
        }

        protected override Val OnCall(EvalContext e, Val[] args) {
            return Method(e, args);
        }



        /// <summary>ネイティブ関数の一覧</summary>
        public static BuiltInFuncDef[] NativeFunctions = enumEmbeddedFunctions().ToArray();
        private static IEnumerable<BuiltInFuncDef> enumEmbeddedFunctions() {
            var list = new List<BuiltInFuncDef>();
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes()) {
                if (type.Namespace == EmbeddedLibraryNamespace) {
                    list.AddRange(EnumFunctions(type));
                }
            }
            return list;
        }

        public static IEnumerable<BuiltInFuncDef> EnumFunctions(Type libType) {
            return libType
                .GetFields()
                .Where(p => p.IsStatic && (p.FieldType == typeof(BuiltInFuncDef)))
                .Select(p => (BuiltInFuncDef)p.GetValue(null));
        }

#if DEBUG
        public static void GenerateDocumentation() {
            if (!AppDataManager.AssemblyPath.EndsWith(@"\bin\Debug")) return;
            Console.WriteLine("Generating embedded function documentation...");
            using (var writer = new StreamWriter("../../FUNCTIONS.md")) {
                writer.WriteLine("# Embedded Functions");
                writer.WriteLine();
                foreach (var libType in Assembly.GetExecutingAssembly().GetTypes().Where(p => p.Name.EndsWith("Funcs")).OrderBy(p => p.Name)) {
                    var libName = libType.Name.Substring(0, libType.Name.Length - 5);
                    var sb = new StringBuilder();
                    foreach(var c in libName) {
                        if (c == '_') {
                            sb.Append('/');
                        }
                        else if (sb.Length > 0 && sb[sb.Length-1] != '/' && 'A' <= c &&  c<= 'Z') {
                            sb.Append(' ').Append(c);
                        }
                        else {
                            sb.Append(c);
                        }
                    }
                    libName = sb.ToString();

                    writer.WriteLine("## " + libName);
                    writer.WriteLine();
                    foreach (var func in EnumFunctions(libType).OrderBy(p => p.Name.Text)) {
                        writer.WriteLine("### `" + func.ToString() + "`");
                        writer.WriteLine();
                        writer.WriteLine(func.Description);
                        writer.WriteLine();
                    }
                    writer.WriteLine("----");
                }
            }
        }
#endif
    }
}
