using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Shapoco.Calctus.Model;
using Shapoco.Calctus.Model.Formats;
using Shapoco.Calctus.Model.Standards;
using Shapoco.Calctus.Model.Expressions;

namespace Shapoco.Calctus.UI.Sheets {
    class ExprBoxCore : GdiBox {
        public event EventHandler TextChanged;

        private GdiControl _owner;

        private ExprBoxCoreEdit _edit = new ExprBoxCoreEdit();
        private ExprBoxCoreLayout _layout;
        private int _scrollX = 0;
        private MouseButtons _pressedMouseButtons = MouseButtons.None;
        private Keys _pressedModifiers = Keys.None;

        private string _placeHolder = "";

        private static Timer _cursorBlinkTimer = new Timer();
        private bool _cursorVisible = false;

        public ExprBoxCore(GdiControl owner) : base(owner) {
            _owner = owner;
            Focusable = true;
            Cursor = Cursors.IBeam;
            _layout = new ExprBoxCoreLayout(owner);
            _edit.TextChanged += Edit_TextChanged;
            _edit.CursorStateChanged += Edit_CursorStateChanged;
            _edit.QueryScreenCursorLocation += Edit_QueryScreenCursorLocation;
            _edit.QueryToken += _edit_QueryToken;
            _layout.Layout(Text);
        }

        public string Text {
            get => _edit.Text;
            set => _edit.Text = value;
        }

        public string PlaceHolder {
            get => _placeHolder;
            set {
                if (value == _placeHolder) return;
                _placeHolder = value;
                Invalidate();
            }
        }

        public Expr ExprObject => _layout.ExprObject;

        public Exception SynstaxError => _layout.SyntaxError;

        public Exception EvalError {
            get => _layout.EvalError;
            set => _layout.EvalError = value;
        }

        public bool ReadOnly {
            get => _edit.ReadOnly;
            set => _edit.ReadOnly = value;
        }

        public int SelectionStart {
            get => _edit.SelectionStart;
            set => _edit.SetSelection(value);
        }

        public int SelectionLength {
            get => _edit.SelectionLength;
            set => _edit.SetSelection(_edit.SelectionStart, _edit.SelectionStart + value);
        }

        public string SelectedText {
            get => _edit.SelectedText;
            set => _edit.SelectedText = value;
        }

        public IInputCandidateProvider InputCandidateProvider {
            get => _edit.CandidateProvider;
            set => _edit.CandidateProvider = value;
        }

        public void SelectAll() => _edit.SelectAll();
        public void Cut() => _edit.Cut();
        public void Copy() => _edit.Copy();
        public void Paste() => _edit.Paste();
        public void Delete() {
            if (!ReadOnly) {
                _edit.SelectedText = "";
            }
        }
        public void InsertToday() {
            if (!ReadOnly) {
                CandidateHide();
                _edit.SelectedText = DateTimeFormat.FormatAsStringLiteral(UnixTime.Today, true);
            }
        }
        public void InsertCurrentTime() {
            if (!ReadOnly) {
                CandidateHide();
                _edit.SelectedText = DateTimeFormat.FormatAsStringLiteral(UnixTime.Now, true);
            }
        }

        public void CandidateHide() => _edit.CandidatesHide();

        public override Size GetPreferredSize() => _layout.PreferredSize;

        public override Point GetCursorPosition() => getCursorLocation();

        protected override void OnGotFocus() {
            base.OnGotFocus();
            SelectAll();
            _cursorBlinkTimer.Tick += CursorBlinkTimer_Tick;
            _cursorBlinkTimer.Interval = 500;
            restartCursorBlink();
        }

        protected override void OnLostFocus() {
            base.OnLostFocus();
            _cursorVisible = false;
            _cursorBlinkTimer.Tick -= CursorBlinkTimer_Tick;
            _pressedModifiers = Keys.None;
            _edit.CandidatesHide();
            Invalidate();
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);
            _pressedModifiers = e.Modifiers;
            _edit.OnKeyDown(e);
            restartCursorBlink();
        }

