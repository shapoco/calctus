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

namespace Shapoco.Calctus.Model {
    class ExtFuncDef : FuncDef {
        [DllImport("shell32.dll")]
        private static extern int FindExecutable(string lpFile, string lpDirectory, [Out] StringBuilder lpResult);

        public readonly string Path;

        public ExtFuncDef(string path) : base(
                System.IO.Path.GetFileNameWithoutExtension(path),
                new ArgDef[] { new ArgDef("args") }, null, VariadicAragumentMode.Flatten, -1,
                "External Function \"" + System.IO.Path.GetFileName(path) + "\"") {
            Path = path;
            Method = (e, a) => Exec(a);
        }

        public Val Exec(Val[] a) {
            var args = new string[a.Length];
            for(int i = 0; i < a.Length; i++) {
                args[i] = a[i].AsReal.ToString();
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
                    psi.Arguments = sf.Parameter.Replace("%s", Path).Replace("%p", string.Join(" ", args));
                }
                else if (directExec) {
                    psi.Arguments = string.Join(" ", args);
                }
                else {
                    psi.Arguments = "\"" + Path + "\" " + string.Join(" ", args);
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

        public static ExtFuncDef[] ExternalFunctions = new ExtFuncDef[0];
        private static IEnumerable<ExtFuncDef> EnumExternalFunctions() {
            var s = Settings.Instance;
            if (!Directory.Exists(s.Script_FolderPath)) yield break;
            foreach(var p in Directory.GetFiles(s.Script_FolderPath)) {
                yield return new ExtFuncDef(p);
            }
        }

        public static void ScanScripts() {
            ExternalFunctions = EnumExternalFunctions().ToArray();
        }
    }
}
