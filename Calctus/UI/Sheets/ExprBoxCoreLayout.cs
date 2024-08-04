using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Shapoco.Drawings;
using Shapoco.Calctus.Model;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Formats;
using Shapoco.Calctus.Model.Expressions;
using Shapoco.Calctus.Model.Evaluations;
using Shapoco.Calctus.Model.Values;

namespace Shapoco.Calctus.UI.Sheets {
    /// <summary>
    /// 数式の文字の配置と描画を担うクラス
    /// </summary>
    class ExprBoxCoreLayout {
        private static readonly Regex ExponentPattern = new Regex("[eE]-?[1-9][0-9]*(_[0-9]+)*$");
        private static readonly Regex DecimalPattern = new Regex(@"^(?<int>0|[1-9][0-9]*(_[0-9]+)*)(\.(?<frac>[0-9]+(_[0-9]+)*))?");
        private static readonly Regex HexBinOctPattern = new Regex(@"^(0[xX](?<hex>[0-9a-fA-F]+(_[0-9a-fA-F]+)*)|0[bB](?<bin>[01]+(_[01]+)*)|0(?<oct>[0-7]+(_[0-7]+)*))");

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

        public Exception EvalError {
            get => _evalError;
            set {
                if (value == _evalError) return;
                _evalError = value;
                _owner.Invalidate();
            }
        }

