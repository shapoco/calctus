using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Shapoco.Calctus.Model;
using Shapoco.Calctus.Parser;

namespace Shapoco.Calctus.UI {
    public partial class MainForm : Form {
        private EvalContext _ctx = new EvalContext();
        private char[] _selectionCancelChars;

        public MainForm() {
            // 全選択された状態で入力されたら選択を解除する文字の一覧
            _selectionCancelChars = OpDef.AllSymbols
                .Select(p => p[0])
                .Distinct()
                .ToArray();

            InitializeComponent();

            try {
                exprBox.Font = new Font("Consolas", exprBox.Font.Size);
                logBox.Font = new Font("Consolas", logBox.Font.Size);
            } 
            catch { }

            this.Text = Application.ProductName + " (v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + ")"; 
            exprBox.AutoSize = false;
            exprBox.Dock = DockStyle.Fill;
            exprBox.KeyPress += ExprBox_KeyPress;
            exprBox.TextChanged += ExprBox_TextChanged;
            calcButton.Click += CalcButton_Click;
            subAnswerLabel.Text = "";
            this.Font = new Font("Meiryo UI", SystemFonts.DefaultFont.Size);
            exprBox.Font = new Font("Meiryo UI", SystemFonts.DefaultFont.Size * 1.5f);
        }

        private void ExprBox_TextChanged(object sender, EventArgs e) {
            try {
                var exprStr = exprBox.Text;
                var expr = Parser.Parser.Parse(exprStr);
                var val = expr.Eval(_ctx);
                var text = "= " + val.ToString();
                if (val is RealVal rval) {
                    if (rval.IsDimless) {
                        var dval = rval.AsDouble;
                        if (rval.IsInteger && 0xA <= dval && dval < UInt32.MaxValue) {
                            var rvalInt = rval.FormatInt();
                            var rvalHex = rval.FormatHex();
                            text = "= " + rvalInt.ToString() +  " (" + rvalHex.ToString() + ")";
                        }
                        else {
                            text += " (無次元)";
                        }
                    }
                    else
                        text += " (raw=" + rval.Raw + ")";
                }
                subAnswerLabel.Text = text;
            }
            catch (Exception ex) {
                subAnswerLabel.Text = ex.Message;
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        private void ExprBox_KeyPress(object sender, KeyPressEventArgs e) {
            if (_selectionCancelChars.Contains(e.KeyChar)) {
                if (exprBox.SelectionStart == 0 && exprBox.SelectionLength == exprBox.TextLength) {
                    // 全選択された状態で演算子が入力された選択を解除して末尾にカーソル移動
                    exprBox.SelectionStart = exprBox.TextLength;
                }
            }
            else if (e.KeyChar == (char)Keys.Return) {
                e.Handled = true;
                calcButton.PerformClick();
            }
        }

        private void CalcButton_Click(object sender, EventArgs e) {
            try {
                var exprStr = exprBox.Text;
                var expr = Parser.Parser.Parse(exprStr);
                var val = expr.Eval(_ctx);
                
                exprBox.Text = val.ToString();
                //exprBox.SelectionStart = exprBox.TextLength;
                exprBox.SelectAll();

                if (logBox.TextLength > 0) {
                    logBox.Text += "\r\n";
                }
                if (expr is BinaryOp binOpExpr && binOpExpr.Method == OpDef.Assign && binOpExpr.B is Literal) {
                    logBox.Text += exprStr;
                }
                else {
                    logBox.Text += exprStr + " = " + val.ToString();
                }
                logBox.SelectionStart = logBox.TextLength;
                logBox.ScrollToCaret();
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
