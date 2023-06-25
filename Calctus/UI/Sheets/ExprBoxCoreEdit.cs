using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Globalization;
using Shapoco.Calctus.Model;
using Shapoco.Calctus.Model.Formats;
using Shapoco.Calctus.Model.Sheets;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Evaluations;
using Shapoco.Calctus.Model.Mathematics;
using Shapoco.Calctus.Model.Types;

namespace Shapoco.Calctus.UI.Sheets {
    class ExprBoxCoreEdit {
        public static readonly Regex NonWordRegex = new Regex(@"\W");
        public static readonly Regex NonWordRtlRegex = new Regex(@"\W", RegexOptions.RightToLeft);

        public event EventHandler TextChanged;
        public event EventHandler CursorStateChanged;

        public event QueryScreenCursorLocationEventHandler QueryScreenCursorLocation;
        public event QueryTokenEventHandler QueryToken;

        private string _text = "";
        private string _undoBuff = "";
        private int _selEnd = 0;
        private int _selStart = 0;

        public IInputCandidateProvider CandidateProvider;
        private InputCandidateForm _candForm;
        private int _candKeyStart = 0, _candKeyEnd = 0;

        private Keys _pressedModifiers = Keys.None;

        public bool ReadOnly = false;

        public string Text {
            get => _text;
            set {
                if (value == _text) return;
                _undoBuff = _text;
                _text = value;
                TextChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public int SelectionOrigin => _selStart;
        public int CursorPos => _selEnd;

        public int SelectionStart {
            get => (_selEnd < _selStart) ? _selEnd : _selStart;
            set => SetSelection(value);
        }

        public int SelectionLength {
            get => Math.Abs(_selEnd - _selStart);
            set => SetSelection(this.SelectionStart, this.SelectionStart + value);
        }

        public string SelectedText {
            get => Text.Substring(this.SelectionStart, this.SelectionLength);
            set {
                if (ReadOnly) throw new InvalidOperationException();
                int selStart = SelectionStart;
                int selLen = SelectionLength;
                var text = Text;
                text = text.Remove(selStart, selLen);
                if (!string.IsNullOrEmpty(value)) {
                    text = text.Insert(selStart, value);
                }
                Text = text;
                SetSelection(selStart);
            }
        }

        public void SelectAll() {
            SetSelection(0, Text.Length);
        }

        public void OnKeyDown(KeyEventArgs e) {
            _pressedModifiers = e.Modifiers;

            if (e.Handled) return;

            if (CandidatesAreShown()) {
                // 入力候補の操作
                if (e.Modifiers == Keys.None && e.KeyCode == Keys.Up) {
                    e.Handled = true;
                    _candForm.SelectUp();
                }
                else if (e.Modifiers == Keys.None && e.KeyCode == Keys.Down) {
                    e.Handled = true;
                    _candForm.SelectDown();
                }
                else if (e.Modifiers == Keys.None && (e.KeyCode == Keys.Tab || e.KeyCode == Keys.Return)) {
                    e.Handled = true;
                    var item = _candForm.SelectedItem;
                    if (item != null) {
                        var id = item.Id;
                        SetSelection(_candKeyStart, _candKeyEnd);
                        if (item.IsFunction) {
                            // 関数の補完
                            if (Settings.Instance.Input_AutoCloseBrackets) {
                                SelectedText = id + "()";
                            }
                            else {
                                SelectedText = id + "(";
                            }
                            SelectionStart = _candKeyStart + id.Length + 1;
                        }
                        else {
                            // 変数・定数の補完
                            SelectedText = id;
                            SelectionStart = _candKeyStart + id.Length;
                        }
                    }
                    CandidatesHide();
                }
            }

            if (e.Handled) return;
            
            if (!e.Alt && e.KeyCode == Keys.Left) {
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
                SetSelection(selStart, selEnd);

                if (CandidatesAreShown() && (SelectionStart < _candKeyStart || SelectionLength > 0)) {
                    CandidatesHide();
                }
            }
            else if (!e.Alt && e.KeyCode == Keys.Right) {
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
                SetSelection(selStart, selEnd);

                if (CandidatesAreShown() && (SelectionStart < _candKeyStart || SelectionLength > 0)) {
                    CandidatesHide();
                }
            }
            else if (e.KeyCode == Keys.Home) {
                // カーソルを先頭へ移動
                e.Handled = true;
                if (e.Shift) {
                    SetSelection(_selStart, 0);
                }
                else {
                    SetSelection(0);
                }
                CandidatesHide();
            }
            else if (e.KeyCode == Keys.End) {
                // カーソルを末尾へ移動
                e.Handled = true;
                if (e.Shift) {
                    SetSelection(_selStart, this.Text.Length);
                }
                else {
                    SetSelection(this.Text.Length);
                }
                CandidatesHide();
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
                    SetSelection(selStart - 1);
                }
                CandidatesUpdate();
            }
            else if (!this.ReadOnly && e.Modifiers.HasFlag(Keys.Control) && e.KeyCode == Keys.Back) {
                // カーソルの前の単語を削除 or 選択範囲を削除
                e.Handled = true;
                int selStart = this.SelectionStart;
                int selLen = this.SelectionLength;
                if (selLen > 0) {
                    this.SelectedText = "";
                }
                else if (selStart > 0) {
                    var match = NonWordRtlRegex.Match(this.Text, 0, selStart);
                    var newSelStart = 0;
                    if (match.Success) {
                        newSelStart = match.Index;
                    }
                    Text = Text.Substring(0, newSelStart) + Text.Substring(selStart);
                    SetSelection(newSelStart);
                }
                CandidatesHide();
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
                CandidatesUpdate();
            }
            else if ((e.Modifiers == Keys.Alt || e.Modifiers == (Keys.Alt | Keys.Shift)) && (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)) {
                e.Handled = true;
                var amount = e.Shift ? 1 : 3;
                amount = (e.KeyCode == Keys.Left) ? amount : -amount;
                changeEnotationExp(amount);
            }
            else if (e.Modifiers == Keys.Alt && (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)) {
                e.Handled = true;
                var amount = (e.KeyCode == Keys.Up) ? 1 : -1;
                changePrefix(amount);
            }
            else if (e.Modifiers == Keys.None && e.KeyCode == Keys.Escape) {
                e.Handled = true;
                CandidatesHide();
            }
            else if (e.Modifiers == Keys.Control) {
                e.SuppressKeyPress = true;
                if (e.KeyCode == Keys.C) {
                    e.Handled = true;
                    CandidatesHide();
                    Copy();
                }
                else if (e.KeyCode == Keys.X) {
                    e.Handled = true;
                    CandidatesHide();
                    Cut();
                }
                else if (e.KeyCode == Keys.V) {
                    e.Handled = true;
                    CandidatesHide();
                    Paste();
                }
                else if (e.KeyCode == Keys.A) {
                    e.Handled = true;
                    CandidatesHide();
                    SelectAll();
                }
                else if (e.KeyCode == Keys.Z) {
                    e.Handled = true;
                    CandidatesHide();
                    Undo();
                }
                else if (e.KeyCode == Keys.Space) {
                    if (Settings.Instance.Input_IdAutoCompletion) {
                        e.Handled = true;
                        CandidatesShow();
                    }
                }
            }
        }

        public void OnKeyUp(KeyEventArgs e) {
            _pressedModifiers = e.Modifiers;
        }

        public void OnKeyPress(KeyPressEventArgs e) {
            //if (_pressedMouseButtons != MouseButtons.None) return;
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

                if (Settings.Instance.Input_AutoCloseBrackets) {
                    // 閉じ括弧の補完
                    switch (e.KeyChar) {
                        case '(': text = text.Insert(selStart + 1, ")"); break;
                        case '[': text = text.Insert(selStart + 1, "]"); break;
                        case '{': text = text.Insert(selStart + 1, "}"); break;
                        case '\'': text = text.Insert(selStart + 1, "'"); break;
                        case '\"': text = text.Insert(selStart + 1, "\""); break;
                    }
                }

                this.Text = text;
                SetSelection(selStart + 1);

                selStart = SelectionStart;
                var prevChar = selStart >= 2 ? text[selStart - 2] : '\0';
                if (!Lexer.IsFollowingIdChar(prevChar) && prevChar != '\'' && prevChar != '\"' && prevChar != '#' && prevChar != '\\' && Lexer.IsFirstIdChar(e.KeyChar)) {
                    if (Settings.Instance.Input_IdAutoCompletion) {
                        // 識別子の先頭文字が入力されたら補完候補を表示する
                        CandidatesShow();
                    }
                }
                else if (_candKeyStart + 1 < selStart && Lexer.IsFollowingIdChar(e.KeyChar)) {
                    // 識別子の2文字目以降が表示されたら補完候補を更新する
                    CandidatesUpdate();
                }
                else {
                    // 識別子の文字以外が表示されたら補間をキャンセルする
                    CandidatesHide();
                }
            }
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
                Clipboard.Clear();
                if (this.SelectionLength == 0) {
                    Clipboard.SetText(this.Text);
                }
                else {
                    Clipboard.SetText(this.SelectedText);
                }
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
                SetSelection(selStart + clipText.Length);
            }
            catch {
                System.Media.SystemSounds.Beep.Play();
            }
        }

