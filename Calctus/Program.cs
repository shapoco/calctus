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
            Shapoco.Calctus.Parser.Lexer.Test("1+2*3/4//5");
            Shapoco.Calctus.Parser.Parser.Test("2");
            Shapoco.Calctus.Parser.Parser.Test("2+3");
            Shapoco.Calctus.Parser.Parser.Test("2+3*4");
            Shapoco.Calctus.Parser.Parser.Test("2*3+4");
            Shapoco.Calctus.Parser.Parser.Test("(2+3)*4");
            Shapoco.Calctus.Parser.Parser.Test("2*(3+4)");
            Model.Standard.PreferredNumbers.Test();
#endif
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new UI.MainForm());
            Settings.Instance.Save();
        }
    }
}
