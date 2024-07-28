using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Functions {
    class ExternalFuncDef : FuncDef {
        [DllImport("shell32.dll")]
        private static extern int FindExecutable(string lpFile, string lpDirectory, [Out] StringBuilder lpResult);

        public readonly string Path;

        public ExternalFuncDef(string path) : base(
                Token.FromWord(System.IO.Path.GetFileNameWithoutExtension(path)),
                new ArgDefList(new ArgDef[] { new ArgDef("args") }, VariadicMode.Flatten, -1, -1),
                "External Function \"" + System.IO.Path.GetFileName(path) + "\"") {
            Path = path;
        }

        protected override Val OnCall(EvalContext e, Val[] args) {
            var strArgs = new string[args.Length];
            for (int i = 0; i < args.Length; i++) {
                strArgs[i] = args[i].AsDecimal.ToString();
            }

            var sf = Settings.Instance.GetScriptFilterFromPath(Path);
            if (sf == null) {
                throw new CalctusError("Interpreter not found for *" + System.IO.Path.GetExtension(Path));
            }

            var directExec = sf.IsExecutedDirectly || ScriptFilter.IsWildcardMatch(ScriptFilter.ExeFilter, Path);
            var exe = directExec ? Path : sf.Command;
            if (string.IsNullOrEmpty(exe)) {
                var sb = new StringBuilder(1024);
                if (FindExecutable(Path, "", sb) <= 32) {
                    throw new CalctusError("Associated app not found for *" + System.IO.Path.GetExtension(Path));
                }
                exe = sb.ToString();
            }

            try {
                var psi = new ProcessStartInfo();
                if (!string.IsNullOrEmpty(sf.Parameter)) {
                    psi.Arguments = sf.Parameter.Replace("%s", Path).Replace("%p", string.Join(" ", strArgs));
                }
                else if (directExec) {
                    psi.Arguments = string.Join(" ", strArgs);
                }
                else {
                    psi.Arguments = "\"" + Path + "\" " + string.Join(" ", strArgs);
                }
                psi.FileName = exe;
                psi.CreateNoWindow = true;
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                var p = Process.Start(psi);
                var output = p.StandardOutput.ReadToEnd();
                var expr = Parser.Parse(output);
                return expr.Eval(new EvalContext());
            }
            catch (Exception ex) {
                throw new CalctusError("Script Failed: " + ex.Message);
            }
        }

        public static ExternalFuncDef[] ExternalFunctions = new ExternalFuncDef[0];
        private static IEnumerable<ExternalFuncDef> enumExternalFunctions() {
            var s = Settings.Instance;
            if (!Directory.Exists(s.Script_FolderPath)) yield break;
            foreach(var p in Directory.GetFiles(s.Script_FolderPath)) {
                yield return new ExternalFuncDef(p);
            }
        }

        public static void ScanScripts() {
            ExternalFunctions = enumExternalFunctions().ToArray();
        }
    }
}
