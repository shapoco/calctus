using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using Shapoco.Calctus.Model.Syntax;

namespace Shapoco.Calctus.UI {
    class ExpressionBox : Control {
        public const int TextMargin = 0;

        public static readonly Regex SymbolRegex = new Regex(@"[+\-*/%:^|&=<>]");
        public static readonly Regex IdRegex = new Regex(@"\b[a-zA-Z_][a-zA-Z0-9_]*\b");
        public static readonly Regex ColorRegex = new Regex(@"#([0-9a-fA-F]{6}|[0-9a-fA-F]{3})\b");
        public static readonly Regex CharRegex = new Regex("'([^'\\\\]|\\\\[abfnrtv\\\\\'0]|\\\\o[0-7]{3}|\\\\x[0-9a-fA-F]{2}|\\\\u[0-9a-fA-F]{4})'");
        public static readonly Regex NonWordRegex = new Regex(@"\W");
        public static readonly Regex NonWordRtlRegex = new Regex(@"\W", RegexOptions.RightToLeft);

        public static readonly Color SymbolColor = Color.FromArgb(64, 192, 255);
        public static readonly Color IdColor = Color.FromArgb(192, 255, 128);
        public static readonly Color LiteralColor = Color.FromArgb(255, 192, 64);
        public static readonly Color[] ParenthesisColors = new Color[] {
            Color.FromArgb(64, 192, 255),
            Color.FromArgb(192, 128, 255),
            Color.FromArgb(255, 128, 192),
            Color.FromArgb(255, 192, 64),
        };
        public static readonly Color SelectionColor = Color.FromArgb(128, 0, 128, 255);

        // C#でIMEの入力を受けるユーザーコントロールの作成 - Qiita
        // https://qiita.com/takao_mofumofu/items/24c060a1d4f6b3df5c73
        // [C#] IME変換領域(ウィンドウ)のフォントを設定する - iPentec
        // https://www.ipentec.com/document/csharp-call-immsetcompositionfont
        #region "IME関連"

        private const int WM_IME_COMPOSITION = 0x010F;
        private const int GCS_RESULTREADSTR = 0x0200;
        private const int WM_IME_STARTCOMPOSITION = 0x10D; // IME変換開始
        private const int WM_IME_ENDCOMPOSITION = 0x10E;   // IME変換終了
        private const int WM_IME_NOTIFY = 0x0282;
        private const int WM_IME_SETCONTEXT = 0x0281;

