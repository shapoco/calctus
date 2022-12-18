using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Shapoco.Calctus.Model;

namespace Shapoco.Calctus.UI {
    class CalcListItem : ContainerControl {
        private static readonly char[] _selectionCancelChars = OpDef.AllSymbols
                .Select(p => p[0])
                .Distinct()
                .ToArray();

        public event EventHandler ExpressionChanged;
        public event KeyEventHandler ItemKeyDown;
        public event KeyEventHandler ItemKeyUp;
        public event MouseEventHandler ItemMouseUp;
        public event EventHandler ItemGotFocus;
        public event EventHandler ItemLostFocus;

        private ExpressionBox _exprBox = new ExpressionBox();
        private ExpressionBox _ansBox = new ExpressionBox();
        private Label _equal = new Label();
        private CalcListBox _owner;
        private bool _selected = false;
        private bool _isFreshAnswer = false;

        public CalcListItem(CalcListBox owner) {
            _owner = owner;
            this.RadixMode = owner.RadixMode;
            _ansBox.PlaceHolder = "?";
            _ansBox.ReadOnly = true;
            _exprBox.KeyDown += box_PreviewKeyDown;
            _exprBox.KeyUp += box_KeyUp;
            _exprBox.KeyPress += exprBox_KeyPress;
            _exprBox.TextChanged += exprBox_TextChanged;
            _exprBox.GotFocus += box_GotFocus;
            _exprBox.LostFocus += box_LostFocus;
            _exprBox.MouseUp += ctrl_MouseUp;
            _ansBox.KeyDown += box_PreviewKeyDown;
            _ansBox.GotFocus += box_GotFocus;
            _ansBox.LostFocus += box_LostFocus;
            _ansBox.MouseUp += ctrl_MouseUp;
            _equal.Text = "=";
            _equal.TextAlign = ContentAlignment.MiddleRight;
            _equal.AutoSize = false;
            _equal.Click += (sender, e) => { _ansBox.Focus(); };
            _equal.MouseUp += ctrl_MouseUp;
            this.Controls.Add(_exprBox);
            this.Controls.Add(_ansBox);
            this.Controls.Add(_equal);
        }

        public CalcListItem(CalcListBox owner, CalcListItem prevItem) : this(owner) {
            this.RadixMode = prevItem.RadixMode;
            _exprBox.Text = prevItem.Answer;
            _ansBox.Text = prevItem.Answer;
            _isFreshAnswer = true;
        }

        public RadixMode RadixMode { get; set; } = RadixMode.Auto;

        public void OnSelected() {
            if (_selected) return;
            _selected = true;
            _exprBox.BackColor = Color.Black;
            _ansBox.BackColor = Color.Black;
            _equal.BackColor = Color.Black;
            if (!_exprBox.Focused && !_ansBox.Focused) {
                _exprBox.Focus();
            }
        }

        public void OnDeselected() {
            if (!_selected) return;
            _selected = false;
            _exprBox.BackColor = _owner.BackColor;
            _ansBox.BackColor = _owner.BackColor;
            _equal.BackColor = _owner.BackColor;
        }

        public bool IsTextCuttable =>
            (_exprBox.Focused && _exprBox.SelectionLength > 0);

        public bool IsTextCopiable =>
            (_exprBox.Focused && _exprBox.SelectionLength > 0) ||
            (_ansBox.Focused && _ansBox.SelectionLength > 0);

        public bool IsTextPastable =>
            _exprBox.Focused;

        public void OnCutText() {
            if (_exprBox.Focused) {
                _exprBox.Cut();
            }
        }

        public void OnCopyText() {
            if (_exprBox.Focused) {
                _exprBox.Copy();
            }
            else if (_ansBox.Focused) {
                _ansBox.Copy();
            }
        }

        public void OnPasteText() {
            if (_exprBox.Focused) {
                _exprBox.Paste();
            }
        }

        public void OnDeleteText() {
            if (_exprBox.Focused) {
                _exprBox.SelectedText = "";
            }
        }

