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
using Shapoco.Calctus.Model.Sheets;

namespace Shapoco.Calctus.UI.Sheets {
    class ExprBoxCoreEdit {
        public static readonly Regex NonWordRegex = new Regex(@"\W");
        public static readonly Regex NonWordRtlRegex = new Regex(@"\W", RegexOptions.RightToLeft);

        public event EventHandler TextChanged;
        public event EventHandler CursorStateChanged;

        public event QueryScreenCursorLocationEventHandler QueryScreenCursorLocation;

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

            if (candidatesAreShown()) {
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
                    candidatesHide();
                }
            }

            if (e.Handled) return;
            
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
                SetSelection(selStart, selEnd);

                if (candidatesAreShown() && (SelectionStart < _candKeyStart || SelectionLength > 0)) {
                    candidatesHide();
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
                SetSelection(selStart, selEnd);

                if (candidatesAreShown() && (SelectionStart < _candKeyStart || SelectionLength > 0)) {
                    candidatesHide();
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
                candidatesHide();
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
                candidatesHide();
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
                candidatesUpdate();
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
                candidatesUpdate();
            }
            else if (e.Modifiers == Keys.None && e.KeyCode == Keys.Escape) {
                e.Handled = true;
                candidatesHide();
            }
            else if (e.Modifiers == Keys.Control) {
                e.SuppressKeyPress = true;
                if (e.KeyCode == Keys.C) {
                    e.Handled = true;
                    candidatesHide();
                    Copy();
                }
                else if (e.KeyCode == Keys.X) {
                    e.Handled = true;
                    candidatesHide();
                    Cut();
                }
                else if (e.KeyCode == Keys.V) {
                    e.Handled = true;
                    candidatesHide();
                    Paste();
                }
                else if (e.KeyCode == Keys.A) {
                    e.Handled = true;
                    candidatesHide();
                    SelectAll();
                }
                else if (e.KeyCode == Keys.Z) {
                    e.Handled = true;
                    candidatesHide();
                    Undo();
                }
                else if (e.KeyCode == Keys.Space) {
                    if (Settings.Instance.Input_IdAutoCompletion) {
                        e.Handled = true;
                        //showCandidates();
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
                if (!isIdChar(prevChar) && prevChar != '\'' && prevChar != '\"' && prevChar != '#' && prevChar != '\\' && isFirstIdChar(e.KeyChar)) {
                    if (Settings.Instance.Input_IdAutoCompletion) {
                        // 識別子の先頭文字が入力されたら補完候補を表示する
                        candidatesShow();
                    }
                }
                else if (_candKeyStart + 1 < selStart && isIdChar(e.KeyChar)) {
                    // 識別子の2文字目以降が表示されたら補完候補を更新する
                    candidatesUpdate();
                }
                else {
                    // 識別子の文字以外が表示されたら補間をキャンセルする
                    candidatesHide();
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

        private bool isFirstIdChar(char c) => ('a' <= c && c <= 'z') || ('A' <= c && c <= 'Z') || c == '_';
        private bool isIdChar(char c) => isFirstIdChar(c) || ('0' <= c && c <= '9');

        public void SetSelection(int selStart) {
            SetSelection(selStart, selStart);
        }

        public void SetSelection(int selStart, int selEnd) {
            selStart = Math.Max(0, Math.Min(Text.Length, selStart));
            selEnd = Math.Max(0, Math.Min(Text.Length, selEnd));
            if (selStart == _selStart && selEnd == _selEnd) return;
            _selStart = selStart;
            _selEnd = selEnd;
            CursorStateChanged?.Invoke(this, EventArgs.Empty);
        }

        private bool candidatesAreShown() => (_candForm != null);

        private void candidatesShow() {
            if (CandidateProvider == null) return;
            if (!candidatesAreShown()) {
                _candForm = new InputCandidateForm(CandidateProvider);
                _candForm.Visible = true;
                candidatesSelectKey();
                var e = new QueryScreenCursorLocationEventArgs(_candKeyStart);
                QueryScreenCursorLocation?.Invoke(this, e);
                _candForm.Location = e.Result;
                candidatesSetKey();
            }
            else {
                candidatesSelectKey();
                candidatesSetKey();
            }
        }

        private void candidatesHide() {
            if (candidatesAreShown()) {
                _candForm.Dispose();
                _candForm = null;
            }
        }

        private void candidatesUpdate() {
            if (candidatesAreShown()) {
                candidatesSelectKey();
                candidatesSetKey();
            }
        }

        private void candidatesSetKey() {
            if (candidatesAreShown()) {
                _candForm.SetKey(Text.Substring(_candKeyStart, _candKeyEnd - _candKeyStart));
            }
        }

        private void candidatesSelectKey() {
            var text = Text;
            var selStart = SelectionStart;
            var candStart = selStart;
            var candEnd = selStart;
            while (candStart > 0 && isIdChar(text[candStart - 1])) {
                candStart--;
            }
            _candKeyStart = candStart;
            _candKeyEnd = candEnd;
        }

    }
}
