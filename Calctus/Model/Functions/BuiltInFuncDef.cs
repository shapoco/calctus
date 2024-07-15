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
        private class FuncCategory {
            public readonly string Name;
            public readonly FuncDescription[] Functions;
            public FuncCategory(Type categoryType) {
                var typeName = categoryType.Name.Substring(0, categoryType.Name.Length - 5);
                var sb = new StringBuilder();
                foreach (var c in typeName) {
                    if (c == '_') {
                        sb.Append('/');
                    }
                    else if (sb.Length > 0 && sb[sb.Length - 1] != '/' && 'A' <= c && c <= 'Z') {
                        sb.Append(' ').Append(c);
                    }
                    else {
                        sb.Append(c);
                    }
                }
                this.Name = sb.ToString();

                this.Functions = EnumFunctions(categoryType)
                    .OrderBy(p => p.Name.Text)
                    .Select(p => new FuncDescription(p))
                    .ToArray();
            }
        }

        private class FuncDescription {
            public readonly string DeclText;
            public readonly string Description;
            public FuncDescription(BuiltInFuncDef funcDef) {
                this.DeclText = funcDef.ToString();
                this.Description = funcDef.Description;
            }
        }

        public static void GenerateDocumentation() {
            Console.WriteLine("AppDataManager.AssemblyPath = '" + AppDataManager.AssemblyPath + "'");
            if (!AppDataManager.AssemblyPath.EndsWith(@"\bin\Debug")) return;
            Console.WriteLine("Generating embedded function documentation...");

            var categories = Assembly.GetExecutingAssembly().GetTypes()
                .Where(p => p.Name.EndsWith("Funcs"))
                .OrderBy(p => p.Name)
                .Select(p => new FuncCategory(p))
                .ToArray();

            int numFuncs = categories.Sum(p => p.Functions.Length);

            using (var writer = new StreamWriter(@"..\..\FUNCTIONS.md")) {
                writer.WriteLine("# Built-In Functions");
                writer.WriteLine();
                foreach (var cat in categories) {
                    writer.WriteLine("## " + cat.Name);
                    writer.WriteLine();
                    foreach (var f in cat.Functions) {
                        writer.WriteLine("### `" + f.DeclText + "`");
                        writer.WriteLine();
                        writer.WriteLine(f.Description);
                        writer.WriteLine();
                    }
                    writer.WriteLine("----");
                }
            }

            var readMeBeforeTable = new StringBuilder();
            var readMeAfterTable = new StringBuilder();
            using (var reader = new StreamReader("../../README.md")) {
                bool isBeforeTable = true;
                bool isAfterTable = false;
                while (!reader.EndOfStream) {
                    var line = reader.ReadLine();
                    if (isBeforeTable) {
                        readMeBeforeTable.AppendLine(line);
                        if (line.Trim() == "<!-- START_OF_BUILT_IN_FUNCTION_TABLE -->") {
                            isBeforeTable = false;
                        }
                    }
                    else if (!isAfterTable) {
                        if (line.Trim() == "<!-- END_OF_BUILT_IN_FUNCTION_TABLE -->") {
                            readMeAfterTable.AppendLine(line);
                            isAfterTable = true;
                        }
                    }
                    else if (!reader.EndOfStream || !string.IsNullOrEmpty(line.Trim())) {
                        readMeAfterTable.AppendLine(line);
                    }
                }
            }

            using (var writer = new StreamWriter("../../README.md")) {
                writer.Write(readMeBeforeTable.ToString());
                writer.WriteLine("Now Calctus has " + numFuncs + " built-in functions.");
                writer.WriteLine();
                writer.WriteLine("|Category|Functions|");
                writer.WriteLine("|:--:|:--|");
                foreach (var cat in categories) {
                    writer.Write("|" + cat.Name + "|");
                    bool first = true;
                    foreach (var f in cat.Functions) {
                        if (!first) writer.Write(", ");
                        writer.Write("`" + f.DeclText.Replace(" ", "") + "`");
                        first = false;
                    }
                    writer.WriteLine("|");
                }
                writer.Write(readMeAfterTable.ToString());
            }
        }
#endif
    }
}
