using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using Shapoco.Calctus.Model.Evaluations;
using Shapoco.Calctus.Model.Parsers;

namespace Shapoco.Calctus.UI.CLI {
    class Command {
        [DllImport("kernel32.dll")]
        public static extern bool AttachConsole(uint dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();

        public void Run(string[] args) {
            if (!AttachConsole(System.UInt32.MaxValue)) return;

            using (var coutStream = Console.OpenStandardOutput())
            using (var cout = new StreamWriter(coutStream)) {
                try {
                    var exprStr = args[1];
                    var expr = Parser.Parse(exprStr);
                    var e = new EvalContext();
                    cout.WriteLine(expr.Eval(e).AsString);
                }
                catch (Exception ex) {
                    cout.WriteLine("*ERROR: " + ex.Message);
                }
                cout.Flush();
            }

            FreeConsole();
        }
    }
}
