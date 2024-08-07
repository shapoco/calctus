using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Evaluations;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Values;

namespace Shapoco.Calctus {
    static class Test {
        public static void AssertEqual(EvalContext e, string TestExpr, string ExpectedStr ) {
            try {
                var expVal = Parser.Parse(ExpectedStr).Eval(e);
                var actVal = Parser.Parse(TestExpr).Eval(e);
                var actStr = actVal.ToStringForLiteral();

                bool success = false;
                if (expVal is ListVal expArrayVal) {
                    var expArray = (Val[])expArrayVal.Raw;
                    var actArray = (Val[])actVal.Raw;
                    if (actArray.Length == expArray.Length) {
                        success = true;
                        for (int i = 0; i < actArray.Length; i++) {
                            if (!actArray[i].Equals(e, expArray[i])) {
                                success = false;
                                break;
                            }
                        }
                    }
                }
                else {
                    success = actVal.Equals(e, expVal);
                }

                if (success) {
                    Success(TestExpr + " == " + ExpectedStr);
                }
                else {
                    Fail(TestExpr + " != " + ExpectedStr + ", act: " + actStr);
                }
            }
            catch (Exception ex) {
                Fail(TestExpr + " : " + ex.Message);
            }
        }

        public static int NumSuccess { get; private set; } = 0;
        public static int NumUntested { get; private set; } = 0;
        public static int NumWarnings { get; private set; } = 0;
        public static int NumErrors { get; private set; } = 0;

        public static void Success(string msg) {
            NumSuccess += 1;
            Log.Here().I(msg);
        }

        public static void Untested(string msg) {
            NumUntested += 1;
            Log.Here().W(msg);
        }

        public static void Warning(string msg) {
            NumWarnings += 1;
            Log.Here().W(msg);
        }

        public static void Fail(string msg) {
            NumErrors += 1;
            Log.Here().E(msg);
        }
    }
}
