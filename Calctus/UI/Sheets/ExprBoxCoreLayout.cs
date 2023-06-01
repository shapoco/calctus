using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Shapoco.Calctus.Model;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Formats;
using Shapoco.Calctus.Model.Expressions;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.UI.Sheets {
    /// <summary>
    /// 数式の文字の配置と描画を担うクラス
    /// </summary>
    class ExprBoxCoreLayout {
        public static readonly Color SymbolColor = Color.FromArgb(64, 192, 255);
        public static readonly Color IdColor = Color.FromArgb(192, 255, 128);
        public static readonly Color LiteralColor = Color.FromArgb(255, 192, 64);
        public static readonly Color PrefixColor = Color.FromArgb(192, 128, 255);
        public static readonly Color[] ParenthesisColors = new Color[] {
            Color.FromArgb(64, 192, 255),
            Color.FromArgb(192, 128, 255),
            Color.FromArgb(255, 128, 192),
            Color.FromArgb(255, 192, 64),
        };
        public static readonly Color ErrorColor = Color.FromArgb(192, 255, 128, 128);

        public event EventHandler PreferredSizeChanged;

        private readonly Control _owner;
        private TokenQueue _tokens;
        private ExprCharInfo[] _chars = new ExprCharInfo[0];
        private ExprTextSegment[] _segments = new ExprTextSegment[0];
        private Expr _exprObj = Expr.Empty;
        private Exception _evalError = null;
        public Exception SyntaxError { get; private set; }
        private int _charHeight;

        private Size _preferredSize = Size.Empty;

        public ExprBoxCoreLayout(Control owner) {
            _owner = owner;
        }

        public Expr ExprObject => _exprObj;

        public Size PreferredSize {
            get => _preferredSize;
            set {
                if (value == _preferredSize) return;
                _preferredSize = value;
                PreferredSizeChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public int CharHeight => _charHeight;

        public void Layout(string text) {
            try {
                // 字句解析
                _tokens = new Lexer(text).PopToEnd();

                // 不足している括弧の補完
                _tokens.CompleteParentheses();

                try {
                    // 構文解析
                    _exprObj = Parser.Parse((TokenQueue)_tokens.Clone());
                    SyntaxError = null;
                }
                catch (Exception ex) {
                    // 構文解析エラー
                    _exprObj = Expr.Empty;
                    SyntaxError = ex;
                }
            }
            catch (Exception ex) {
                // 字句解析エラー
                _tokens = new TokenQueue();
                _exprObj = Expr.Empty;
                SyntaxError = ex;
            }

            var font = _owner.Font;
            var foreColor = _owner.ForeColor;

            _chars = new ExprCharInfo[text.Length];

            int xShift = 0;
            using (var g = _owner.CreateGraphics()) {
                // 欧文フォントで日本語の文字の位置を知るのに AntiAlias に設定する必要がある
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                // 文字の高さ
                _charHeight = (int)g.MeasureString("|", font).Height;

                // 各文字の位置を割り出す
                // クリック座標からカーソル位置を割り出したりするのに使う
                for (int i = 0; i < text.Length; i++) {
                    using (var sf = new StringFormat()) {
                        // 両端の空白も含めて位置を知るのに MeasureTrailingSpaces が必要
                        sf.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;
                        sf.SetMeasurableCharacterRanges(new CharacterRange[] { new CharacterRange(i, 1) });
                        var range = g.MeasureCharacterRanges(text, font, new RectangleF(0, 0, int.MaxValue, int.MaxValue), sf);
                        var charSize = range[0].GetBounds(g);
                        _chars[i].X = xShift + charSize.X;
                        _chars[i].Width = charSize.Width;
                        _chars[i].Style.ForeColor = Color.Transparent;
                        _chars[i].Style.BackColor = Color.Transparent;
                    }
                }
            }

            // 強調表示
            int parDepthCounter = 0;
            int parDepth = 0;
            foreach (var t in _tokens) {
                // 括弧の深さのカウント
                if (t.Type == TokenType.GeneralSymbol) {
                    if (t.Text == "(") {
                        parDepth = parDepthCounter;
                        parDepthCounter++;
                    }
                    else if (t.Text == ")") {
                        parDepthCounter--;
                        parDepth = parDepthCounter;
                    }
                }

                // 位置情報を持たないトークンは無視する
                if (t.Position == TextPosition.Nowhere) continue;

                // トークン種別に応じた強調表示
                switch (t.Type) {
                    case TokenType.Word:
                        // 識別子の強調表示
                        for (int i = 0; i < t.Text.Length; i++) {
                            _chars[t.Position.Index + i].Style.ForeColor = IdColor;
                        }
                        break;

                    case TokenType.BoolLiteral:
                        // 真偽値の強調表示
                        for (int i = 0; i < t.Text.Length; i++) {
                            _chars[t.Position.Index + i].Style.ForeColor = LiteralColor;
                        }
                        break;

                    case TokenType.NumericLiteral:
                        if (t.Hint is NumberTokenHint nth) {
                            if (nth.Value.FormatHint.Formatter == NumberFormatter.SiPrefixed) {
                                // SI接頭語の強調表示
                                _chars[t.Position.Index + t.Text.Length - 1].Style.ForeColor = PrefixColor;
                            }
                            else if (nth.Value.FormatHint.Formatter == NumberFormatter.BinaryPrefixed) {
                                // 二進接頭語の強調表示
                                _chars[t.Position.Index + t.Text.Length - 2].Style.ForeColor = PrefixColor;
                                _chars[t.Position.Index + t.Text.Length - 1].Style.ForeColor = PrefixColor;
                            }
                            else if (nth.Value.FormatHint.Formatter == NumberFormatter.WebColor) {
                                // WebColorの強調表示
                                var back = Color.FromArgb((0xff << 24) | nth.Value.AsInt);
                                int gray = ((30 * back.R) + (59 * back.G) + (11 * back.B)) / 100;
                                var fore = gray < 128 ? Color.White : Color.Black;
                                for (int i = 0; i < t.Text.Length; i++) {
                                    _chars[t.Position.Index + i].Style.BackColor = back;
                                    _chars[t.Position.Index + i].Style.ForeColor = fore;
                                }
                            }
                            else if (nth.Value.FormatHint.Formatter == NumberFormatter.CStyleChar) {
                                // 文字リテラルの強調表示
                                for (int i = 0; i < t.Text.Length; i++) {
                                    _chars[t.Position.Index + i].Style.ForeColor = LiteralColor;
                                }
                            }
                            else if (nth.Value.FormatHint.Formatter == NumberFormatter.DateTime) {
                                // 日付リテラルの強調表示
                                for (int i = 0; i < t.Text.Length; i++) {
                                    _chars[t.Position.Index + i].Style.ForeColor = LiteralColor;
                                }
                            }
                        }
                        break;

                    case TokenType.OperatorSymbol:
                    case TokenType.GeneralSymbol:
                    case TokenType.Keyword:
                        // 記号とキーワードの強調表示
                        if ((t.Text == "(" || t.Text == ")") && parDepth >= 0) {
                            _chars[t.Position.Index].Style.ForeColor
                                = ParenthesisColors[parDepth % ParenthesisColors.Length];
                        }
                        else {
                            for (int i = 0; i < t.Text.Length; i++) {
                                _chars[t.Position.Index + i].Style.ForeColor = SymbolColor;
                            }
                        }
                        break;
                }
            }

            // 色の付いてない文字はデフォルトの色にする
            for (int i = 0; i < text.Length; i++) {
                if (_chars[i].Style.ForeColor == Color.Transparent) {
                    _chars[i].Style.ForeColor = foreColor;
                }
            }

            // 描画を効率的にするために同じスタイルが適用されている文字の連続をまとめる
            var segs = new List<ExprTextSegment>();
            int from = 0;
            while (from < text.Length) {
                // 同じスタイルが適用されている文字の集合を検出する
                var style = _chars[from].Style;
                int to = from + 1;
                while (to < text.Length && _chars[to].Style.Equals(style)) {
                    to++;
                }

                // 集合を TextSegment としてまとめる
                var seg = new ExprTextSegment();
                seg.Text = text.Substring(from, to - from);
                seg.X = _chars[from].X;
                seg.Width = _chars[to - 1].X + _chars[to - 1].Width - _chars[from].X;
                seg.Style = style;
                segs.Add(seg);

                from = to;
            }
            _segments = segs.ToArray();

            PreferredSize = new Size(CursorPosToX(text.Length), _charHeight);
        }
        
        /// <summary>カーソル位置からX座標を返す</summary>
        public int CursorPosToX(int i) {
            if (_chars.Length == 0) {
                return 0;
            }
            else if (i < _chars.Length) {
                return (int)_chars[i].X;
            }
            else {
                return (int)(_chars[_chars.Length - 1].X + _chars[_chars.Length - 1].Width);
            }
        }

        /// <summary>X座標からカーソル位置を返す</summary>
        public int XtoCursorPos(int x) {
            if (_chars.Length == 0) {
                return 0;
            }
            else {
                for (int i = 0; i < _chars.Length; i++) {
                    if (x <= _chars[i].X + _chars[i].Width / 2) {
                        return i;
                    }
                }
                return _chars.Length;
            }
        }

        public void Paint(Graphics g, Point offset) {
            var font = _owner.Font;

            var bkp = g.Save();
            g.TranslateTransform(offset.X, offset.Y);

            // 文字列の描画
            if (_segments.Length > 0) {
                float head_padding = _chars[0].X;
                foreach (var seg in _segments) {
                    if (seg.Style.BackColor.A != 0) {
                        using (var brush = new SolidBrush(seg.Style.BackColor)) {
                            g.FillRectangle(brush, seg.X, 0, seg.Width, _charHeight);
                        }
                    }
                    using (var brush = new SolidBrush(seg.Style.ForeColor)) {
                        g.DrawString(seg.Text, font, brush, new PointF(seg.X - head_padding, 0));
                    }
                }
            }

            var error = SyntaxError != null ? SyntaxError : _evalError;

            if (_chars.Length > 0 && error != null) {
                // 文法エラーの強調表示
                int errorStart = 0;
                int errorEnd = _chars.Length;
                if (error is LexerError syntaxError) {
                    // 字句解析エラー
                    errorStart = syntaxError.Position.Index;
                    errorEnd = errorStart + 1;
                }
                else if (error is ParserError parserError) {
                    // 構文解析エラー
                    if (parserError.Token != null && parserError.Token.Text != null) {
                        errorStart = parserError.Token.Position.Index;
                        errorEnd = errorStart + parserError.Token.Text.Length;
                    }
                }
                else if (error is EvalError evalError) {
                    // 評価エラー
                    if (evalError.Token != null && evalError.Token.Text != null) {
                        errorStart = evalError.Token.Position.Index;
                        errorEnd = errorStart + evalError.Token.Text.Length;
                    }
                }
                int x0 = CursorPosToX(errorStart);
                int x1 = CursorPosToX(errorEnd);
                using (var brush = new SolidBrush(ErrorColor)) {
                    g.FillRectangle(brush, new Rectangle(x0, _charHeight - 3, x1 - x0, 3));
                }
            }

            g.Restore(bkp);
        }
    }

    struct ExprCharStyle {
        public Color ForeColor;
        public Color BackColor;
        public override int GetHashCode() => base.GetHashCode();
        public override bool Equals(object obj) {
            if (obj is ExprCharStyle s) {
                return
                    ForeColor == s.ForeColor &&
                    BackColor == s.BackColor;
            }
            else {
                return false;
            }
        }
    }

    struct ExprCharInfo {
        public float X;
        public float Width;
        public ExprCharStyle Style;
    }

    struct ExprTextSegment {
        public string Text;
        public float X;
        public float Width;
        public ExprCharStyle Style;
    }
}
