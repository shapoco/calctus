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
    internal partial class MainForm : Form {
        private char[] _selectionCancelChars;
        private bool _suppressListIndexChangedEvent = false;
        private RadixMode _radixMode = RadixMode.Auto;

        class CustomProfessionalColors : ProfessionalColorTable {
            public override Color ToolStripGradientBegin { get { return Color.FromArgb(64, 64, 64); } }
            public override Color ToolStripGradientMiddle { get { return Color.FromArgb(56, 56, 56); } }
            public override Color ToolStripGradientEnd { get { return Color.FromArgb(48, 48, 48); } }
            public override Color ToolStripBorder { get { return Color.FromArgb(64, 64, 64); } }
        }

        public MainForm() {
            // 全選択された状態で入力されたら選択を解除する文字の一覧
            _selectionCancelChars = OpDef.AllSymbols
                .Select(p => p[0])
                .Distinct()
                .ToArray();

            InitializeComponent();
            if (this.DesignMode) return;

            ToolStripManager.Renderer = new ToolStripProfessionalRenderer(new CustomProfessionalColors());

            historyBox.Items.Add(new HistoryItem());
            historyBox.SelectedIndex = 0;
            historyBox.SelectedIndexChanged += HistoryBox_SelectedIndexChanged;

            try {
                exprBox.Font = new Font("Consolas", exprBox.Font.Size);
            } 
            catch { }

            this.Text = Application.ProductName + " (v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + ")";
            this.KeyPreview = true;
            this.KeyDown += MainForm_KeyDown;

            exprBox.AutoSize = false;
            exprBox.Dock = DockStyle.Fill;
            exprBox.KeyPress += ExprBox_KeyPress;
            exprBox.TextChanged += ExprBox_TextChanged;
            exprBox.KeyDown += ExpressionBox_KeyDown;
            
            calcButton.Click += CalcButton_Click;
            
            radixAutoButton.CheckedChanged += (s, e) => { RadixButtonClicked((RadioButton)s, RadixMode.Auto); };
            radixDecButton.CheckedChanged += (s, e) => { RadixButtonClicked((RadioButton)s, RadixMode.Dec); };
            radixHexButton.CheckedChanged += (s, e) => { RadixButtonClicked((RadioButton)s, RadixMode.Hex); };
            radixBinButton.CheckedChanged += (s, e) => { RadixButtonClicked((RadioButton)s, RadixMode.Bin); };
            radixAutoButton.Checked = true;

            settingsButton.Click += delegate { new SettingsDialog().ShowDialog(); };
            helpButton.Click += delegate { System.Diagnostics.Process.Start(@"https://github.com/shapoco/calctus"); };

            subAnswerLabel.Text = "";
            try {
                this.Font = new Font("Meiryo UI", SystemFonts.DefaultFont.Size);
                historyBox.Font = new Font("Consolas", SystemFonts.DefaultFont.Size * 1.25f);
                exprBox.Font = new Font("Consolas", SystemFonts.DefaultFont.Size * 1.5f);
                subAnswerLabel.Font = new Font("Consolas", SystemFonts.DefaultFont.Size);
            }
            catch { }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e) {
            e.SuppressKeyPress = true;
            switch(e.KeyCode) {
                case Keys.F9: this.RadixMode = RadixMode.Auto; break;
                case Keys.F10: this.RadixMode = RadixMode.Dec; break;
                case Keys.F11: this.RadixMode = RadixMode.Hex; break;
                case Keys.F12: this.RadixMode = RadixMode.Bin; break;
                default: e.SuppressKeyPress = false; break;
            }
        }

        private void RadixButtonClicked(RadioButton btn, RadixMode mode) {
            if (btn.Checked) {
                this.RadixMode = mode;
            }
            exprBox.Focus();
        }

        public RadixMode RadixMode {
            get => _radixMode;
            set {
                if (value == _radixMode) return;
                _radixMode = value;
                switch(value) {
                    case RadixMode.Auto: radixAutoButton.Checked = true; break;
                    case RadixMode.Dec: radixDecButton.Checked = true; break;
                    case RadixMode.Hex: radixHexButton.Checked = true; break;
                    case RadixMode.Bin: radixBinButton.Checked = true; break;
                }
                Recalc();
            }
        }

        private void HistoryBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (historyBox.SelectedIndex < 0) return;
            if (_suppressListIndexChangedEvent) return;
            exprBox.Text = ((HistoryItem)historyBox.Items[historyBox.SelectedIndex]).Expression;
            exprBox.SelectAll();
        }

        private void ExprBox_TextChanged(object sender, EventArgs e) {
            Recalc();
        }

        private void ExprBox_KeyPress(object sender, KeyPressEventArgs e) {
            if (_selectionCancelChars.Contains(e.KeyChar)) {
                if (exprBox.SelectionLength > 0 && exprBox.SelectionStart == 0 && exprBox.SelectionLength == exprBox.TextLength) {
                    // 全選択された状態で演算子が入力された選択を解除して末尾にカーソル移動
                    exprBox.SelectedText = "Ans";
                    exprBox.SelectionStart = exprBox.TextLength;
                }
            }
            else if (e.KeyChar == (char)Keys.Return) {
                e.Handled = true;
            }
        }

        private void ExpressionBox_KeyDown(object sender, KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.Return:
                    OnReturnPressed(e.Shift);
                    e.Handled = true;
                    break;
                case Keys.Up:
                    if (historyBox.SelectedIndex > 0) {
                        historyBox.SelectedIndex -= 1;
                    }
                    e.Handled = true;
                    break;
                case Keys.Down:
                    if (historyBox.SelectedIndex < historyBox.Items.Count - 1) {
                        historyBox.SelectedIndex += 1;
                    }
                    e.Handled = true;
                    break;

            }
        }

        private void OnReturnPressed(bool insert) {
            // 数式をヒストリに設定
            if (historyBox.SelectedIndex >= 0) {
                ((HistoryItem)historyBox.Items[historyBox.SelectedIndex]).Expression = exprBox.Text;
            }

            // 次のアイテムを選択する
            int nextIndex;
            if (insert) {
                nextIndex = historyBox.SelectedIndex;
            }
            else {
                nextIndex = historyBox.SelectedIndex + 1;
            }
            if (nextIndex >= historyBox.Items.Count || insert) {
                historyBox.Items.Insert(nextIndex, new HistoryItem());
            }
            historyBox.SelectedIndex = nextIndex;
            Recalc();
            if (historyBox.SelectedIndex == historyBox.Items.Count - 1 && historyBox.Items.Count >= 2) {
                var lastItem = (HistoryItem)historyBox.Items[historyBox.SelectedIndex - 1];
                if (!lastItem.IsEmpty) {
                    exprBox.Text = lastItem.Answer;
                    exprBox.SelectAll();
                }
            }
        }

        private void Recalc() {
            EvalContext ctx = new EvalContext();
            for (int i = 0; i < historyBox.Items.Count; i++) {
                var item = (HistoryItem)historyBox.Items[i];
                try {
                    item.Answer = "";
                    string exprStr;
                    if (i == historyBox.SelectedIndex) {
                        exprStr = exprBox.Text;
                        item.RadixMode = this.RadixMode;
                    }
                    else {
                        exprStr = item.Expression;
                    }
                    var expr = Parser.Parser.Parse(exprStr);
                    var val = expr.Eval(ctx);

                    switch (item.RadixMode) {
                        case RadixMode.Dec: val = val.FormatInt(); break;
                        case RadixMode.Hex: val = val.FormatHex(); break;
                        case RadixMode.Bin: val = val.FormatBin(); break;
                    }

                    var valStr = val.ToString();
                    var hintStr = "";
                    if (val is RealVal realVal) {
                        if (realVal.IsDimless) {
                            var doubleVal = realVal.AsDouble;
                            if (realVal.IsInteger && (doubleVal < 0 || 10 <= doubleVal && doubleVal <= int.MaxValue)) {
                                // 10以上の整数については 10進と 16進を併記する
                                if (realVal.FormatHint.Formatter == Shapoco.Calctus.Model.Syntax.IntFormatter.CStyleInt) {
                                    hintStr = realVal.FormatHex().ToString();
                                }
                                else {
                                    hintStr = realVal.FormatInt().ToString();
                                }
                            }
                        }
                    }

                    item.Answer = valStr;
                    item.Error = false;
                    item.Hint = hintStr;
                    ctx.Ref("Ans", true).Value = val;

                    if (i == historyBox.SelectedIndex) {
                        subAnswerLabel.Text = "= " + valStr;
                    }
                }
                catch (Exception ex) {
                    item.Error = true;
                    if (i == historyBox.SelectedIndex) {
                        subAnswerLabel.Text = ex.Message;
                    }
                    item.Hint = ex.Message;
                }
            }
            historyBox.Invalidate();
        }

        private void CalcButton_Click(object sender, EventArgs e) {
            try {
                OnReturnPressed(false);
                exprBox.Focus();
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
