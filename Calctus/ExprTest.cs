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
            Test.AssertEqual(e, TestExpr, ExpectedStr);
        }
#endif
    }
}
