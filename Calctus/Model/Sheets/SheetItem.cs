using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Expressions;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Sheets {
    class SheetItem {
        public event EventHandler ExpressionChanged;
        public event EventHandler RadixModeChanged;
        public event EventHandler AnswerChanged;

        private string _exprText = null;
        
        private RadixMode _radixMode = RadixMode.Auto;
        //public Token[] Tokens { get; private set; }
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

        public RadixMode RadixMode {
            get => _radixMode;
            set {
                if (value == _radixMode) return;
                _radixMode = value;
                ExpressionChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void parse(string exprText) {
            try {
                // 字句解析
                var tokens = new Lexer(exprText).PopToEnd();

                // 不足している括弧の補完
                tokens.CompleteParentheses();

                try {
                    // 構文解析
                    var exprObj = Parser.Parse(tokens);
                    SetExprTree(exprText, exprObj, null);
                }
                catch (Exception ex) {
                    // 構文解析エラー
                    SetExprTree(exprText, Expr.Empty, ex);
                }
            }
            catch (Exception ex) {
                // 字句解析エラー
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
                    var val = ExprTree.Eval(e);
                    switch (RadixMode) {
                        case RadixMode.Dec: val = val.FormatInt(); break;
                        case RadixMode.Hex: val = val.FormatHex(); break;
                        case RadixMode.Bin: val = val.FormatBin(); break;
                        case RadixMode.Oct: val = val.FormatOct(); break;
                    }
                    SetStatus(val, null, null);
                }
                catch (Exception ex) {
                    SetStatus(NullVal.Instance, null, ex);
                }
            }
        }

        public void SetStatus(Val ans, Exception syntaxError, Exception evalError) {
            string ansText = ans.ToString();
            if (ans.Equals(AnsVal) && ansText != AnsText && syntaxError == SyntaxError && evalError == EvalError) return;
            AnsVal = ans;
            AnsText = ansText;
            SyntaxError = syntaxError;
            EvalError = evalError;
            AnswerChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
