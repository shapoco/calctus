using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus {
#if DEBUG
    static class Test {
        public static int NumSuccess { get; private set; } = 0;
        public static int NumUntested { get; private set; } = 0;
        public static int NumWarnings { get; private set; } = 0;
        public static int NumErrors { get; private set; } = 0;

        public static void Success(string msg) {
            NumSuccess += 1;
            //WriteLine("*I: " + msg);
        }

        public static void Untested(string msg) {
            NumUntested += 1;
            WriteLine("*U: " + msg);
        }

        public static void Warning(string msg) {
            NumWarnings += 1;
            WriteLine("*W: " + msg);
        }

        public static void Error(string msg) {
            NumErrors += 1;
            WriteLine("*E: " + msg);
        }

        public static void WriteLine(string msg) {
            Console.WriteLine(msg);
        }
    }
#endif
}