        public void Layout(string text) {
            var s = Settings.Instance;

            if (text == null || text.Trim().Length == 0) {
                _tokens = new TokenQueue();
                _exprObj = Expr.Empty;
                SyntaxError = LexerError.EmptyError;
            }
            else {
                try {
                    // 字句解析
                    _tokens = new Model.Parsers.Lexer(text).PopToEnd();

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
            }

            var font = _owner.Font;

            _chars = new ExprCharInfo[text.Length];

            // 自動桁区切り
            foreach (var t in _tokens) {
                if (t.Type == TokenType.Literal) {
                    Match m;
                    if (s.NumberFormat_Separator_Hexadecimal && (m = HexBinOctPattern.Match(t.Text)).Success && !m.Value.Contains("_")) {
                        insertSeparators(t, m.Groups["hex"], 4, false);
                        insertSeparators(t, m.Groups["bin"], 4, false);
                        insertSeparators(t, m.Groups["oct"], 4, false);
                    }
                    else if (s.NumberFormat_Separator_Thousands && (m = DecimalPattern.Match(t.Text)).Success && !m.Value.Contains("_")) {
                        insertSeparators(t, m.Groups["int"], 3, false); // 整数部
                        insertSeparators(t, m.Groups["frac"], 3, true); // 小数部
                    }
                }
            }

            int xShift = 0;
            using (var g = _owner.CreateGraphics()) {
                // 欧文フォントで日本語の文字の位置を知るのに AntiAlias に設定する必要がある
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                // 文字の高さ
                _charHeight = (int)g.MeasureString("|", font).Height;

                // 桁区切りの幅
                int numericSepWidth = (int)g.MeasureString("0", font).Width / 3;

                // 各文字の位置を割り出す
                // クリック座標からカーソル位置を割り出したりするのに使う
                for (int i = 0; i < text.Length; i++) {
                    using (var sf = new System.Drawing.StringFormat()) {
                        if (_chars[i].Shifted) xShift += numericSepWidth;

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
            var parenthesisColors = new Color[] {
                s.Appearance_Color_Parenthesis_1,
                s.Appearance_Color_Parenthesis_2,
                s.Appearance_Color_Parenthesis_3,
                s.Appearance_Color_Parenthesis_4,
            };
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
                if (t.Position == DeprecatedTextPosition.Nowhere) continue;

                // トークン種別に応じた強調表示
                switch (t.Type) {
                    case TokenType.Identifier:
                        // 識別子の強調表示
                        setForeColor(t, s.Appearance_Color_Identifiers);
                        break;

                    case TokenType.Literal:
                        if (t is LiteralToken lt) {
                            var fmt = lt.Value.FormatFlags;
                            var style = fmt.GetStyle();
                            if (lt.Value is BoolVal || style == FormatStyle.Character) {
                                // BoolVal
                                setForeColor(t, s.Appearance_Color_Special_Literals);
                            }
                            else if (fmt == FormatFlags.WebColor) {
                                // WebColorの強調表示
                                var back = Color.FromArgb((0xff << 24) | lt.Value.AsInt);
                                var gray = ColorEx.GrayScale(back);
                                var fore = gray.R < 128 ? Color.White : Color.Black;
                                setBackColor(t, back);
                                setForeColor(t, fore);
                            }
                            else if (lt.Value is RealVal && style != FormatStyle.DateTime && style != FormatStyle.TimeSpan) {
                                var postfixLen = lt.PostfixLength;
                                setForeColor(t.Position.Index + t.Text.Length - postfixLen, postfixLen, s.Appearance_Color_SI_Prefix);
                                //Match m;
                                //if ((m = ExponentPattern.Match(t.Text)).Success) {
                                //    // 指数の強調表示
                                //    setForeColor(t.Position.Index + m.Index, m.Length, s.Appearance_Color_SI_Prefix);
                                //}
                            }
                            else {
                                // その他のリテラルの強調表示
                                setForeColor(t, s.Appearance_Color_Special_Literals);
                            }
                        }
                        break;

                    case TokenType.OperatorSymbol:
                    case TokenType.GeneralSymbol:
                    case TokenType.Keyword:
                        // 記号とキーワードの強調表示
                        if ((t.Text == "(" || t.Text == ")") && parDepth >= 0) {
                            _chars[t.Position.Index].Style.ForeColor
                                = parenthesisColors[parDepth % parenthesisColors.Length];
                        }
                        else {
                            for (int i = 0; i < t.Text.Length; i++) {
                                _chars[t.Position.Index + i].Style.ForeColor = s.Appearance_Color_Symbols;
                            }
                        }
                        break;
                }
            }

            // 色の付いてない文字はデフォルトの色にする
            for (int i = 0; i < text.Length; i++) {
                if (_chars[i].Style.ForeColor == Color.Transparent) {
                    _chars[i].Style.ForeColor = s.Appearance_Color_Text;
                }
            }

            // 描画を効率的にするために同じスタイルが適用されている文字の連続をまとめる
            var segs = new List<ExprTextSegment>();
            int from = 0;
            while (from < text.Length) {
                // 同じスタイルが適用されている文字の集合を検出する
                var style = _chars[from].Style;
                int to = from + 1;
                while (to < text.Length && _chars[to].Style.Equals(style) && !_chars[to].Shifted) {
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

        private void setForeColor(Token t, Color color) => setForeColor(t.Position.Index, t.Text.Length, color);
        private void setForeColor(int start, int length, Color color) {
            for (int i = 0; i < length; i++) {
                _chars[start + i].Style.ForeColor = color;
            }
        }

        private void setBackColor(Token t, Color color) => setBackColor(t.Position.Index, t.Text.Length, color);
        private void setBackColor(int start, int length, Color color) {
            for (int i = 0; i < length; i++) {
                _chars[start + i].Style.BackColor = color;
            }
        }

        /// <summary>カーソル位置からX座標を返す</summary>
        public int CursorPosToX(int i) {
            if (_chars.Length == 0 || i < 0) {
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

        public Token GetTokenAt(int i, TokenType type) {
            foreach(var token in _tokens) {
                if (string.IsNullOrEmpty(token.Text)) continue;
                if (token.Position.Index <= i && i <= token.Position.Index + token.Text.Length && token.Type == type) {
                    return token;
                }
            }
            return null;
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
                if (error is LexerError lexerError) {
                    // 字句解析エラー
                    errorStart = lexerError.Position.Index;
                    errorEnd = errorStart + lexerError.Length;
                }
                else if (error is ParserError parserError) {
                    // 構文解析エラー
                    if (!Token.IsNullOrEmpty(parserError.Token)) {
                        errorStart = parserError.Token.Position.Index;
                        errorEnd = errorStart + parserError.Token.Text.Length;
                    }
                }
                else if (error is EvalError evalError) {
                    // 評価エラー
                    if (!Token.IsNullOrEmpty(evalError.Token)) {
                        errorStart = evalError.Token.Position.Index;
                        errorEnd = errorStart + evalError.Token.Text.Length;
                    }
                }
                int x0 = CursorPosToX(errorStart);
                int x1 = CursorPosToX(errorEnd);
                using (var brush = new SolidBrush(Settings.Instance.Appearance_Color_Error)) {
                    g.FillRectangle(brush, new Rectangle(x0, _charHeight - 3, x1 - x0, 3));
                }
            }

            g.Restore(bkp);
        }

        private void insertSeparators(Token t, Group g, int interval, bool fraction) {
            if (!g.Success) return;
            int len = g.Value.Length;
            int firstSepPos = fraction ? 0 : interval - (len % interval);
            for (int i = 1; i < len; i++) {
                if ((i + firstSepPos) % interval == 0) {
                    _chars[t.Position.Index + g.Index + i].Shifted = true;
                }
            }
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
        public bool Shifted;
        public ExprCharStyle Style;
    }

    struct ExprTextSegment {
        public string Text;
        public float X;
        public float Width;
        public ExprCharStyle Style;
    }
}
