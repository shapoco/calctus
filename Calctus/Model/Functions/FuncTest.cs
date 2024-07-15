using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Functions {
    class FuncTest {
        public readonly string ArgListStr;
        public readonly string ExpectedStr;
        public TestResult Result { get; private set; } = null;
        public bool Done => this.Result != null;
        public bool Success => this.Done && Result.Success;

        public FuncTest(string argListStr, string expectedStr) {
            this.ArgListStr = argListStr;
            this.ExpectedStr = expectedStr;
        }

#if DEBUG
        public void DoTest(EvalContext e, FuncDef f) {
            var exprStr = f.Name.Text + "(" + ArgListStr + ")";
            try {
                var expVal = Parser.Parse(ExpectedStr).Eval(e);
                var actVal = Parser.Parse(exprStr).Eval(e);
                var actStr = actVal.ToString();

                bool success = false;
                if (expVal is ArrayVal expArrayVal) {
                    var expArray = (Val[])expArrayVal.Raw;
                    var actArray = (Val[])actVal.Raw;
                    if (actArray.Length == expArray.Length) {
                        success = true;
                        for (int i = 0; i < actArray.Length; i++) {
                            if (!actArray[i].Equals(e, expArray[i]).AsBool) {
                                success = false;
                                break;
                            }
                        }
                    }
                }
                else {
                    success = actVal.Equals(e, expVal).AsBool;
                }
                
                if (success) {
                    Console.WriteLine("test succes: " + exprStr + " == " + ExpectedStr);
                }
                else {
                    Console.WriteLine("TEST FAILED: " + exprStr + " != " + ExpectedStr + ", act: " + actStr);
                }
                this.Result = new TestResult(success, exprStr, ExpectedStr, actStr);
            }
            catch (Exception ex) {
                Console.WriteLine("TEST FAILED: " + exprStr + " : " + ex.Message);
                this.Result = null;
            }
        }
#endif
    }
}
