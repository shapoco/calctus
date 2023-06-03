using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Globalization;
using Shapoco.Calctus.Model;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Sheets;
using Shapoco.Calctus.Model.Expressions;

namespace Shapoco.Calctus.UI.Sheets {
    class SheetView : GdiControl, IInputCandidateProvider {
        public event EventHandler RadixModeChanged;
        public event EventHandler DialogOpening;
        public event EventHandler DialogClosed;

        private Sheet _sheet = null;
        private int _focusedIndex = -1;
        private RpnOperation _focusedRpnOperation = null;
        private bool _recalcRequested = true;
        private bool _layoutValidated = false;
        private GdiBox _innerBox;
        private VScrollBar _scrollBar = new VScrollBar();
        private bool _disposed = false;
        
        private float _indentRatio = 0.3f;
        private int _equalWidth = 10;

        private RadixMode _activeRadixMode = RadixMode.Auto;

        private ContextMenuStrip _ctxMenu = new ContextMenuStrip();
        private ToolStripMenuItem _cmenuTextCut = new ToolStripMenuItem("Cut Text");
        private ToolStripMenuItem _cmenuTextCopy = new ToolStripMenuItem("Copy Text");
        private ToolStripMenuItem _cmenuTextPaste = new ToolStripMenuItem("Paste Text");
        private ToolStripMenuItem _cmenuTextDelete = new ToolStripMenuItem("Delete Text");
        private ToolStripSeparator _cmenuSep0 = new ToolStripSeparator();
        private ToolStripMenuItem _cmenuCopyAll = new ToolStripMenuItem("Copy All");
        private ToolStripSeparator _cmenuSep1 = new ToolStripSeparator();
        private ToolStripMenuItem _cmenuInsertTime = new ToolStripMenuItem("Insert Current Time");
        private ToolStripSeparator _cmenuSep2 = new ToolStripSeparator();
        private ToolStripMenuItem _cmenuMoveUp = new ToolStripMenuItem("Move Up");
        private ToolStripMenuItem _cmenuMoveDown = new ToolStripMenuItem("Move Down");
        private ToolStripSeparator _cmenuSep3 = new ToolStripSeparator();
        private ToolStripMenuItem _cmenuItemInsert = new ToolStripMenuItem("Insert Item");
        private ToolStripMenuItem _cmenuItemDelete = new ToolStripMenuItem("Delete Item");
        private ToolStripSeparator _cmenuTextSep2 = new ToolStripSeparator();
        private ToolStripMenuItem _cmenuClear = new ToolStripMenuItem("Clear");

        private InputCandidate[] _inputCandidates = new InputCandidate[0];

