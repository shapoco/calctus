using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Shapoco.Calctus {
    static class Program {
#if DEBUG
        private static bool _debugModeFirstRef = true;
        private static bool _debugMode = true;
        public static bool DebugMode {
            get {
                if (_debugModeFirstRef) {
                    if (Control.ModifierKeys == Keys.Shift) {
                        _debugMode = false;
                    }
                    _debugModeFirstRef = false;
                }
                return _debugMode;
            }
        }
#else
        public const bool DebugMode = false;
#endif

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main(string[] args) {
#if DEBUG
            if (!DebugMode) {
                Log.Here().I("==== DEBUG MODE DISABLED ====");
            }
            Log.StartLogging();
#endif
            Log.Here().I(Application.ProductName + " ver." + Application.ProductVersion);

            bool cliMode = (args.Length >= 2) && (args[0] == "-c");
            if (cliMode) {
                var cli = new UI.CLI.Command();
                cli.Run(args);
            }
            else {
#if DEBUG
                DocumentGenerator.WriteVersion();

                //Model.Formats.ValFormat.Test();
                Model.Standards.PreferredNumbers.Test();
                Maths.ufixed113.Test();
                Maths.quad.Test();
                Maths.QuadMath.Test();
                Model.Functions.BuiltInFuncLibrary.Instance.DoTest();
                Model.Values.ApFixedVal.DoTest();
                DocumentGenerator.GenerateDocumentRst();
                DocumentGenerator.GenerateDocumentation();
#endif
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new UI.MainForm());
                Settings.Instance.Save();
            }

            Log.Here().I("Exiting...");
            Log.EndLogging();
        }
    }
}
