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
        static void Main()
        {
#if DEBUG
            //Model.Formats.ValFormat.Test();
            Model.Standards.PreferredNumbers.Test();
            Model.Types.ufixed113.Test();
            Model.Types.quad.Test();
            Model.Mathematics.QMath.Test();
            Model.Functions.BuiltInFuncDef.GenerateDocumentation();
#endif
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new UI.MainForm());
            Settings.Instance.Save();
        }
    }
}
