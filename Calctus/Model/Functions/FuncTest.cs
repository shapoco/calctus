namespace Shapoco.Calctus.Model.Functions {
    class FuncTest {
        public readonly string ArgListStr;
        public readonly string ExpectedStr;

        public FuncTest(string argListStr, string expectedStr) {
            this.ArgListStr = argListStr;
            this.ExpectedStr = expectedStr;
        }

        public ExprTest GenTest(FuncDef func) {
            return new ExprTest(func.Name.Text + "(" + ArgListStr + ")", ExpectedStr);
        }
    }
}