        public string Expression {
            get => _exprBox.Text;
            set => _exprBox.Text = value;
        }

        public string Answer {
            get => _ansBox.Text;
            set => _ansBox.Text = value;
        }

        public string Hint {
            get => _ansBox.PlaceHolder;
            set => _ansBox.PlaceHolder = value;
        }

        public override Size GetPreferredSize(Size proposedSize) {
            if (proposedSize.Width == 0) proposedSize.Width = int.MaxValue;
            var client = this.ClientSize;
            var indent = getAnswerIndent(proposedSize);
            var expr = _exprBox.PreferredSize;
            var equal = _equal.PreferredSize;
            var ans = _ansBox.PreferredSize; 
            if (expr.Width < indent - equal.Width) {
                // 式が短い場合は横並びで計算する
                return new Size(expr.Width + equal.Width + ans.Width, Math.Max(expr.Height, ans.Height));
            }
            else {
                // 式が長い場合は縦並びにする
                return new Size(Math.Max(expr.Width, ans.Width), expr.Height + ans.Height);
            }
        }

        public void Relayout() {
            var client = this.ClientSize;
            var indent = getAnswerIndent(client);
            var expr = _exprBox.PreferredSize;
            var equal = _equal.PreferredSize;
            var ans = _ansBox.PreferredSize;
            if (expr.Width < indent - equal.Width) {
                // 式が短い場合は横並びで計算する
                _exprBox.SetBounds(0, 0, indent - equal.Width, client.Height);
                _equal.SetBounds(indent - equal.Width, 0, equal.Width, client.Height);
                _ansBox.SetBounds(indent, 0, client.Width - indent, client.Height);
            }
            else {
                // 式が長い場合は縦並びにする
                _exprBox.SetBounds(0, 0, client.Width, client.Height / 2);
                _equal.SetBounds(0, client.Height / 2, indent, client.Height / 2);
                _ansBox.SetBounds(indent, client.Height / 2, client.Width - indent, client.Height / 2);
            }
        }

        protected override void OnResize(EventArgs eventargs) {
            base.OnResize(eventargs);
            this.Relayout();
        }

        private int getAnswerIndent(Size client) {
            return Math.Max(50, client.Width / 3);
        }

        private void exprBox_TextChanged(object sender, EventArgs e) {
            ExpressionChanged?.Invoke(this, EventArgs.Empty);
            _isFreshAnswer = false;
        }

        private void exprBox_KeyPress(object sender, KeyPressEventArgs e) {
            var box = (ExpressionBox)sender;
            var allSelected = (box.SelectionStart == 0) && (box.SelectionLength == box.Text.Length);
            if (allSelected && _isFreshAnswer && _selectionCancelChars.Contains(e.KeyChar)) {
                box.SelectedText = "Ans";
                box.SelectionStart = box.Text.Length;
                box.SelectionLength = 0;
                e.Handled = true;
            }
        }

        private void box_PreviewKeyDown(object sender, KeyEventArgs e) {
            ItemKeyDown?.Invoke(this, e);
        }

        private void box_KeyUp(object sender, KeyEventArgs e) {
            ItemKeyUp?.Invoke(this, e);
        }

        private void box_GotFocus(object sender, EventArgs e) {
            var box = (ExpressionBox)sender;
            box.SelectAll();
            ItemGotFocus?.Invoke(this, e);
        }

        private void box_LostFocus(object sender, EventArgs e) {
            var box = (ExpressionBox)sender;
            ItemLostFocus?.Invoke(this, e);
        }

        private void ctrl_MouseUp(object sender, MouseEventArgs e) {
            var ctrl = (Control)sender;
            var x = e.X + ctrl.Left;
            var y = e.Y + ctrl.Top;
            var args = new MouseEventArgs(e.Button, e.Clicks, x, y, e.Delta);
            ItemMouseUp?.Invoke(this, args);
        }
    }
}