        public SheetView() {
            if (DesignMode) return;
            DoubleBuffered = true;

            _innerBox = new GdiBox(this);
            RootBox.Children.Add(_innerBox);

            _scrollBar.Dock = DockStyle.Right;
            _scrollBar.Visible = false;
            _scrollBar.ValueChanged += (sender, e) => { _innerBox.Top = - _scrollBar.Value; Invalidate(); };
            Controls.Add(_scrollBar);

            Sheet = new Sheet();
            Sheet.Items.Add(new SheetItem());
            FocusedIndex = 0;

            _cmenuTextCut.ShortcutKeyDisplayString = "Ctrl+X";
            _cmenuTextCopy.ShortcutKeyDisplayString = "Ctrl+C";
            _cmenuTextPaste.ShortcutKeyDisplayString = "Ctrl+V";
            _cmenuCopyAll.ShortcutKeyDisplayString = "Ctrl+Shift+C";
            _cmenuInsertTime.ShortcutKeyDisplayString = "Ctrl+Shift+N";
            _cmenuMoveUp.ShortcutKeyDisplayString = "Ctrl+Shift+Up";
            _cmenuMoveDown.ShortcutKeyDisplayString = "Ctrl+Shift+Down";
            _cmenuItemInsert.ShortcutKeyDisplayString = "Shift+Enter";
            _cmenuItemDelete.ShortcutKeyDisplayString = "Shift+Del";
            _cmenuClear.ShortcutKeyDisplayString = "Ctrl+Shift+Del";

            _cmenuTextCut.Click += (sender, e) => { getFocusedExprBox()?.Cut(); };
            _cmenuTextCopy.Click += (sender, e) => { Copy(); };
            _cmenuTextPaste.Click += (sender, e) => { Paste(); };
            _cmenuTextDelete.Click += (sender, e) => { getFocusedExprBox()?.Delete(); };
            _cmenuCopyAll.Click += (sender, e) => { CopyAll(); };
            _cmenuInsertTime.Click += (sender, e) => { getFocusedExprBox()?.InsertCurrentTime(); };
            _cmenuMoveUp.Click += (sender, e) => { ItemMoveUp(); };
            _cmenuMoveDown.Click += (sender, e) => { ItemMoveDown(); };
            _cmenuItemInsert.Click += (sender, e) => { ItemInsert(); };
            _cmenuItemDelete.Click += (sender, e) => { ItemDelete(); };
            _cmenuClear.Click += (sender, e) => { Clear(); };

            _ctxMenu.Items.AddRange(new ToolStripItem[] {
                _cmenuTextCut,
                _cmenuTextCopy,
                _cmenuTextPaste,
                _cmenuTextDelete,
                _cmenuSep0,
                _cmenuCopyAll,
                _cmenuSep1,
                _cmenuInsertTime,
                _cmenuSep2,
                _cmenuMoveUp,
                _cmenuMoveDown,
                _cmenuSep3,
                _cmenuItemInsert,
                _cmenuItemDelete,
                _cmenuTextSep2,
                _cmenuClear
            });
        }

        [Browsable(false)]
        [DefaultValue(null)]
        public Sheet Sheet {
            get => _sheet;
            set {
                if (value == _sheet) return;
                if (_sheet != null) {
                    _sheet.Items.CollectionChanged -= Items_CollectionChanged;
                    _sheet.PreviewExecute -= Sheet_PreviewExecute;
                    foreach(var noteItem in _sheet.Items) {
                        unlinkSheetItem(noteItem);
                    }
                }
                _sheet = value;
                if (_sheet != null) {
                    _sheet.Items.CollectionChanged += Items_CollectionChanged;
                    _sheet.PreviewExecute += Sheet_PreviewExecute;
                    foreach (var noteItem in _sheet.Items) {
                        linkSheetItem(noteItem);
                    }
                }
                Invalidate();
            }
        }

        public int FocusedIndex {
            get => _focusedIndex;
            set {
                if (value == _focusedIndex) return;
                focusViewItem(value);
            }
        }

        public SheetViewItem FocusedViewItem {
            get => (_focusedIndex >= 0) ? getViewItem(_focusedIndex) : null;
            set => FocusedIndex = indexOf(value);
        }

