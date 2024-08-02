using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Evaluations;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Values;

namespace Shapoco.Calctus {
    class ExprTest {
        public readonly string TestExpr;
        public readonly string ExpectedStr;
        public TestResult Result { get; private set; } = null;
        public bool Done => this.Result != null;
        public bool Success => this.Done && Result.Success;

        public ExprTest(string testExpr, string expectedStr) {
            this.TestExpr = testExpr;
            this.ExpectedStr = expectedStr;
        }

#if DEBUG
        public void DoTest(EvalContext e) {
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
                    Test.Success("test succes: " + TestExpr + " == " + ExpectedStr);
                }
                else {
                    Test.Error("TEST FAILED: " + TestExpr + " != " + ExpectedStr + ", act: " + actStr);
                }
                this.Result = new TestResult(success, TestExpr, ExpectedStr, actStr);
            }
            catch (Exception ex) {
                Test.Error("TEST FAILED: " + TestExpr + " : " + ex.Message);
                this.Result = null;
            }
        }
#endif
    }
}
