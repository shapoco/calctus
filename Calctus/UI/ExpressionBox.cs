using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Shapoco.Calctus.UI {
    class ExpressionBox : Control {
        public const int TextMargin = 0;

        public static readonly Regex RegexSymbols = new Regex(@"[+\-*/%^|&=<>]");
        public static readonly Regex RegexHexBinNumbers = new Regex(@"\b(0x[0-9a-fA-F]+|0b[01]+)\b");
        public static readonly Regex RegexIDs = new Regex(@"\b[a-zA-Z_][a-zA-Z0-9_]*\b");
        public static readonly Regex RegexColors = new Regex(@"#([0-9a-fA-F]{6}|[0-9a-fA-F]{3})\b");

        public static readonly Color ColorId = Color.FromArgb(192, 255, 128);
        public static readonly Color ColorSymbols = Color.FromArgb(64, 192, 255);
        public static readonly Color ColorSelection = Color.FromArgb(128, 0, 128, 255);

        public event MouseEventHandler ContextMenuOpening;

        private static Timer _timer = new Timer();

        private CharInfo[] _chars = new CharInfo[0];
        private TextSegment[] _segments = new TextSegment[0];
        private int _selEnd = 0;
        private int _selStart = 0;
        private bool _cursorVisible = false;
        private Point _offset = new Point(TextMargin, TextMargin);
        private int _scrollX = 0;
        private int _charHeight = 20;
        private string _placeHolder = "";

        private MouseButtons _pressedMouseButtons = MouseButtons.None;
        private Keys _pressedModifiers = Keys.None;

        public ExpressionBox() {
            this.Cursor = Cursors.IBeam;
            this.ImeMode = ImeMode.Disable;
            if (this.DesignMode) return;
            this.DoubleBuffered = true;
        }

        public bool ReadOnly { get; set; } = false;

        public int SelectionStart {
            get => (_selEnd < _selStart) ? _selEnd : _selStart;
            set => setSelection(value);
        } 
        
        public int SelectionLength { 
            get => Math.Abs(_selEnd - _selStart);
            set => setSelection(this.SelectionStart, this.SelectionStart + value);
        }

        public string SelectedText {
            get => this.Text.Substring(this.SelectionStart, this.SelectionLength);
            set {
                if (ReadOnly) throw new InvalidOperationException();
                int selStart = this.SelectionStart;
                int selLen = this.SelectionLength;
                var text = this.Text;
                text = text.Remove(selStart, selLen);
                if (!string.IsNullOrEmpty(value)) {
                    text = text.Insert(selStart, value);
                }
                this.Text = text;
                setSelection(selStart);
            }
        }
        
        public void SelectAll() {
            setSelection(0, this.Text.Length);
        }

        public string PlaceHolder {
            get => _placeHolder;
            set {
                if (value == _placeHolder) return;
                _placeHolder = value;
                this.Invalidate();
            }
        }

        public override Size GetPreferredSize(Size proposedSize) {
            return new Size(
                TextMargin * 2 + CursorPosToX(this.Text.Length),
                TextMargin * 2 + _charHeight
            );
        }

        /// <summary>カーソル位置からX座標を返す</summary>
        public int CursorPosToX(int i) {
            if (_chars.Length == 0) {
                return _offset.X - _scrollX;
            }
            else if (i < _chars.Length) {
                return (int)_chars[i].X - _scrollX;
            }
            else {
                return (int)(_chars[_chars.Length - 1].X + _chars[_chars.Length - 1].Width) - _scrollX;
            }
        }

        /// <summary>X座標からカーソル位置を返す</summary>
        public int XtoCursorPos(int x) {
            x += _scrollX;
            if (_chars.Length == 0) {
                return 0;
            }
            else {
                for (int i = 0; i < _chars.Length; i++) {
                    if (x <=_chars[i].X + _chars[i].Width / 2) {
                        return i;
                    }
                }
                return _chars.Length;
            }
        }

        /// <summary>現在のカーソルの矩形を返す</summary>
        public Rectangle GetCursorRectangle() {
            return new Rectangle(CursorPosToX(_selEnd), _offset.Y, 1, _charHeight);
        }

        /// <summary>選択領域の矩形を返す</summary>
        public Rectangle GetSelectionRectangle() {
            int selStart = SelectionStart;
            int selEnd = SelectionStart + SelectionLength;
            if (selStart == selEnd) {
                return Rectangle.Empty;
            }
            int x0 = CursorPosToX(selStart);
            int x1 = CursorPosToX(selEnd);
            return new Rectangle(x0, _offset.Y, x1 - x0, _charHeight);
        }

        public void Cut() {
            try {
                if (this.ReadOnly || this.SelectionLength == 0) {
                    throw new InvalidOperationException();
                }
                Clipboard.Clear();
                Clipboard.SetText(this.SelectedText);
                this.SelectedText = "";
            }
            catch {
                System.Media.SystemSounds.Beep.Play();
            }
        }

        public void Copy() {
            try {
                if (this.SelectionLength == 0) {
                    throw new InvalidOperationException();
                }
                Clipboard.Clear();
                Clipboard.SetText(this.SelectedText);
            }
            catch {
                System.Media.SystemSounds.Beep.Play();
            }
        }

        public void Paste() {
            try {
                if (this.ReadOnly || !Clipboard.ContainsText()) {
                    throw new InvalidOperationException();
                }
                int selStart = this.SelectionStart;
                var clipText = Clipboard.GetText();
                clipText = clipText.Replace("\t", " ");
                clipText = clipText.Replace("\r", "");
                clipText = clipText.Replace("\n", " ");
                this.SelectedText = clipText;
                setSelection(selStart + clipText.Length);
            }
            catch {
                System.Media.SystemSounds.Beep.Play();
            }
        }

        protected override void OnFontChanged(EventArgs e) {
            base.OnFontChanged(e);
            // 文字の再レイアウト
            relayoutChars();
        }

        protected override bool IsInputKey(Keys keyData) {
            // デフォルトでは方向キーでは OnKeyDown が呼び出されないので IsInputKey をオーバーライドする
            // https://stackoverflow.com/questions/1646998/up-down-left-and-right-arrow-keys-do-not-trigger-keydown-event
            switch (keyData) {
                case Keys.Right:
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                case Keys.Shift | Keys.Right:
                case Keys.Shift | Keys.Left:
                case Keys.Shift | Keys.Up:
                case Keys.Shift | Keys.Down:
                    return true;
            }
            return base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);
            _pressedModifiers = e.Modifiers;
            if (_pressedMouseButtons != MouseButtons.None) return;

            if (e.KeyCode == Keys.Left) {
                // カーソルを左へ移動
                e.Handled = true;
                int selStart = _selStart;
                int selEnd = _selEnd;
                if (!e.Shift && selStart != selEnd) {
                    selEnd = this.SelectionStart;
                }
                else {
                    selEnd = Math.Max(0, selEnd - 1);
                }
                if (!e.Shift) {
                    selStart = selEnd;
                }
                setSelection(selStart, selEnd);
            }
            else if (e.KeyCode == Keys.Right) {
                // カーソルを右へ移動
                e.Handled = true;
                int selStart = _selStart;
                int selEnd = _selEnd;
                if (!e.Shift && selEnd != selStart) {
                    selEnd = this.SelectionStart + this.SelectionLength;
                }
                else {
                    selEnd = Math.Min(this.Text.Length, selEnd + 1);
                }
                if (!e.Shift) {
                    selStart = selEnd;
                }
                setSelection(selStart, selEnd);
            }
            else if (e.KeyCode == Keys.Home) {
                // カーソルを先頭へ移動
                e.Handled = true;
                if (e.Shift) {
                    setSelection(_selStart, 0);
                }
                else {
                    setSelection(0);
                }
            }
            else if (e.KeyCode == Keys.End) {
                // カーソルを末尾へ移動
                e.Handled = true;
                if (e.Shift) {
                    setSelection(_selStart, this.Text.Length);
                }
                else {
                    setSelection(this.Text.Length);
                }
            }
            else if (!this.ReadOnly && e.KeyCode == Keys.Back) {
                // カーソルの前の文字を削除 or 選択範囲を削除
                e.Handled = true;
                int selStart = this.SelectionStart;
                int selLen = this.SelectionLength;
                if (selLen > 0) {
                    this.SelectedText = "";
                }
                else if (selStart > 0) {
                    this.Text = this.Text.Remove(selStart - 1, 1);
                    setSelection(selStart - 1);
                }
            }
            else if (!this.ReadOnly && e.KeyCode == Keys.Delete) {
                // カーソルの後ろの文字を削除 or 選択範囲を削除
                e.Handled = true;
                int selStart = this.SelectionStart;
                int selLen = this.SelectionLength;
                if (selLen != 0) {
                    this.SelectedText = "";
                }
                else if (selStart < this.Text.Length) {
                    this.Text = this.Text.Remove(selStart, 1);
                }
            }
            else if (e.Modifiers == Keys.Control) {
                if (e.KeyCode == Keys.C) {
                    e.Handled = true;
                    Copy();
                }
                else if (e.KeyCode == Keys.X) {
                    e.Handled = true;
                    Cut();
                }
                else if (e.KeyCode == Keys.V) {
                    e.Handled = true;
                    Paste();
                }
                else if (e.KeyCode == Keys.A) {
                    e.Handled = true;
                    SelectAll();
                }
            }
            
            restartCursorBlink();
        }

        protected override void OnKeyUp(KeyEventArgs e) {
            base.OnKeyUp(e);
            _pressedModifiers = e.Modifiers;
        }

        protected override void OnKeyPress(KeyPressEventArgs e) {
            base.OnKeyPress(e);
            if (_pressedMouseButtons != MouseButtons.None) return;
            if (this.ReadOnly) return;

            if (' ' <= e.KeyChar && e.KeyChar <= '~') {
                // カーソル位置に文字を挿入する
                e.Handled = true;
                var text = this.Text;
                int selStart = this.SelectionStart;
                int selLen = this.SelectionLength;
                if (selLen != 0) {
                    text = text.Remove(selStart, selLen);
                }
                text = text.Insert(selStart, e.KeyChar.ToString());
                this.Text = text;
                setSelection(selStart + 1);
            }

            restartCursorBlink();
        }

        protected override void OnTextChanged(EventArgs e) {
            // カーソル位置の調整
            var text = this.Text;
            if (_selEnd > text.Length) _selEnd = text.Length;
            if (_selStart > text.Length) _selStart = text.Length;
            // 文字の再レイアウト
            relayoutChars();
            // スクロール量の補正
            adjustScroll();
            // 文字が再レイアウトされてからUIイベントを発生させる
            base.OnTextChanged(e);
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            var g = e.Graphics;

            if (string.IsNullOrEmpty(this.Text)) {
                // 空欄の場合
                using (var brush = new SolidBrush(Color.FromArgb(128, this.ForeColor))) {
                    g.DrawString(_placeHolder, this.Font, brush, _offset.X, _offset.Y);
                }
            }

            // 文字列の描画
            g.TranslateTransform(-_scrollX, 0);
            foreach (var seg in _segments) {
                if (seg.Style.BackColor.A != 0) {
                    using (var brush = new SolidBrush(seg.Style.BackColor)) {
                        g.FillRectangle(brush, seg.X, _offset.Y, seg.Width, _charHeight);
                    }
                }
                using (var brush = new SolidBrush(seg.Style.ForeColor)) {
                    g.DrawString(seg.Text, this.Font, brush, seg.X, _offset.Y);
                }
            }
            g.ResetTransform();

            if (this.Focused && this.SelectionLength != 0) {
                // 選択範囲の描画
                using (var brush = new SolidBrush(ColorSelection)) {
                    g.FillRectangle(brush, GetSelectionRectangle());
                }
            }

            if (this.Focused && _cursorVisible) {
                // カーソルの描画
                using (var brush = new SolidBrush(this.ForeColor)) {
                    g.FillRectangle(brush, GetCursorRectangle());
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            if (!this.Focused) {
                this.Focus();
                return;
            }
            _pressedMouseButtons |= e.Button;
            if (e.Button == MouseButtons.Left) {
                if (_pressedModifiers == Keys.Shift) {
                    setSelection(_selStart, XtoCursorPos(e.X));
                }
                else {
                    setSelection(XtoCursorPos(e.X));
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            if (_pressedMouseButtons == MouseButtons.Left) {
                setSelection(_selStart, XtoCursorPos(e.X));
            }
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);
            _pressedMouseButtons &= ~e.Button;
            if (e.Button == MouseButtons.Right) {
                ContextMenuOpening?.Invoke(this, e);
            }
        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            adjustScroll();
        }

        protected override void OnGotFocus(EventArgs e) {
            base.OnGotFocus(e);
            // カーソル点滅開始
            _cursorVisible = true;
            _timer.Tick += _timer_Tick;
            _timer.Interval = 500;
            restartCursorBlink();
        }

        protected override void OnLostFocus(EventArgs e) {
            base.OnLostFocus(e);
            // カーソル点滅停止
            _cursorVisible = false;
            _timer.Tick -= _timer_Tick;
            this.Invalidate();
        }

        private void setSelection(int selStart) {
            setSelection(selStart, selStart);
        }

        private void setSelection(int selStart, int selEnd) {
            selStart = Math.Max(0, Math.Min(this.Text.Length, selStart));
            selEnd = Math.Max(0, Math.Min(this.Text.Length, selEnd));
            if (selStart == _selStart && selEnd == _selEnd) return;
            _selStart = selStart;
            _selEnd = selEnd;
            scrollToCursorPos(selEnd);
            restartCursorBlink();
        }

        private void scrollToCursorPos(int cursorPos) {
            var x = CursorPosToX(cursorPos);
            var right = this.ClientSize.Width - 1;
            if (x < 0) {
                _scrollX += x;
            }
            else if (x >= right) {
                _scrollX += (x - right);
            }
            this.Invalidate();
        }

        private void adjustScroll() {
            var text = this.Text;
            if (text.Length > 0) {
                var lastChar = _chars[text.Length - 1];
                var rightX = (int)(lastChar.X + lastChar.Width);
                var maxScrollX = Math.Max(0, rightX - (this.ClientSize.Width - 1));
                _scrollX = Math.Min(maxScrollX, _scrollX);
            }
            else {
                _scrollX = 0;
            }
            this.Invalidate();
        }

        private void restartCursorBlink() {
            if (!this.Focused) return;
            _cursorVisible = true;
            _timer.Stop();
            _timer.Start();
            this.Invalidate();
        }

        private void _timer_Tick(object sender, EventArgs e) {
            _cursorVisible = !_cursorVisible;
            this.Invalidate();
        }

        private void relayoutChars() {
            var text = this.Text;
            _chars = new CharInfo[text.Length];

            using (var g = this.CreateGraphics()) {
                // 文字の高さ
                _charHeight = (int)g.MeasureString("|", this.Font).Height;

                // 各文字の位置を割り出す
                // クリック座標からカーソル位置を割り出したりするのに使う
                // Textの値そのままだとなぜか末尾の空白スペースの幅を正しく計測できないため
                // 末尾に適当に文字を追加して計測する
                float x = _offset.X;
                string textForMeas = text + ".";
                for (int i = 0; i < text.Length; i++) {
                    var sf = new StringFormat();
                    sf.SetMeasurableCharacterRanges(new CharacterRange[] { new CharacterRange(i, 1) });
                    var range = g.MeasureCharacterRanges(textForMeas, this.Font, new RectangleF(0, 0, int.MaxValue, int.MaxValue), sf);
                    var charSize = range[0].GetBounds(g);
                    _chars[i].X = x;
                    _chars[i].Width = charSize.Width;
                    _chars[i].Style.ForeColor = this.ForeColor;
                    _chars[i].Style.BackColor = Color.Transparent;
                    x += charSize.Width;
                }
            }

            // 識別子の強調表示
            {
                var matches = RegexIDs.Matches(text);
                for (int i = 0; i < matches.Count; i++) {
                    var m = matches[i];
                    for (int j = 0; j < m.Length; j++) {
                        _chars[m.Index + j].Style.ForeColor = ColorId;
                    }
                }
            }

            // 色の強調表示
            {
                var matches = RegexColors.Matches(text);
                for (int i = 0; i < matches.Count; i++) {
                    var m = matches[i];
                    var s = m.Value.Substring(1);
                    var rgb = Convert.ToInt32(s, 16);
                    
                    // 背景色
                    Color back = Color.Black;
                    if (s.Length == 3) {
                        var r = (rgb >> 8) & 0xf;
                        var g = (rgb >> 4) & 0xf;
                        var b = rgb & 0xf;
                        r |= r << 4;
                        g |= g << 4;
                        b |= b << 4;
                        rgb = (0xff << 24) | (r << 16) | (g << 8) | b;
                        back = Color.FromArgb(rgb);
                    }
                    else {
                        rgb |= (0xff << 24);
                        back = Color.FromArgb(rgb);
                    }

                    // 前景色
                    Color fore;
                    int gray = ((30 * back.R) + (59 * back.G) + (11 * back.B)) / 100;
                    if (gray < 128) {
                        fore = Color.White;
                    }
                    else {
                        fore = Color.Black;
                    }

                    for (int j = 0; j < m.Length; j++) {
                        _chars[m.Index + j].Style.BackColor = back;
                        _chars[m.Index + j].Style.ForeColor = fore;
                    }
                }
            }

            // 記号の強調表示
            {
                var matches = RegexSymbols.Matches(text);
                for (int i = 0; i < matches.Count; i++) {
                    var m = matches[i];
                    for (int j = 0; j < m.Length; j++) {
                        _chars[m.Index + j].Style.ForeColor = ColorSymbols;
                    }
                }
            }

            // 描画を効率的にするために同じスタイルが適用されている文字の連続をまとめる
            var segs = new List<TextSegment>();
            int from = 0;
            while (from < text.Length) {
                // 同じスタイルが適用されている文字の集合を検出する
                var style = _chars[from].Style;
                int to = from + 1;
                while (to < text.Length && _chars[to].Style.Equals(style)) {
                    to++;
                }

                // 集合を TextSegment としてまとめる
                var seg = new TextSegment();
                seg.Text = text.Substring(from, to - from);
                seg.X = _chars[from].X;
                seg.Width = _chars[to - 1].X + _chars[to - 1].Width - _chars[from].X;
                seg.Style = style;
                segs.Add(seg);

                from = to;
            }
            _segments = segs.ToArray();

            this.Invalidate();
        }

        struct Style {
            public Color ForeColor;
            public Color BackColor;
            public override int GetHashCode() => base.GetHashCode();
            public override bool Equals(object obj) {
                if (obj is Style s) {
                    return 
                        ForeColor == s.ForeColor &&
                        BackColor == s.BackColor;
                }
                else {
                    return false;
                }
            }
        }

        struct CharInfo {
            public float X;
            public float Width;
            public Style Style;
        }

        struct TextSegment {
            public string Text;
            public float X;
            public float Width;
            public Style Style;
        }
    }
}