        public RadixMode ActiveRadixMode {
            get => _activeRadixMode;
            set {
                if (value == _activeRadixMode) return;
                _activeRadixMode = value;
                var viewItem = FocusedViewItem;
                if (viewItem!= null) {
                    viewItem.SheetItem.RadixMode = value;
                }
                RadixModeChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public int Indent {
            get => (int)(_indentRatio * (ClientSize.Width - _scrollBar.Width));
        }

        public int EqualWidth {
            get => _equalWidth;
        }

        public void InvalidateLayout() {
            _layoutValidated = false;
            Invalidate();
        }

        public void InsertExpr(string expr) {
            var insertPos = FocusedIndex;
            if (insertPos < 0) insertPos = _sheet.Items.Count;
            if (insertPos < _sheet.Items.Count && string.IsNullOrEmpty(_sheet.Items[insertPos].ExprText)) {
                _sheet.Items[insertPos].ExprText = expr;
            }
            else {
                _sheet.Items.Insert(insertPos, new SheetItem(expr));
            }
            _sheet.Items.Insert(insertPos + 1, new SheetItem());
            focusViewItem(insertPos + 1);
        }

        public void ItemMoveUp() {
            var selIndex = FocusedIndex;
            if (selIndex > 0) { 
                _sheet.Items.Move(selIndex, selIndex - 1);
                FocusedIndex = selIndex - 1;
            }
        }

        public void ItemMoveDown() {
            var selIndex = FocusedIndex;
            if (selIndex < _sheet.Items.Count - 1) {
                _sheet.Items.Move(selIndex, selIndex + 1);
                FocusedIndex = selIndex + 1;
            }
        }

        public void ItemInsert() {
            var insIndex = FocusedIndex;
            if (insIndex < 0) {
                insIndex = _sheet.Items.Count;
            }
            _sheet.Items.Insert(insIndex, new SheetItem());
            focusViewItem(insIndex);
        }

        public void ItemDelete() {
            int selIndex = FocusedIndex;
            _sheet.Items.RemoveAt(selIndex);
            if (_sheet.Items.Count == 0) {
                _sheet.Items.Add(new SheetItem());
            }
            if (selIndex < _sheet.Items.Count) {
                focusViewItem(selIndex);
            }
            else {
                focusViewItem(selIndex - 1);
            }
        }

        public void Copy() => getFocusedExprBox()?.Copy();

        public void CopyAll() {
            var sb = new StringBuilder();
            foreach (var item in _sheet.Items) {
                sb.Append(item.ExprText).Append(" = ").AppendLine(item.AnsVal.ToString());
            }
            try {
                Clipboard.Clear();
                Clipboard.SetText(sb.ToString());
            }
            catch {
                System.Media.SystemSounds.Beep.Play();
            }
        }

        public void Paste() {
            var viewItem = FocusedViewItem;
            if (viewItem == null) return;
            try {
                var text = Clipboard.GetText();
                if (text.IndexOf("\n") > 0) {
                    MultilinePaste();
                }
                else {
                    getFocusedExprBox()?.Paste();
                }
            }
            catch { }
        }

        public void MultilinePaste() {
            int insertPos = FocusedIndex;
            if (insertPos < 0) insertPos = _sheet.Items.Count;

            var dlg = new PasteOptionForm();
            DialogOpening?.Invoke(this, EventArgs.Empty);
            if (dlg.ShowDialog() == DialogResult.OK) {
                var lines = dlg.TextWillBePasted.Split('\n');
                for (int i = 0; i < lines.Length; i++) {
                    var line = lines[i].Replace("\r", "");
                    if (i == 0 && insertPos < _sheet.Items.Count && string.IsNullOrEmpty(_sheet.Items[insertPos].ExprText)) {
                        // 先頭行については挿入先の行が空行の場合はそこを置き換える
                        _sheet.Items[insertPos++].ExprText = line;
                    }
                    else {
                        _sheet.Items.Insert(insertPos++, new SheetItem(line));
                    }
                }
                focusViewItem(insertPos - 1);
            }
            dlg.Dispose();
            DialogClosed?.Invoke(this, EventArgs.Empty);
        }

        public void Clear() {
            var ans = MessageBox.Show("Are you sure you want to delete all?", Application.ProductName, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            if (ans == DialogResult.OK) {
                _sheet.Items.Clear();
                _sheet.Items.Add(new SheetItem());
                focusViewItem(0);
            }
        }

        public void PageUp() {
            var selItem = FocusedViewItem;
            if (selItem != null) {
                int y = selItem.Bottom - ClientSize.Height;
                FocusedIndex = indexFromY(y);
            }
        }

        public void PageDown() {
            var selItem = FocusedViewItem;
            if (selItem != null) {
                int y = selItem.Top + ClientSize.Height;
                FocusedIndex = indexFromY(y);
            }
        }

        public void RequestRecalc() {
            _recalcRequested = true;
            Invalidate();
        }

        public IEnumerable<InputCandidate> Candidates => _inputCandidates;

        protected override void OnFocusedBoxChanged() {
            base.OnFocusedBoxChanged();
            // フォーカスされたボックスが所属するアイテムを選択状態にする
            var viewItem = getViewItem(FocusedBox);
            if (viewItem != null) {
                FocusedIndex = indexOf(viewItem);
            }
            else {
                FocusedIndex = -1;
            }
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
            int selIndex = FocusedIndex;
            if (selIndex < 0) return;
            var viewItem = (SheetViewItem)_sheet.Items[selIndex].Tag;

            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.V) {
                e.Handled = true;
                Paste();
            }
            else if (e.Modifiers == (Keys.Control | Keys.Shift) && e.KeyCode == Keys.V) {
                e.Handled = true;
                MultilinePaste();
            }
            if (e.Handled) return;

            base.OnKeyDown(e);
            if (e.Handled) return;

            if (e.Modifiers == Keys.None && e.KeyCode == Keys.Return) { 
                var ans = viewItem.AnsBox.Text;

                // Return
                var rpn = parseRpnOperation(selIndex);
                if (rpn != null) {
                    // RPNコマンドの実行
                    if (rpn.Error != null) {
                        return;
                    }

                    for (int i = rpn.StartIndex; i < rpn.EndIndex; i++) {
                        _sheet.Items.RemoveAt(rpn.StartIndex);
                    }
                    _sheet.Items[rpn.StartIndex].ExprText = rpn.Expression;
                    selIndex = rpn.StartIndex;
                }

                e.Handled = true;
                var newSheetItem = new SheetItem();
                newSheetItem.RadixMode = viewItem.SheetItem.RadixMode;
                _sheet.Items.Insert(selIndex + 1, newSheetItem);
                var newViewItem = (SheetViewItem)newSheetItem.Tag;
                newViewItem.ExprBox.Text = ans;
                newViewItem.ExprBox.SelectAll();
                newViewItem.IsFreshAnswer = true;
                focusViewItem(selIndex + 1);
            }
            else if (e.Modifiers == Keys.Shift && e.KeyCode == Keys.Return) {
                e.Handled = true;
                var newItem = new SheetItem();
                newItem.RadixMode = viewItem.SheetItem.RadixMode;
                _sheet.Items.Insert(selIndex, newItem);
                focusViewItem(selIndex);
            }
            else if (e.Modifiers == Keys.None && e.KeyCode == Keys.Up) {
                if (selIndex > 0) {
                    e.Handled = true;
                    focusViewItem(selIndex - 1);
                }
            }
            else if (e.Modifiers == Keys.None && e.KeyCode == Keys.Down) {
                if (selIndex < _sheet.Items.Count - 1) {
                    e.Handled = true;
                    focusViewItem(selIndex + 1);
                }
            }
            else if (e.Modifiers == (Keys.Control | Keys.Shift) && e.KeyCode == Keys.Up) {
                e.Handled = true;
                ItemMoveUp();
            }
            else if (e.Modifiers == (Keys.Control | Keys.Shift) && e.KeyCode == Keys.Down) {
                e.Handled = true;
                ItemMoveDown();
            }
            else if (e.Modifiers == Keys.None && e.KeyCode == Keys.PageUp) {
                e.Handled = true;
                PageUp();
            }
            else if (e.Modifiers == Keys.None && e.KeyCode == Keys.PageDown) {
                e.Handled = true;
                PageDown();
            }
            else if (e.Modifiers == (Keys.Control | Keys.Shift) && e.KeyCode == Keys.C) {
                e.Handled = true;
                CopyAll();
            }
            else if (e.Modifiers == Keys.Shift && e.KeyCode == Keys.Delete) {
                e.Handled = true;
                ItemDelete();
            }
            else if (e.Modifiers == (Keys.Control | Keys.Shift) && e.KeyCode == Keys.Delete) {
                e.Handled = true;
                Clear();
            }
            else if (e.Modifiers == (Keys.Control | Keys.Shift) && e.KeyCode == Keys.N) {
                e.Handled = true;
                _cmenuInsertTime.PerformClick();
            }
        }

        protected override void OnKeyUp(KeyEventArgs e) {
            base.OnKeyUp(e);
            if (e.Modifiers == Keys.None && e.KeyCode == Keys.Apps) {
                e.Handled = true;
                var box = FocusedBox;
                if (box != null) {
                    openContextMenu(box.PointToScreen(new Point(0, box.Height)));
                }
                else {
                    openContextMenu(PointToScreen(new Point(0, Height)));
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Right) {
                openContextMenu(Cursor.Position);
            }
        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            InvalidateLayout();
        }

        protected override void OnPaint(PaintEventArgs e) {
            processRecalcRequest();
            validateLayout(e.Graphics);

            int n = _sheet.Items.Count;
            for (int i = 0; i < n; i++) {
                var viewItem = getViewItem(i);
                if (_focusedRpnOperation != null) {
                    if (_focusedRpnOperation.StartIndex <= i && i < _focusedRpnOperation.EndIndex) {
                        viewItem.BackColor = Color.FromArgb(0, 0, 64);
                    }
                }
                else if (i == _focusedIndex) {
                    viewItem.BackColor = Color.Black;
                }
                else {
                    viewItem.BackColor = Color.Transparent;
                }
            }

            base.OnPaint(e);
        }

        protected override void OnFontChanged(EventArgs e) {
            base.OnFontChanged(e);
            if (_sheet == null) return;
            foreach (var noteItem in _sheet.Items) {
                ((SheetViewItem)noteItem.Tag).RelayoutText();
            }
        }

        protected override void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
                    foreach (var sheetItem in _sheet.Items) {
                        unlinkSheetItem(sheetItem);
                    }
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }

        private void Sheet_PreviewExecute(Sheet sender, PreviewExecuteEventArgs e) {
            if (e.Index == FocusedIndex) {
                var rpnOp = parseRpnOperation(e.Index);
                if (rpnOp != null) {
                    // 式が選択中でかつ RPN操作として解釈できる場合は RPN操作を実行する
                    e.Overrided = true;
                    if (rpnOp.Error == null) {
                        try {
                            var expr = Parser.Parse(rpnOp.Expression);
                            e.Answer = expr.Eval(e.Context);
                            e.EvalError = null;
                        }
                        catch (Exception ex) {
                            e.Answer = NullVal.Instance;
                            e.EvalError = ex;
                        }
                    }
                    else {
                        e.Answer = NullVal.Instance;
                        e.SyntaxError = rpnOp.Error;
                    }
                }
            }
        }

        private RpnOperation parseRpnOperation(int index) {
            if (index < 0 || _sheet.Items.Count <= index) return null;
            
            // 式が演算子のみで構成されている場合は RPN操作とみなす
            if (!Lexer.TryGetRpnSymbols(_sheet.Items[index].ExprText, out Token[] symbols)) {
                return null;
            }

            Exception err = null;
            string expr = null;
            if (index <= symbols.Length) {
                err = new CalctusError("Invalid RPN Operation");
            }

            int start = index - symbols.Length - 1;
            int end = index;

            try {
                string rightStr = _sheet.Items[end - 1].ExprText;
                Expr rightExpr = Parser.Parse(rightStr);
                if (err == null) {
                    for (int i = 0; i < symbols.Length; i++) {
                        var sym = symbols[i];
                        string leftStr = _sheet.Items[end - 2 - i].ExprText;
                        Expr leftExpr = Parser.Parse(leftStr);

                        var op = new BinaryOp(sym, leftExpr, rightExpr);
                        if (leftExpr is Operator leftOp) {
                            if (leftOp.Method.Priority <= op.Method.Priority) {
                                leftStr = "(" + leftStr + ")";
                            }
                        }
                        if (rightExpr is Operator rightOp) {
                            if (op.Method.Priority >= rightOp.Method.Priority) {
                                rightStr = "(" + rightStr + ")";
                            }
                        }

                        rightStr = leftStr + sym.Text + rightStr;
                        rightExpr = op;
                    }
                }
                expr = rightStr;
            }
            catch (Exception ex) {
                err = ex;
            }

            return new RpnOperation(start, end, expr, err);
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            switch(e.Action) {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems) {
                        linkSheetItem((SheetItem)item);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems) {
                        unlinkSheetItem((SheetItem)item);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (var item in e.OldItems) {
                        unlinkSheetItem((SheetItem)item);
                    }
                    foreach (var item in e.NewItems) {
                        linkSheetItem((SheetItem)item);
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    foreach (var child in _innerBox.Children.ToArray()) {
                        if (child is SheetViewItem viewItem) {
                            unlinkSheetItem(viewItem.SheetItem);
                        }
                    }
                    break;
            }
            
            if (_focusedIndex >= _sheet.Items.Count) {
                focusViewItem(_sheet.Items.Count - 1);
            }

            RequestRecalc();
            InvalidateLayout();
        }

        private void linkSheetItem(SheetItem noteItem) {
            var viewItem = new SheetViewItem(this, noteItem);
            noteItem.ExpressionChanged += SheetItem_ExpressionChanged;
            noteItem.Tag = viewItem;
            _innerBox.Children.Add(viewItem);
        }

        private void unlinkSheetItem(SheetItem noteItem) {
            var viewItem = (SheetViewItem)noteItem.Tag;
            _innerBox.Children.Remove(viewItem);
            noteItem.ExpressionChanged -= SheetItem_ExpressionChanged;
            viewItem.Dispose();
            noteItem.Tag = null;
        }

        private void SheetItem_ExpressionChanged(object sender, EventArgs e) {
            var senderItem = (SheetItem)sender;
            var focusedIndex = FocusedIndex;
            if (focusedIndex >= 0 && getViewItem(focusedIndex).SheetItem == senderItem) {
                _focusedRpnOperation = parseRpnOperation(focusedIndex);
            }
            RequestRecalc();
        }

        private void ViewItem_RepaintRequest(object sender, EventArgs e) {
            Invalidate();
        }

        private void openContextMenu(Point screenPos) {
            var selIndex = FocusedIndex;
            var focusBox = getFocusedExprBox();
            _cmenuTextCut.Enabled = focusBox != null && !focusBox.ReadOnly && focusBox.SelectionLength > 0;
            _cmenuTextCopy.Enabled = focusBox != null;
            _cmenuTextPaste.Enabled = focusBox != null && !focusBox.ReadOnly;
            _cmenuTextDelete.Enabled = focusBox != null && !focusBox.ReadOnly && focusBox.SelectionLength > 0;
            _cmenuMoveUp.Enabled = selIndex > 0;
            _cmenuMoveDown.Enabled = selIndex >= 0 && selIndex < _sheet.Items.Count - 1;
            _cmenuItemDelete.Enabled = selIndex >= 0;
            _ctxMenu.Show(screenPos);
        }

        private void focusViewItem(int index) {
            if (_focusedRpnOperation != null) {
                // RPN操作が確定されないままフォーカスが外されたら文法エラーにするために再計算を要求する
                RequestRecalc();
            }

            _focusedIndex = index;
            if (_focusedIndex >= 0) {
                // 選択されたアイテムに所属するボックスがフォーカスされてなければフォーカスする
                var newViewItem = getViewItem(_focusedIndex);
                var oldViewItem = getViewItem(FocusedBox);
                if (newViewItem != oldViewItem) {
                    newViewItem.ExprBox.Focus();
                }

                _focusedRpnOperation = parseRpnOperation(_focusedIndex);
                if (_focusedRpnOperation != null) {
                    // 新たに選択されたアイテムが RPN操作として解釈される場合は再計算を要求する
                    RequestRecalc();
                }

                ActiveRadixMode = newViewItem.SheetItem.RadixMode;

                scrollTo(_focusedIndex);
            }
#if DEBUG
            Console.WriteLine("SelectedIndex = " + _focusedIndex);
#endif
        }

        // 指定の ViewItem のインデックスを返す
        private int indexOf(SheetViewItem viewItem) => (viewItem != null) ? _sheet.Items.IndexOf(viewItem.SheetItem) : -1;

        // 指定のインデックスの ViewItem を返す
        private SheetViewItem getViewItem(int index) {
            return (SheetViewItem)_sheet.Items[index].Tag;
        }

        // Box が所属する ViewItem を返す
        private SheetViewItem getViewItem(GdiBox box) {
            GdiBox p = box;
            while (p != null) {
                if (p is SheetViewItem viewItem) {
                    return viewItem;
                }
                p = p.Parent;
            }
            return null;
        }

        // クライアントY座標からインデックスを返す
        private int indexFromY(int y) {
            if (_sheet.Items.Count == 0) return -1;
            int n = _sheet.Items.Count;
            for (int i = 0; i < n; i++) {
                var viewItem = getViewItem(i);
                if (y < _innerBox.Top + viewItem.Bottom) {
                    return i;
                }
            }
            return _sheet.Items.Count - 1;
        }

        private ExprBoxCore getFocusedExprBox() {
            var viewItem = FocusedViewItem;
            if (viewItem == null) return null;
            if (viewItem.ExprBox.Focused) return viewItem.ExprBox;
            if (viewItem.AnsBox.Focused) return viewItem.AnsBox;
            return null;
        }

        private void scrollTo(int index) {
            if (index < 0 || index >= _sheet.Items.Count) return;
            validateLayout();
            var client = ClientSize;
            var viewItem = getViewItem(index);

            if (_innerBox.Top + viewItem.Top < 0) {
                _scrollBar.Value = viewItem.Top;
            }
            else if (_innerBox.Top + viewItem.Bottom > client.Height) {
                _scrollBar.Value = viewItem.Bottom - client.Height;
            }
        }

        private void validateLayout() {
            if (_layoutValidated) return;
            using (var g = CreateGraphics()) {
                validateLayout(g);
            }
        }

        private void validateLayout(Graphics g) {
            if (_layoutValidated) return;
#if DEBUG
            Console.WriteLine("Relayout");
#endif
            _equalWidth = (int)g.MeasureString("==", Font).Width;

            // 行の配置
            var client = ClientSize;
            int w = client.Width - _scrollBar.Width;
            int y = 0;
            int n = _sheet.Items.Count;
            _innerBox.SetBounds(0, 0, w, 0);
            for (int i = 0; i < n; i++) {
                var sheetItem = _sheet.Items[i];
                var viewItem = (SheetViewItem)sheetItem.Tag;
                var size = viewItem.GetPreferredSize();
                viewItem.SetBounds(0, y, w, size.Height);
                viewItem.TabIndex = i;
                y += viewItem.Height;
            }
            _innerBox.Height = y;

            // スクロールバーの調整
            if (y > client.Height) {
                _scrollBar.Visible = true;
                _scrollBar.Maximum = y;
                _scrollBar.LargeChange = client.Height;
            }
            else {
                _scrollBar.Visible = false;
            }
            _layoutValidated = true;
        }

        private void processRecalcRequest() {
            if (!_recalcRequested) return;
#if DEBUG
            Console.WriteLine("Recalc");
#endif
            var ctx = _sheet.Run();
            ctx.Undef(Sheet.LastAnsId, true);

            // 入力補完候補の列挙
            var list = new List<InputCandidate>();
            foreach (var f in FuncDef.EnumAllFunctions()) {
                list.Add(new InputCandidate(f.Name, f.ToString(), f.Description, true));
            }
            foreach (var v in ctx.EnumVars()) {
                if (!(v.Value is FuncVal)) {
                    list.Add(new InputCandidate(v.Name.Text, v.Name.Text, v.Description, false));
                }
            }
            foreach(var f in ctx.EnumUserFuncs()) {
                list.Add(new InputCandidate(f.Name, f.ToString(), f.Description, true));
            }
            list.Add(new InputCandidate(Sheet.LastAnsId, Sheet.LastAnsId, "last answer", false));
            list.Add(new InputCandidate(BoolVal.TrueKeyword, BoolVal.TrueKeyword, "true value", false));
            list.Add(new InputCandidate(BoolVal.FalseKeyword, BoolVal.FalseKeyword, "false value", false));
            list.Add(new InputCandidate("def", "def", "user function definition", false));
            list.Add(new InputCandidate("solve", "solve(expr,var,a,b)", "Solves the equation using Newton's method.", true));
            _inputCandidates = list.OrderBy(p => p.Id).ToArray();
            _recalcRequested = false;
        }

    }
}