        private enum ImmAssociateContextExFlags : uint {
            IACE_CHILDREN = 0x0001,
            IACE_DEFAULT = 0x0010,
            IACE_IGNORENOCONTEXT = 0x0020
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct C_RECT {
            public int _Left;
            public int _Top;
            public int _Right;
            public int _Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct C_POINT {
            public int x;
            public int y;
        }

        private const uint CFS_POINT = 0x0002;

        private struct COMPOSITIONFORM {
            public uint dwStyle;
            public C_POINT ptCurrentPos;
            public C_RECT rcArea;
        }

        //LOGFONT
        public enum FontWeight : int {
            FW_DONTCARE = 0,
            FW_THIN = 100,
            FW_EXTRALIGHT = 200,
            FW_LIGHT = 300,
            FW_NORMAL = 400,
            FW_MEDIUM = 500,
            FW_SEMIBOLD = 600,
            FW_BOLD = 700,
            FW_EXTRABOLD = 800,
            FW_HEAVY = 900,
        }

        public enum FontCharSet : byte {
            ANSI_CHARSET = 0,
            DEFAULT_CHARSET = 1,
            SYMBOL_CHARSET = 2,
            SHIFTJIS_CHARSET = 128,
            HANGEUL_CHARSET = 129,
            HANGUL_CHARSET = 129,
            GB2312_CHARSET = 134,
            CHINESEBIG5_CHARSET = 136,
            OEM_CHARSET = 255,
            JOHAB_CHARSET = 130,
            HEBREW_CHARSET = 177,
            ARABIC_CHARSET = 178,
            GREEK_CHARSET = 161,
            TURKISH_CHARSET = 162,
            VIETNAMESE_CHARSET = 163,
            THAI_CHARSET = 222,
            EASTEUROPE_CHARSET = 238,
            RUSSIAN_CHARSET = 204,
            MAC_CHARSET = 77,
            BALTIC_CHARSET = 186,
        }

        public enum FontPrecision : byte {
            OUT_DEFAULT_PRECIS = 0,
            OUT_STRING_PRECIS = 1,
            OUT_CHARACTER_PRECIS = 2,
            OUT_STROKE_PRECIS = 3,
            OUT_TT_PRECIS = 4,
            OUT_DEVICE_PRECIS = 5,
            OUT_RASTER_PRECIS = 6,
            OUT_TT_ONLY_PRECIS = 7,
            OUT_OUTLINE_PRECIS = 8,
            OUT_SCREEN_OUTLINE_PRECIS = 9,
            OUT_PS_ONLY_PRECIS = 10,
        }

        public enum FontClipPrecision : byte {
            CLIP_DEFAULT_PRECIS = 0,
            CLIP_CHARACTER_PRECIS = 1,
            CLIP_STROKE_PRECIS = 2,
            CLIP_MASK = 0xf,
            CLIP_LH_ANGLES = (1 << 4),
            CLIP_TT_ALWAYS = (2 << 4),
            CLIP_DFA_DISABLE = (4 << 4),
            CLIP_EMBEDDED = (8 << 4),
        }

        public enum FontQuality : byte {
            DEFAULT_QUALITY = 0,
            DRAFT_QUALITY = 1,
            PROOF_QUALITY = 2,
            NONANTIALIASED_QUALITY = 3,
            ANTIALIASED_QUALITY = 4,
            CLEARTYPE_QUALITY = 5,
            CLEARTYPE_NATURAL_QUALITY = 6,
        }

        [Flags]
        public enum FontPitchAndFamily : byte {
            DEFAULT_PITCH = 0,
            FIXED_PITCH = 1,
            VARIABLE_PITCH = 2,
            FF_DONTCARE = (0 << 4),
            FF_ROMAN = (1 << 4),
            FF_SWISS = (2 << 4),
            FF_MODERN = (3 << 4),
            FF_SCRIPT = (4 << 4),
            FF_DECORATIVE = (5 << 4),
        }

        const int LF_FACESIZE = 32;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class LOGFONT {
            public int lfHeight;
            public int lfWidth;
            public int lfEscapement;
            public int lfOrientation;
            public FontWeight lfWeight;
            [MarshalAs(UnmanagedType.U1)]
            public bool lfItalic;
            [MarshalAs(UnmanagedType.U1)]
            public bool lfUnderline;
            [MarshalAs(UnmanagedType.U1)]
            public bool lfStrikeOut;
            public FontCharSet lfCharSet;
            public FontPrecision lfOutPrecision;
            public FontClipPrecision lfClipPrecision;
            public FontQuality lfQuality;
            public FontPitchAndFamily lfPitchAndFamily;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = LF_FACESIZE * 2)]
            public string lfFaceName;
        }
        [DllImport("Imm32.dll")]
        private static extern IntPtr ImmGetContext(IntPtr hWnd);
        [DllImport("Imm32.dll")]
        private static extern int ImmGetCompositionString(IntPtr hIMC, int dwIndex, StringBuilder lpBuf, int dwBufLen);
        [DllImport("Imm32.dll")]
        private static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);
        [DllImport("imm32.dll")]
        private static extern IntPtr ImmCreateContext();
        [DllImport("imm32.dll")]
        private static extern bool ImmAssociateContextEx(IntPtr hWnd, IntPtr hIMC, ImmAssociateContextExFlags dwFlags);
        [DllImport("imm32.dll")]
        private static extern int ImmSetCompositionWindow(IntPtr hIMC, ref COMPOSITIONFORM lpCompositionForm);
        [DllImport("kernel32.dll")]
        private static extern IntPtr GlobalLock(IntPtr hMem);
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GlobalUnlock(IntPtr hMem);
        [DllImport("imm32.dll")]
        private static extern int ImmSetCompositionFont(IntPtr hIMC, [In, Out] IntPtr lplf);

        #endregion