        public void Undo() {
            if (ReadOnly) {
                System.Media.SystemSounds.Beep.Play();
                return;
            }
            Text = _undoBuff;
            SetSelection(Text.Length);
        }

        /// <summary>指数を変更する</summary>
        private void changeEnotationExp(int amount) {
            try {
                decimal frac;
                char eChar;
                int exp;

                // カーソル位置の数値を解釈
                var queryTokenArgs = new QueryTokenEventArgs(SelectionStart, TokenType.NumericLiteral);
                QueryToken?.Invoke(this, queryTokenArgs);
                var token = queryTokenArgs.Result;
                if (token == null) return;

                if (real.TryParse(token.Text, out frac, out eChar, out exp)) { }
                else if (SiPrefixFormatter.TryParse(token.Text, out frac, out var prefixIndex)) {
                    exp = prefixIndex * 3;
                }
                else {
                    frac = Parser.Parse(token.Text).Eval(new EvalContext()).AsReal;
                    exp = 0;
                }

                if (eChar != 'e' && eChar != 'E') eChar = 'e';

                // 指数を変更
                frac *= RMath.Pow10(-amount);
                exp += amount;
                if (exp < -28 || 28 < exp) return;

                // 文字列に変換
                var changedStr = 
                    frac.ToString("0.##############################", CultureInfo.InvariantCulture)
                    + eChar + exp.ToString(CultureInfo.InvariantCulture);

                // 再度文字列に変換して元のトークンと差し替える
                SetSelection(token.Position.Index, token.Position.Index + token.Text.Length);
                SelectedText = changedStr;
                SelectionStart = token.Position.Index;
            }
            catch { }
        }

