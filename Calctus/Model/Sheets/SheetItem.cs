using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model;
using Shapoco.Calctus.Model.Formats;
using Shapoco.Calctus.Model.Values;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Expressions;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Sheets {
    class SheetItem {
        public event EventHandler ExpressionChanged;
        public event EventHandler AnswerChanged;

        private string _exprText = null;
        
        public Expr ExprTree { get; private set; }
        public Val AnsVal { get; private set; }
        public string AnsText { get; private set; } = "";
        public Exception SyntaxError { get; private set; }
        public Exception EvalError { get; private set; }

        public SheetItem(string expr = "") {
            ExprText = expr;
        }

        public object Tag = null;

        public string ExprText {
            get => _exprText;
            set {
                if (value == _exprText) return;
                parse(value);
            }
        }

        private void parse(string exprText) {
            try {
                // 字句解析
                var tokens = new Parsers.Lexer(exprText).PopToEnd();

                // 不足している括弧の補完
                tokens.CompleteParentheses();

                try {
                    // 構文解析
                    var exprObj = Parser.Parse(tokens);
                    SetExprTree(exprText, exprObj, null);
                }
                catch (Exception ex) {
                    // 構文解析エラー
                    Log.Here().W(ex);
                    SetExprTree(exprText, Expr.Empty, ex);
                }
            }
            catch (Exception ex) {
                // 字句解析エラー
                Log.Here().W(ex);
                SetExprTree(exprText, Expr.Empty, ex);
            }
        }

        public void SetExprTree(string exprText, Expr exprTree, Exception err) {
            _exprText = exprText;
            ExprTree = exprTree;
            SyntaxError = err;
            AnsVal = NullVal.Instance;
            EvalError = null;
            ExpressionChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Eval(EvalContext e) {
            if (SyntaxError != null) {
                SetStatus(NullVal.Instance, SyntaxError, null);
            }
            else {
                try {
                    SetStatus(ExprTree.Eval(e), null, null);
                }
                catch (Exception ex) {
                    Log.Here().W(ex);
                    SetStatus(NullVal.Instance, null, ex);
                }
            }
        }

        public void SetStatus(Val ans, Exception syntaxError, Exception evalError) {
            string ansText;
            ansText = ans.ToString(ToStringArgs.ForLiteral());
            if (ans.Equals(AnsVal) && ansText == AnsText && syntaxError == SyntaxError && evalError == EvalError) return;
            AnsVal = ans;
            AnsText = ansText;
            SyntaxError = syntaxError;
            EvalError = evalError;
            AnswerChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