        private IntPtr himc = IntPtr.Zero;

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
        private string _undoBuff0 = "";
        private string _undoBuff1 = "";

        private MouseButtons _pressedMouseButtons = MouseButtons.None;
        private Keys _pressedModifiers = Keys.None;

        private ICandidateProvider _candProvider = null;
        private CandidateForm _candForm = null;
        private int _candStart = 0, _candEnd = 0;

        public ExpressionBox() {
            this.Cursor = Cursors.IBeam;
            if (this.DesignMode) return;
            this.DoubleBuffered = true;
        }

        protected override void Dispose(bool disposing) {
            if (himc != IntPtr.Zero) {
                try {
                    ImmReleaseContext(this.Handle, himc);
                }
                catch { }
                himc = IntPtr.Zero;
            }
            base.Dispose(disposing);
        }

        public bool ReadOnly { get; set; } = false;

        public ICandidateProvider CandidateProvider {
            get => _candProvider;
            set => _candProvider = value;
        }

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

        public void Undo() {
            if (this.ReadOnly) {
                System.Media.SystemSounds.Beep.Play();
                return;
            }
            this.Text = _undoBuff1;
            setSelection(this.Text.Length);
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
                case Keys.Tab:
                    return _candForm != null;
            }
            return base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            _pressedModifiers = e.Modifiers;
            if (_pressedMouseButtons != MouseButtons.None) return;

            if (e.KeyCode == Keys.Left) {
                // カーソルを左へ移動
                e.Handled = true;
                int selStart = _selStart;
                int selEnd = _selEnd;
                if (e.Control) {
                    var match = NonWordRtlRegex.Match(this.Text, 0, selEnd);
                    if (match.Success) {
                        selEnd = match.Index;
                    }
                    else {
                        selEnd = 0;
                    }
                }
                else if (!e.Shift && selStart != selEnd) {
                    selEnd = this.SelectionStart;
                }
                else {
                    selEnd = Math.Max(0, selEnd - 1);
                }
                if (!e.Shift) {
                    selStart = selEnd;
                }
                setSelection(selStart, selEnd);

                if (_candForm != null && (SelectionStart < _candStart || SelectionLength > 0)) {
                    hideCandidates();
                }
            }
            else if (e.KeyCode == Keys.Right) {
                // カーソルを右へ移動
                e.Handled = true;
                int selStart = _selStart;
                int selEnd = _selEnd;
                if (e.Control) {
                    var match = NonWordRegex.Match(this.Text, selEnd);
                    if (match.Success) {
                        selEnd = match.Index + 1;
                    }
                    else {
                        selEnd = this.Text.Length;
                    }
                }
                else if (!e.Shift && selEnd != selStart) {
                    selEnd = this.SelectionStart + this.SelectionLength;
                }
                else {
                    selEnd = Math.Min(this.Text.Length, selEnd + 1);
                }
                if (!e.Shift) {
                    selStart = selEnd;
                }
                setSelection(selStart, selEnd);

                if (_candForm != null && (SelectionStart > _candEnd || SelectionLength > 0)) {
                    hideCandidates();
                }
            }
            else if (e.Modifiers == Keys.None && e.KeyCode == Keys.Up) {
                if (_candForm != null) {
                    e.Handled = true;
                    _candForm.SelectUp();
                }
            }
            else if (e.Modifiers == Keys.None && e.KeyCode == Keys.Down) {
                if (_candForm != null) {
                    e.Handled = true;
                    _candForm.SelectDown();
                }
            }
            else if (e.Modifiers == Keys.None && (e.KeyCode == Keys.Tab || e.KeyCode == Keys.Return)) {
                if (_candForm != null) {
                    e.Handled = true;
                    var item = _candForm.SelectedItem;
                    if (item != null) {
                        var id = item.Id;
                        setSelection(_candStart, _candEnd);
                        if (item.IsFunction) {
                            SelectedText = id + "()";
                            SelectionStart = _candStart + id.Length + 1;
                        }
                        else {
                            SelectedText = id;
                            SelectionStart = _candStart + id.Length;
                        }
                    }
                    hideCandidates();
                }
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
                hideCandidates();
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
                hideCandidates();
            }
            else if (!this.ReadOnly && e.Modifiers == Keys.None && e.KeyCode == Keys.Back) {
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
                updateCandidates();
            }
            else if (!this.ReadOnly && e.Modifiers == Keys.None && e.KeyCode == Keys.Delete) {
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
                updateCandidates();
            }
            else if (e.Modifiers == Keys.None && e.KeyCode == Keys.Escape) {
                e.Handled = true;
                hideCandidates();
            }
            else if (e.Modifiers == Keys.Control) {
                e.SuppressKeyPress = true;
                if (e.KeyCode == Keys.C) {
                    e.Handled = true;
                    hideCandidates();
                    Copy();
                }
                else if (e.KeyCode == Keys.X) {
                    e.Handled = true;
                    hideCandidates();
                    Cut();
                }
                else if (e.KeyCode == Keys.V) {
                    e.Handled = true;
                    hideCandidates();
                    Paste();
                }
                else if (e.KeyCode == Keys.A) {
                    e.Handled = true;
                    hideCandidates();
                    SelectAll();
                }
                else if (e.KeyCode == Keys.Z) {
                    e.Handled = true;
                    hideCandidates();
                    Undo();
                }
                else if (e.KeyCode == Keys.Space) {
                    e.Handled = true;
                    showCandidates();
                }
            }
            