        /// <summary>SI接頭語を変更する</summary>
        private void changePrefix(int amount) {
            try {
                decimal frac;
                int prefixIndex;
                bool isBinaryPrefix;

                // カーソル位置の数値を解釈
                var queryTokenArgs = new QueryTokenEventArgs(SelectionStart, TokenType.NumericLiteral);
                QueryToken?.Invoke(this, queryTokenArgs);
                var token = queryTokenArgs.Result;
                if (token == null) return;

                if (SiPrefixFormatter.TryParse(token.Text, out frac, out prefixIndex)) {
                    isBinaryPrefix = false;
                }
                else if (BinaryPrefixFormatter.TryParse(token.Text, out frac, out prefixIndex)) {
                    isBinaryPrefix = true;
                }
                else if (real.TryParse(token.Text, out frac, out _, out int exp)) {
                    prefixIndex = exp / 3;
                    int alignedExp = prefixIndex * 3;
                    frac *= RMath.Pow10(exp - alignedExp);
                    isBinaryPrefix = false;
                }
                else {
                    frac = Parser.Parse(token.Text).Eval(new EvalContext()).AsReal;
                    prefixIndex = 0;
                    isBinaryPrefix = false;
                }

                // 接頭語を変更
                if (isBinaryPrefix) {
                    frac *= (decimal)Math.Pow(1024, -amount);
                    prefixIndex += amount;
                    if (prefixIndex < BinaryPrefixFormatter.MinPrefixIndex || BinaryPrefixFormatter.MaxPrefixIndex < prefixIndex) {
                        return;
                    }
                }
                else {
                    frac *= RMath.Pow10(-amount * 3);
                    prefixIndex += amount;
                    if (prefixIndex < SiPrefixFormatter.MinPrefixIndex || SiPrefixFormatter.MaxPrefixIndex < prefixIndex) {
                        return;
                    }
                }

                // 文字列に変換
                string changedStr = frac.ToString("0.##############################", CultureInfo.InvariantCulture);
                if (isBinaryPrefix) {
                    changedStr += BinaryPrefixFormatter.GetPrefixString(prefixIndex);
                }
                else if (prefixIndex != 0) {
                    changedStr += SiPrefixFormatter.GetPrefixChar(prefixIndex);
                }

                // 再度文字列に変換して元のトークンと差し替える
                SetSelection(token.Position.Index, token.Position.Index + token.Text.Length);
                SelectedText = changedStr;
                SelectionStart = token.Position.Index;
            }
            catch { }
        }

