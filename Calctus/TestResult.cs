using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus {
    class TestResult {
        public readonly bool Success;
        public readonly string Expr;
        public readonly string ExpectedValue;
        public readonly string ActualValue;

        public TestResult(bool success, string expr, string exp, string act) {
            this.Success = success;
            this.Expr = expr;
            this.ExpectedValue = exp;
            this.ActualValue = act;
        }
    }
}
