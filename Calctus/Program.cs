using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Shapoco.Calctus
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            bool cliMode = (args.Length >= 2) && (args[0] == "-c");
            if (cliMode) {
                var cli = new UI.CLI.Command();
                cli.Run(args);
            }
            else {
#if DEBUG
                //Model.Formats.ValFormat.Test();
                Model.Standards.PreferredNumbers.Test();
                Model.Maths.Types.ufixed113.Test();
                Model.Maths.Types.quad.Test();
                Model.Maths.QMath.Test();
                Model.Functions.BuiltInFuncLibrary.Instance.DoTest();
                DocumentGenerator.GenerateDocumentation();
#endif
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new UI.MainForm());
                Settings.Instance.Save();
            }
        }
    }
}