        protected override void OnKeyUp(KeyEventArgs e) {
            base.OnKeyUp(e);
            _pressedModifiers = e.Modifiers;
            _edit.OnKeyUp(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e) {
            base.OnKeyPress(e);
            _edit.OnKeyPress(e);
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            _pressedMouseButtons |= e.Button;
            if (e.Button == MouseButtons.Left) {
                if (_pressedModifiers == Keys.Shift) {
                    _edit.SetSelection(_edit.SelectionOrigin, xToCursorPos(e.X));
                }
                else {
                    _edit.SetSelection(xToCursorPos(e.X));
                }
                restartCursorBlink();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            if (_pressedMouseButtons == MouseButtons.Left) {
                _edit.SetSelection(_edit.SelectionOrigin, xToCursorPos(e.X));
            }
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);
            _pressedMouseButtons &= ~e.Button;
        }

        protected override void OnDoubleClick(EventArgs e) {
            base.OnDoubleClick(e);
            SelectAll();
        }

        private void Edit_TextChanged(object sender, EventArgs e) {
            _layout.Layout(_edit.Text);
            Invalidate();
            TextChanged?.Invoke(this, EventArgs.Empty);
        }

        private void Edit_CursorStateChanged(object sender, EventArgs e) {
            scrollToCursorPos(_edit.CursorPos);
            Invalidate();
        }

        private void Edit_QueryScreenCursorLocation(object sender, QueryScreenCursorLocationEventArgs e) {
            e.Result = PointToScreen(new Point(cursorPosToX(e.CursorPosition), _layout.CharHeight));
        }

        private void _edit_QueryToken(object sender, QueryTokenEventArgs e) {
            e.Result = _layout.GetTokenAt(e.CursorPosition, e.TokenType);
        }

        public void RelayoutText() => _layout.Layout(Text);

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            var g = e.Graphics;

            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            if (string.IsNullOrEmpty(Text)) {
                // 空欄の場合プレースホルダを表示
                using (var brush = new SolidBrush(Color.FromArgb(128, _owner.ForeColor))) {
                    g.DrawString(_placeHolder, _owner.Font, brush, _scrollX, 0);
                }
            }
            else {
                // 空欄でない場合、レイアウトされた文字列を表示
                _layout.Paint(g, new Point(_scrollX, 0));
            }

            if (this.Focused && this.SelectionLength != 0) {
                // 選択範囲の描画
                using (var brush = new SolidBrush(Color.FromArgb(128, Settings.Instance.Appearance_Color_Selection))) {
                    g.FillRectangle(brush, getSelectionRectangle());
                }
            }

            if (Focused && _cursorVisible) {
                // カーソルの描画
                using (var brush = new SolidBrush(_owner.ForeColor)) {
                    g.FillRectangle(brush, getCursorRectangle());
                }
            }
        }

        protected override void OnResize() => adjustScroll();

        private int xToCursorPos(int x) => _layout.XtoCursorPos(x - _scrollX);
        private int cursorPosToX(int cursorPos) => _scrollX + _layout.CursorPosToX(cursorPos);

        /// <summary>現在のカーソルの位置を返す</summary>
        private Point getCursorLocation() {
            return new Point(cursorPosToX(_edit.CursorPos), 0);
        }

        /// <summary>現在のカーソルの矩形を返す</summary>
        private Rectangle getCursorRectangle() {
            var cursorLoc = getCursorLocation();
            return new Rectangle(
                Math.Max(1, cursorLoc.X - 1),
                cursorLoc.Y, 
                2,
                _layout.CharHeight);
        }

        /// <summary>選択領域の矩形を返す</summary>
        private Rectangle getSelectionRectangle() {
            int selStart = SelectionStart;
            int selEnd = SelectionStart + SelectionLength;
            if (selStart == selEnd) {
                return Rectangle.Empty;
            }
            int x0 = cursorPosToX(selStart);
            int x1 = cursorPosToX(selEnd);
            return new Rectangle(x0, 0, x1 - x0, _layout.CharHeight);
        }

        private void scrollToCursorPos(int cursorPos) {
            var x = cursorPosToX(cursorPos);
            var right = Width - 1;
            if (x < 0) {
                _scrollX -= x;
            }
            else if (x >= right) {
                _scrollX += (right - x);
            }
            Invalidate();
        }

        private void adjustScroll() {
            var text = this.Text;
            if (text.Length > 0) {
                var rightX = _layout.CursorPosToX(text.Length);
                var minScrollX = Math.Min(0, Width - rightX - 1);
                _scrollX = Math.Max(minScrollX, _scrollX);
            }
            else {
                _scrollX = 0;
            }
            Invalidate();
        }

        private void restartCursorBlink() {
            if (!this.Focused) return;
            _cursorVisible = true;
            _cursorBlinkTimer.Stop();
            _cursorBlinkTimer.Start();
            Invalidate();
        }

        private void CursorBlinkTimer_Tick(object sender, EventArgs e) {
            _cursorVisible = !_cursorVisible;
            Invalidate();
        }
    }
}
