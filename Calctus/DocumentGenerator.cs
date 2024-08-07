using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Functions.BuiltIns;
using Shapoco.Calctus.Model.Functions;

namespace Shapoco.Calctus {
    static class DocumentGenerator {
        public static readonly string ReadMeFilename = "README.md";
        public static readonly string FunctionsFilename = "FUNCTIONS.md";
        public static readonly string RestRoot = @"..\..\docsrc\source";
        public static readonly string RestFunctionDir = RestRoot + @"\functions";

        public static string VersionString {
            get {
                var vers = System.Windows.Forms.Application.ProductVersion.Split('.');
                return vers[0] + "." + vers[1];
            }
        }

        public static void WriteVersion() {
            using (var writer = new System.IO.StreamWriter("..\\..\\doc_version.txt")) {
                writer.Write(VersionString);
            }
        }

        public static string MarkdownLinkIdOf(string filename, string title) {
            var sb = new StringBuilder("./" + filename + "#");
            foreach (var c in title.ToLower()) {
                if (('a' <= c && c <= 'z') || ('0' <= c && c <= '9') || c == '_') {
                    sb.Append(c);
                }
                else if (c == ' ') {
                    sb.Append('-');
                }
            }
            return sb.ToString();
        }

        public static string MarkdownEscape(string text) => text.Replace("*", "\\*");

#if DEBUG
        public static void GenerateDocumentRst() {
            Log.Here().I("AppDataManager.AssemblyPath = '" + AppDataManager.AssemblyPath + "'");
            if (!AppDataManager.AssemblyPath.EndsWith(@"\bin\Debug")) return;
            Log.Here().I("Generating embedded function documentation...");

            var categories = BuiltInFuncLibrary.Instance.Categories.OrderBy(p => p.DocTitle).ToArray();

            int numFuncs = categories.Sum(p => p.Functions.Length);


            Directory.CreateDirectory(RestFunctionDir);

            using (var writer = new StreamWriter(RestFunctionDir + @"\index.rst")) {
                writer.WriteLine("Built-In Functions");
                writer.WriteLine("#################");
                writer.WriteLine();
                writer.WriteLine(".. toctree::");
                foreach (var cat in categories) {
                    writer.WriteLine("    " + cat.DocFileNameBase + ".rst");
                }
            }

            foreach (var cat in categories) {
                using (var writer = new StreamWriter(RestFunctionDir + @"\" + cat.DocFileNameBase + ".rst")) {
                    writer.WriteLine(cat.Name);
                    writer.WriteLine("#################");
                    writer.WriteLine();
                    foreach (var f in cat.Functions.OrderBy(p => p.DocTitle)) {
                        var validTests = f.HasTest ? f.Tests.Where(p => p.Done).ToArray() : null;
                        var testExists = validTests != null && validTests.Length > 0;
                        writer.WriteLine(f.GetDeclarationText());
                        writer.WriteLine("*****************");
                        writer.WriteLine();
                        writer.WriteLine(f.Description + (testExists ? " ::" : ""));
                        writer.WriteLine();
                        if (testExists) {
                            var indent = validTests.Max(p => p.Result.Expr.Length) + 1;
                            if (indent > 32) indent = indent * 3 / 4;
                            foreach (var test in validTests) {
                                if (test.Result.Expr.Length < indent) {
                                    writer.WriteLine("    " + test.Result.Expr.PadRight(indent) + "//--> " + test.Result.ActualValue);
                                }
                                else {
                                    writer.WriteLine("    " + test.Result.Expr);
                                    writer.WriteLine("    " + new string(' ', indent) + "//--> " + test.Result.ActualValue);
                                }
                            }
                            writer.WriteLine();
                        }
                    }
                    writer.WriteLine();
                }
            }
        }

        public static void GenerateDocumentation() {
            Log.Here().I("AppDataManager.AssemblyPath = '" + AppDataManager.AssemblyPath + "'");
            if (!AppDataManager.AssemblyPath.EndsWith(@"\bin\Debug")) return;
            Log.Here().I("Generating embedded function documentation...");

            var categories = BuiltInFuncLibrary.Instance.Categories.OrderBy(p => p.DocTitle).ToArray();

            int numFuncs = categories.Sum(p => p.Functions.Length);

            using (var writer = new StreamWriter(@"..\..\" + FunctionsFilename)) {
                writer.WriteLine("# Built-In Functions");
                writer.WriteLine();
                foreach (var cat in categories) {
                    writer.WriteLine("## " + cat.Name);
                    writer.WriteLine();
                    foreach (var f in cat.Functions.OrderBy(p => p.DocTitle)) {
                        writer.WriteLine("### `" + f.GetDeclarationText() + "`");
                        writer.WriteLine();
                        writer.WriteLine(f.Description);
                        writer.WriteLine();
                        var validTests = f.HasTest ? f.Tests.Where(p => p.Done).ToArray() : null;
                        if (validTests != null && validTests.Length > 0) {
                            writer.WriteLine("```c++");
                            var indent = validTests.Max(p => p.Result.Expr.Length) + 1;
                            if (indent > 32) indent = indent * 3 / 4;
                            foreach (var test in validTests) {
                                if (test.Result.Expr.Length < indent) {
                                    writer.WriteLine(test.Result.Expr.PadRight(indent) + "//--> " + test.Result.ActualValue);
                                }
                                else {
                                    writer.WriteLine(test.Result.Expr);
                                    writer.WriteLine(new string(' ', indent) + "//--> " + test.Result.ActualValue);
                                }
                            }
                            writer.WriteLine("```");
                            writer.WriteLine();
                        }
                    }
                    writer.WriteLine("----");
                    writer.WriteLine();
                }
            }

            var readMeBeforeTable = new StringBuilder();
            var readMeAfterTable = new StringBuilder();
            using (var reader = new StreamReader("../../" + ReadMeFilename)) {
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

            using (var writer = new StreamWriter("../../" + ReadMeFilename)) {
                writer.Write(readMeBeforeTable.ToString());
                writer.WriteLine("Now Calctus has " + numFuncs + " built-in functions.");
                writer.WriteLine();
                writer.WriteLine("|Category|Functions|");
                writer.WriteLine("|:--:|:--|");
                foreach (var cat in categories) {
                    writer.Write("|[" + cat.DocTitle + "](" + MarkdownLinkIdOf(FunctionsFilename, cat.DocTitle) + ")|");
                    bool first = true;
                    foreach (var f in cat.Functions.OrderBy(p => p.DocTitle)) {
                        if (!first) writer.Write(", ");
                        writer.Write("[" + f.DocTitle + "](" + MarkdownLinkIdOf(FunctionsFilename, f.DocTitle) + ")");
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