            restartCursorBlink();

            if (!e.Handled) base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e) {
            base.OnKeyUp(e);
            _pressedModifiers = e.Modifiers;
        }

        protected override void OnKeyPress(KeyPressEventArgs e) {
            base.OnKeyPress(e);
            if (_pressedMouseButtons != MouseButtons.None) return;
            if (this.ReadOnly) return;

            if (!char.IsControl(e.KeyChar)) {
                // カーソル位置に文字を挿入する
                e.Handled = true;
                var text = this.Text;
                int selStart = this.SelectionStart;
                int selLen = this.SelectionLength;
                if (selLen != 0) {
                    text = text.Remove(selStart, selLen);
                }
                text = text.Insert(selStart, e.KeyChar.ToString());

                if (e.KeyChar == '(') {
                    if (Settings.Instance.Input_AutoCloseBrackets) {
                        // 閉じ括弧の補完
                        text = text.Insert(selStart + 1, ")");
                    }
                }

                this.Text = text;
                setSelection(selStart + 1);

                selStart = this.SelectionStart;
                var prevChar = selStart >= 2 ? text[selStart - 2] : '\0';
                if (!isIdChar(prevChar) && isFirstIdChar(e.KeyChar)) {
                    // 識別子の先頭文字が入力されたら補完候補を表示する
                    showCandidates();
                }
                else if (isIdChar(e.KeyChar)) {
                    // 識別子の2文字目以降が表示されたら補完候補を更新する
                    updateCandidates();
                }
                else {
                    // 識別子の文字以外が表示されたら補間をキャンセルする
                    hideCandidates();
                }
            }

            restartCursorBlink();
        }

        protected override void OnTextChanged(EventArgs e) {
            var text = this.Text;
            // Undoバッファの更新
            _undoBuff1 = _undoBuff0;
            _undoBuff0 = text;
            // カーソル位置の調整
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
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            if (string.IsNullOrEmpty(this.Text)) {
                // 空欄の場合
                using (var brush = new SolidBrush(Color.FromArgb(128, this.ForeColor))) {
                    g.DrawString(_placeHolder, this.Font, brush, _offset.X, _offset.Y);
                }
            }

            // 文字列の描画
            if (_segments.Length > 0) {
                float head_padding = _chars[0].X;
                g.TranslateTransform(-_scrollX, 0);
                foreach (var seg in _segments) {
                    if (seg.Style.BackColor.A != 0) {
                        using (var brush = new SolidBrush(seg.Style.BackColor)) {
                            g.FillRectangle(brush, seg.X, _offset.Y, seg.Width, _charHeight);
                        }
                    }
                    using (var brush = new SolidBrush(seg.Style.ForeColor)) {
                        g.DrawString(seg.Text, this.Font, brush, new PointF(seg.X - head_padding, _offset.Y));
                    }
                }
                g.ResetTransform();
            }

            if (this.Focused && this.SelectionLength != 0) {
                // 選択範囲の描画
                using (var brush = new SolidBrush(SelectionColor)) {
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

        protected override void OnDoubleClick(EventArgs e) {
            base.OnDoubleClick(e);
            this.SelectAll();
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
            // 補完候補キャンセル
            hideCandidates();
            // カーソル点滅停止
            _cursorVisible = false;
            _timer.Tick -= _timer_Tick;
            this.Invalidate();
            base.OnLostFocus(e);
        }

        // C#でIMEの入力を受けるユーザーコントロールの作成 - Qiita
        // https://qiita.com/takao_mofumofu/items/24c060a1d4f6b3df5c73
        // [C#] IME変換領域(ウィンドウ)のフォントを設定する - iPentec
        // https://www.ipentec.com/document/csharp-call-immsetcompositionfont
        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                case WM_IME_SETCONTEXT: {
                        //Imeを関連付ける
                        himc = ImmCreateContext();
                        ImmAssociateContextEx(this.Handle, himc, ImmAssociateContextExFlags.IACE_DEFAULT);
                        base.WndProc(ref m);
                        break;
                    }
                case WM_IME_STARTCOMPOSITION: {
                        //入力コンテキストにアクセスするためのお約束
                        IntPtr himc = ImmGetContext(this.Handle);

                        var cursorRect = this.GetCursorRectangle();

                        //コンポジションウィンドウの位置を設定
                        COMPOSITIONFORM info = new COMPOSITIONFORM();
                        info.dwStyle = CFS_POINT;
                        info.ptCurrentPos.x = cursorRect.X;
                        info.ptCurrentPos.y = cursorRect.Y;
                        ImmSetCompositionWindow(himc, ref info);

                        //コンポジションウィンドウのフォントを設定
                        IntPtr hHGlobalLOGFONT = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(LOGFONT)));
                        IntPtr pLogFont = GlobalLock(hHGlobalLOGFONT);
                        LOGFONT logFont = new LOGFONT();
                        this.Font.ToLogFont(logFont);
                        logFont.lfFaceName = this.Font.Name;
                        Marshal.StructureToPtr(logFont, pLogFont, false);
                        GlobalUnlock(hHGlobalLOGFONT);
                        ImmSetCompositionFont(himc, hHGlobalLOGFONT);
                        Marshal.FreeHGlobal(hHGlobalLOGFONT);

                        //入力コンテキストへのアクセスが終了したらロックを解除する
                        ImmReleaseContext(Handle, himc);


                        base.WndProc(ref m);
                        break;
                    }
                default:
                    //IME以外のメッセージは元のプロシージャで処理
                    base.WndProc(ref m);
                    break;
            }
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

        private bool isFirstIdChar(char c) => ('a' <= c && c <= 'z') || ('A' <= c && c <= 'Z') || c == '_';
        private bool isIdChar(char c) => isFirstIdChar(c) || ('0' <= c && c <= '9');

        private void showCandidates() {
            if (_candProvider == null) return;
            if (_candForm == null) {
                _candForm = new CandidateForm(_candProvider);
                _candForm.Visible = true;
                _candForm.Font = new Font(Settings.Instance.Appearance_Font_Button_Name, Settings.Instance.Appearance_Font_Size, FontStyle.Regular);
                _candForm.BackColor = Color.FromArgb(32, 32, 32);
                _candForm.ForeColor = this.ForeColor;
                selectCandKey();
                var x = _candStart < _chars.Length ? (int)_chars[_candStart].X : 0;
                _candForm.Location = this.PointToScreen(new Point(x, this.Height));
                updateCandList();
            }
            else {
                selectCandKey();
                updateCandList();
            }
        }

        private void updateCandidates() {
            if (_candForm == null) return;
            selectCandKey();
            updateCandList();
        }

        private void hideCandidates() {
            if (_candForm != null) {
                _candForm.Dispose();
                _candForm = null;
            }
        }

        private void selectCandKey() {
            var text = this.Text;
            var selStart = this.SelectionStart;
            var candStart = selStart;
            var candEnd = selStart;
            while (candStart > 0 && isIdChar(text[candStart - 1])) {
                candStart--;
            }
            while (candEnd < text.Length && isIdChar(text[candEnd])) {
                candEnd++;
            }
            _candStart = candStart;
            _candEnd = candEnd;
        }

        private void updateCandList() {
            if (_candForm == null) return;
            _candForm.SetKey(this.Text.Substring(_candStart, _candEnd - _candStart));
        }

        private void relayoutChars() {
            var text = this.Text;
            _chars = new CharInfo[text.Length];

            using (var g = this.CreateGraphics()) {
                // 欧文フォントで日本語の文字の位置を知るのに AntiAlias に設定する必要がある
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                // 文字の高さ
                _charHeight = (int)g.MeasureString("|", this.Font).Height;

                // 各文字の位置を割り出す
                // クリック座標からカーソル位置を割り出したりするのに使う
                for (int i = 0; i < text.Length; i++) {
                    using (var sf = new StringFormat()) {
                        // 両端の空白も含めて位置を知るのに MeasureTrailingSpaces が必要
                        sf.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;
                        sf.SetMeasurableCharacterRanges(new CharacterRange[] { new CharacterRange(i, 1) });
                        var range = g.MeasureCharacterRanges(text, this.Font, new RectangleF(0, 0, int.MaxValue, int.MaxValue), sf);
                        var charSize = range[0].GetBounds(g);
                        _chars[i].X = charSize.X;
                        _chars[i].Width = charSize.Width;
                        _chars[i].Style.ForeColor = Color.Transparent;
                        _chars[i].Style.BackColor = Color.Transparent;
                    }
                }
            }

            // 識別子の強調表示
            {
                var matches = IdRegex.Matches(text);
                for (int i = 0; i < matches.Count; i++) {
                    var m = matches[i];
                    for (int j = 0; j < m.Length; j++) {
                        _chars[m.Index + j].Style.ForeColor = IdColor;
                    }
                }
            }

            // 色の強調表示
            {
                var matches = ColorRegex.Matches(text);
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
                var matches = SymbolRegex.Matches(text);
                for (int i = 0; i < matches.Count; i++) {
                    var m = matches[i];
                    for (int j = 0; j < m.Length; j++) {
                        _chars[m.Index + j].Style.ForeColor = SymbolColor;
                    }
                }
            }

            // 日付リテラルの強調表示
            {
                var matches = NumberFormatter.DateTime.Pattern.Matches(text);
                for (int i = 0; i < matches.Count; i++) {
                    var m = matches[i];
                    for (int j = 0; j < m.Length; j++) {
                        _chars[m.Index + j].Style.ForeColor = LiteralColor;
                    }
                }
            }

            // 文字リテラルの強調表示
            {
                var matches = CharRegex.Matches(text);
                for (int i = 0; i < matches.Count; i++) {
                    var m = matches[i];
                    for (int j = 0; j < m.Length; j++) {
                        _chars[m.Index + j].Style.ForeColor = LiteralColor;
                    }
                }
            }

            // 括弧の強調表示
            {
                var stack = new Stack<int>();
                for (int i = 0; i < text.Length; i++ ) {
                    var c = text[i];
                    var foreColor = _chars[i].Style.ForeColor;
                    if (c == '(' && foreColor == Color.Transparent) {
                        stack.Push(i);
                    }
                    else if (c == ')' && foreColor == Color.Transparent) {
                        if (stack.Count > 0) {
                            int start = stack.Pop();
                            var color = ParenthesisColors[stack.Count % ParenthesisColors.Length];
                            _chars[start].Style.ForeColor = color;
                            _chars[i].Style.ForeColor = color;
                        }
                        else {
                            _chars[i].Style.ForeColor = Color.White;
                            _chars[i].Style.BackColor = Color.Red;
                        }
                    }
                }
                while (stack.Count > 0) {
                    int start = stack.Pop();
                    _chars[start].Style.ForeColor = Color.White;
                    _chars[start].Style.BackColor = Color.Red;
                }
            }

            // 色の付いてない文字はデフォルトの色にする
            for (int i = 0; i < text.Length; i++) {
                if (_chars[i].Style.ForeColor == Color.Transparent) {
                    _chars[i].Style.ForeColor = this.ForeColor;
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