        public void SetSelection(int selStart) {
            SetSelection(selStart, selStart);
        }

        public void SetSelection(int selStart, int selEnd) {
            selStart = Math.Max(0, Math.Min(Text.Length, selStart));
            selEnd = Math.Max(0, Math.Min(Text.Length, selEnd));
            if (selStart == _selStart && selEnd == _selEnd) return;
            if (selStart != selEnd) {
                // 選択範囲が作成されたら入力候補を隠す
                CandidatesHide();
            }
            else if (selEnd < _candKeyStart) {
                // カーソルが候補キーより前に移動したら入力候補を隠す
                CandidatesHide();
            }
            else if (_candKeyEnd < selEnd) {
                // カーソルが候補キーより後ろ移動した場合、可能なら候補キーを拡張する
                // そうでなければ入力候補を隠す
                bool keyExtend = true;
                for (int i = _candKeyEnd; i < selEnd; i++) {
                    if (!Lexer.IsFollowingIdChar(Text[i])) {
                        keyExtend = false;
                        break;
                    }
                }
                if (keyExtend) {
                    _candKeyEnd = selEnd;
                }
                else {
                    CandidatesHide();
                }
            }
            _selStart = selStart;
            _selEnd = selEnd;
            CursorStateChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CandidatesAreShown() => (_candForm != null);

        public void CandidatesShow() {
            if (CandidateProvider == null) return;
            if (!CandidatesAreShown()) {
                _candForm = new InputCandidateForm(CandidateProvider);
                _candForm.Visible = true;
                CandidatesSelectKey();
                var e = new QueryScreenCursorLocationEventArgs(_candKeyStart);
                QueryScreenCursorLocation?.Invoke(this, e);
                _candForm.Location = e.Result;
                CandidatesSetKey();
            }
            else {
                CandidatesSelectKey();
                CandidatesSetKey();
            }
        }

        public void CandidatesHide() {
            if (CandidatesAreShown()) {
                _candForm.Dispose();
                _candForm = null;
            }
        }

        public void CandidatesUpdate() {
            if (CandidatesAreShown()) {
                CandidatesSelectKey();
                CandidatesSetKey();
            }
        }

        public void CandidatesSetKey() {
            if (CandidatesAreShown()) {
                _candForm.SetKey(Text.Substring(_candKeyStart, _candKeyEnd - _candKeyStart));
            }
        }

        public void CandidatesSelectKey() {
            var text = Text;
            var selStart = SelectionStart;
            var candStart = selStart;
            var candEnd = selStart;
            while (candStart > 0 && Lexer.IsFollowingIdChar(text[candStart - 1])) {
                candStart--;
            }
            _candKeyStart = candStart;
            _candKeyEnd = candEnd;
        }

    }
}
