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
using System.IO;
using Shapoco.Calctus.Model;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Sheets;
using Shapoco.Calctus.Model.Expressions;
using Shapoco.Calctus.Model.Functions;

namespace Shapoco.Calctus.UI.Sheets {
    class SheetView : GdiControl, IInputCandidateProvider {
        public event EventHandler DialogOpening;
        public event EventHandler DialogClosed;
        public event EventHandler Changed;
        public event EventHandler BeforeCleared;

        private Sheet _sheet = null;
        private SheetOperator _operator = null;
        private int _focusedIndex = -1;
        private RpnOperation _focusedRpnOperation = null;
        private bool _recalcRequested = true;
        private bool _layoutValidated = false;
        private GdiBox _innerBox;
        private VScrollBar _scrollBar = new VScrollBar();
        private bool _disposed = false;

        private int _equalWidth = 10;
        private int _equalPosition = 100;

        private ContextMenuStrip _ctxMenu = new ContextMenuStrip();
        private ToolStripMenuItem _cmenuUndo = new ToolStripMenuItem("Undo");
        private ToolStripMenuItem _cmenuRedo = new ToolStripMenuItem("Redo");
        private ToolStripMenuItem _cmenuTextCut = new ToolStripMenuItem("Cut text");
        private ToolStripMenuItem _cmenuTextCopy = new ToolStripMenuItem("Copy text");
        private ToolStripMenuItem _cmenuPickupValues = new ToolStripMenuItem("Pickup values using clipboard");
        private ToolStripMenuItem _cmenuTextPaste = new ToolStripMenuItem("Paste text");
        private ToolStripMenuItem _cmenuTextDelete = new ToolStripMenuItem("Delete text");
        private ToolStripMenuItem _cmenuCopyAll = new ToolStripMenuItem("Copy all");
        private ToolStripMenuItem _cmenuInsertTime = new ToolStripMenuItem("Insert current time");
        private ToolStripMenuItem _cmenuMoveUp = new ToolStripMenuItem("Move up");
        private ToolStripMenuItem _cmenuMoveDown = new ToolStripMenuItem("Move down");
        private ToolStripMenuItem _cmenuItemInsert = new ToolStripMenuItem("Insert line");
        private ToolStripMenuItem _cmenuItemDelete = new ToolStripMenuItem("Delete line");
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
            _scrollBar.ValueChanged += (sender, e) => { _innerBox.Top = -_scrollBar.Value; Invalidate(); };
            Controls.Add(_scrollBar);

            FocusedIndex = 0;

            _cmenuUndo.ShortcutKeyDisplayString = "Ctrl+Z";
            _cmenuRedo.ShortcutKeyDisplayString = "Ctrl+Y";
            _cmenuTextCut.ShortcutKeyDisplayString = "Ctrl+X";
            _cmenuTextCopy.ShortcutKeyDisplayString = "Ctrl+C";
            _cmenuTextPaste.ShortcutKeyDisplayString = "Ctrl+V";
            _cmenuCopyAll.ShortcutKeyDisplayString = "Ctrl+Shift+C";
            _cmenuPickupValues.ShortcutKeyDisplayString = "Alt+C";
            _cmenuInsertTime.ShortcutKeyDisplayString = "Ctrl+Shift+N";
            _cmenuMoveUp.ShortcutKeyDisplayString = "Ctrl+Shift+Up";
            _cmenuMoveDown.ShortcutKeyDisplayString = "Ctrl+Shift+Down";
            _cmenuItemInsert.ShortcutKeyDisplayString = "Shift+Enter";
            _cmenuItemDelete.ShortcutKeyDisplayString = "Shift+Del";
            _cmenuClear.ShortcutKeyDisplayString = "Ctrl+Shift+Del";

            _cmenuUndo.Click += (sender, e) => { Undo(); };
            _cmenuRedo.Click += (sender, e) => { Redo(); };
            _cmenuTextCut.Click += (sender, e) => { getFocusedExprBox()?.Cut(); };
            _cmenuTextCopy.Click += (sender, e) => { Copy(); };
            _cmenuTextPaste.Click += (sender, e) => { Paste(); };
            _cmenuPickupValues.Click += (sender, e) => { PickupValuesUsingClipboard(); };
            _cmenuTextDelete.Click += (sender, e) => { getFocusedExprBox()?.Delete(); };
            _cmenuCopyAll.Click += (sender, e) => { CopyAll(); };
            _cmenuInsertTime.Click += (sender, e) => { getFocusedExprBox()?.InsertCurrentTime(); };
            _cmenuMoveUp.Click += (sender, e) => { ItemMoveUp(); };
            _cmenuMoveDown.Click += (sender, e) => { ItemMoveDown(); };
            _cmenuItemInsert.Click += (sender, e) => { ItemInsert(); };
            _cmenuItemDelete.Click += (sender, e) => { ItemDelete(); };
            _cmenuClear.Click += (sender, e) => { Clear(); };

            _ctxMenu.Items.AddRange(new ToolStripItem[] {
                _cmenuUndo,
                _cmenuRedo,
                 new ToolStripSeparator(),
                _cmenuTextCut,
                _cmenuTextCopy,
                _cmenuTextPaste,
                _cmenuTextDelete,
                 new ToolStripSeparator(),
                _cmenuCopyAll,
                 new ToolStripSeparator(),
                _cmenuPickupValues,
                 new ToolStripSeparator(),
                _cmenuInsertTime,
                 new ToolStripSeparator(),
                _cmenuMoveUp,
                _cmenuMoveDown,
                 new ToolStripSeparator(),
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
                    foreach (var noteItem in _sheet.Items) {
                        unlinkSheetItem(noteItem);
                    }
                    _operator.Changed -= _operator_Changed;
                    _operator.Dispose();
                }
                _sheet = value;
                _operator = null;
                if (_sheet != null) {
                    _sheet.Items.CollectionChanged += Items_CollectionChanged;
                    _sheet.PreviewExecute += Sheet_PreviewExecute;
                    foreach (var noteItem in _sheet.Items) {
                        linkSheetItem(noteItem);
                    }
                    _operator = new SheetOperator(this);
                    if (_sheet.Items.Count > 0) {
                        FocusViewItem(_sheet.Items.Count - 1);
                    }
                    _operator.Changed += _operator_Changed;
                }
                else {
                    _focusedIndex = -1;
                }
                InvalidateLayout();
            }
        }

        public SheetOperator Operator => _operator;

        [Browsable(false)]
        [DefaultValue(-1)]
        public int FocusedIndex {
            get => _focusedIndex;
            set {
                if (value == _focusedIndex) return;
                FocusViewItem(value);
            }
        }

        [Browsable(false)]
        [DefaultValue(null)]
        public SheetViewItem FocusedViewItem {
            get => (_focusedIndex >= 0) ? GetViewItem(_focusedIndex) : null;
            set => FocusedIndex = IndexOf(value);
        }

        public int EqualPosition {
            get => _equalPosition;
            set => _equalPosition = value;
        }

        public int EqualWidth {
            get => _equalWidth;
        }

        public void InvalidateLayout() {
            _layoutValidated = false;
            Invalidate();
#if DEBUG
            Console.WriteLine("Layout Invalidated");
#endif
        }

        public void ItemMoveUp() {
            if (_sheet == null) return;
            CandidateHide();
            var selIndex = FocusedIndex;
            if (selIndex > 0) {
                _operator.Move(selIndex, selIndex - 1);
                FocusedIndex = selIndex - 1;
            }
        }

        public void ItemMoveDown() {
            if (_sheet == null) return;
            CandidateHide();
            var selIndex = FocusedIndex;
            if (selIndex < _sheet.Items.Count - 1) {
                _operator.Move(selIndex, selIndex + 1);
                FocusedIndex = selIndex + 1;
            }
        }

        public void ItemInsert() {
            if (_sheet == null) return;
            CandidateHide();
            var insIndex = FocusedIndex;
            if (insIndex < 0) {
                insIndex = _sheet.Items.Count;
            }
            _operator.Insert(insIndex, "");
            FocusViewItem(insIndex);
        }

        public void ItemDelete() {
            if (_sheet == null) return;
            CandidateHide();
            int selIndex = FocusedIndex;
            _operator.Delete(selIndex);
            if (selIndex < _sheet.Items.Count) {
                FocusViewItem(selIndex);
            }
            else {
                FocusViewItem(selIndex - 1);
            }
        }

        public void Copy() {
            if (_sheet == null) return;
            CandidateHide();
            getFocusedExprBox()?.Copy();
        }

        public void CopyAll() {
            if (_sheet == null) return;
            CandidateHide();
            var sb = new StringBuilder();
            foreach (var item in _sheet.Items) {
                sb.Append(item.ExprText).Append(" = ").AppendLine(item.AnsText);
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
            if (_sheet == null) return;
            CandidateHide();
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
            if (_sheet == null) return;
            using (var dlg = new PasteOptionForm()) {
                DialogOpening?.Invoke(this, EventArgs.Empty);
                if (dlg.ShowDialog() == DialogResult.OK) {
                    int insertPos = FocusedIndex;
                    if (insertPos < 0) insertPos = _sheet.Items.Count;
                    var lines = dlg.TextWillBePasted.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                    _operator.Insert(insertPos, lines, true, InsertOptions.Focus);
                }
                DialogClosed?.Invoke(this, EventArgs.Empty);
            }
        }

        public void PickupValuesUsingClipboard() {
            if (_sheet == null) return;
            var exprBox = getFocusedExprBox();
            if (exprBox == null || exprBox.ReadOnly) return;
            using (var dlg = new ValuePickupDialog(exprBox)) {
                var undoEntry = _operator.LastEntry;
                if (dlg.ShowDialog() == DialogResult.OK) {
                    // 追加されたアンドゥ履歴を無かったことにして最新の式を設定し直す
                    var resultText = exprBox.Text;
                    _operator.UndoUntil(undoEntry);
                    exprBox.Text = resultText;
                }
                else {
                    // 変更を巻き戻す
                    _operator.UndoUntil(undoEntry);
                }
                InvalidateLayout();
            }
        }

        public void Undo() {
            if (_sheet == null) return;
            CandidateHide();
            _operator.Undo();
        }

        public void Redo() {
            if (_sheet == null) return;
            CandidateHide();
            _operator.Redo();
        }

        public void Clear() {
            if (_sheet == null) return;
            var ans = MessageBox.Show("Are you sure you want to delete all?", Application.ProductName, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            if (ans == DialogResult.OK) {
                BeforeCleared?.Invoke(this, EventArgs.Empty);
                _operator.Clear();
            }
        }

        public void ReplaceFormatterFunction(FuncDef func) {
            FocusedViewItem?.ReplaceFormatterFunction(func);
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

        public void RelayoutText() {
            if (_sheet == null) return;
            foreach (var noteItem in _sheet.Items) {
                ((SheetViewItem)noteItem.Tag).RelayoutText();
            }
        }

        public void CandidateHide() => FocusedViewItem?.ExprBox.CandidateHide();

        public IEnumerable<InputCandidate> Candidates => _inputCandidates;

        protected override void OnFocusedBoxChanged() {
            base.OnFocusedBoxChanged();
            if (_sheet == null) return;
            // フォーカスされたボックスが所属するアイテムを選択状態にする
            var viewItem = getViewItem(FocusedBox);
            if (viewItem != null) {
                FocusedIndex = IndexOf(viewItem);
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
            if (_sheet == null) return;
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
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Z) {
                e.Handled = true;
                Undo();
            }
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Y) {
                e.Handled = true;
                Redo();
            }
            else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.C) {
                e.Handled = true;
                PickupValuesUsingClipboard();
            }

            if (e.Handled) return;

            base.OnKeyDown(e);

            if (e.Handled) return;

            if (e.Modifiers == Keys.None && e.KeyCode == Keys.Return) {
                e.Handled = true;
                CandidateHide();

                // Return
                var rpn = parseRpnOperation(selIndex);
                if (rpn != null) {
                    // RPNコマンドの実行
                    _operator.ExecuteRpn(rpn);
                    selIndex = rpn.StartIndex;
                }
                else {
                    // 通常の計算結果の確定
                    var ans = viewItem.AnsBox.Text;
                    _operator.Insert(selIndex + 1, ans, false, InsertOptions.FreshAnswer | InsertOptions.Focus);
                }
            }
            else if (e.Modifiers == Keys.Shift && e.KeyCode == Keys.Return) {
                e.Handled = true;
                ItemInsert();
            }
            else if (e.Modifiers == Keys.None && e.KeyCode == Keys.Up) {
                if (selIndex > 0) {
                    e.Handled = true;
                    FocusViewItem(selIndex - 1);
                }
            }
            else if (e.Modifiers == Keys.None && e.KeyCode == Keys.Down) {
                if (selIndex < _sheet.Items.Count - 1) {
                    e.Handled = true;
                    FocusViewItem(selIndex + 1);
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

            if (_focusedIndex >= 0) {
                scrollTo(_focusedIndex);
            }
        }

        protected override void OnKeyUp(KeyEventArgs e) {
            base.OnKeyUp(e);
            if (_sheet == null) return;
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
            if (_sheet == null) return;
            if (e.Button == MouseButtons.Right) {
                openContextMenu(Cursor.Position);
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e) {
            base.OnMouseWheel(e);
            if (_sheet == null) return;
            if (_scrollBar.Visible && e.Delta != 0) {
                setScrollBarValue(_scrollBar.Value - e.Delta);
            }
        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            if (_sheet == null) return;
            if (_focusedIndex >= 0) {
                scrollTo(_focusedIndex);
            }
            InvalidateLayout();
        }

        protected override void OnPaint(PaintEventArgs e) {
            if (_sheet == null) return;

            processRecalcRequest();
            validateLayout(e.Graphics);

            var s = Settings.Instance;

            int n = _sheet.Items.Count;
            for (int i = 0; i < n; i++) {
                var viewItem = GetViewItem(i);
                viewItem.IsRpnOperand =
                    (_focusedRpnOperation != null) &&
                    (_focusedRpnOperation.StartIndex <= i && i < _focusedRpnOperation.EndIndex);
                if (i == _focusedIndex) {
                    viewItem.BackColor = s.Appearance_Color_Active_Background;
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
            RelayoutText();
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
            if (_sheet == null) return null;
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
            switch (e.Action) {
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

            if (_focusedIndex < _sheet.Items.Count) {
                FocusViewItem(_focusedIndex);
            }
            else {
                FocusViewItem(_sheet.Items.Count - 1);
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

        private void _operator_Changed(object sender, EventArgs e) {
            Changed?.Invoke(this, EventArgs.Empty);
        }

        private void SheetItem_ExpressionChanged(object sender, EventArgs e) {
            var senderItem = (SheetItem)sender;
            var focusedIndex = FocusedIndex;
            if (focusedIndex >= 0 && GetViewItem(focusedIndex).SheetItem == senderItem) {
                _focusedRpnOperation = parseRpnOperation(focusedIndex);
            }
            RequestRecalc();
        }

        private void ViewItem_RepaintRequest(object sender, EventArgs e) {
            Invalidate();
        }

        private void openContextMenu(Point screenPos) {
            if (_sheet == null) return;
            var selIndex = FocusedIndex;
            var focusBox = getFocusedExprBox();
            _cmenuUndo.Enabled = _operator.CanUndo;
            _cmenuRedo.Enabled = _operator.CanRedo;
            _cmenuTextCut.Enabled = focusBox != null && !focusBox.ReadOnly && focusBox.SelectionLength > 0;
            _cmenuTextCopy.Enabled = focusBox != null;
            _cmenuTextPaste.Enabled = focusBox != null && !focusBox.ReadOnly;
            _cmenuTextDelete.Enabled = focusBox != null && !focusBox.ReadOnly && focusBox.SelectionLength > 0;
            _cmenuMoveUp.Enabled = selIndex > 0;
            _cmenuMoveDown.Enabled = selIndex >= 0 && selIndex < _sheet.Items.Count - 1;
            _cmenuItemDelete.Enabled = selIndex >= 0;
            _ctxMenu.Show(screenPos);
        }

        public void FocusViewItem(int index) {
            if (_sheet == null) return;

            if (_focusedRpnOperation != null) {
                // RPN操作が確定されないままフォーカスが外されたら文法エラーにするために再計算を要求する
                RequestRecalc();
            }

            _focusedIndex = index;
            if (_focusedIndex >= 0) {
                // 選択されたアイテムに所属するボックスがフォーカスされてなければフォーカスする
                var newViewItem = GetViewItem(_focusedIndex);
                var oldViewItem = getViewItem(FocusedBox);
                if (newViewItem != oldViewItem) {
                    newViewItem.ExprBox.Focus();
                }

                _focusedRpnOperation = parseRpnOperation(_focusedIndex);
                if (_focusedRpnOperation != null) {
                    // 新たに選択されたアイテムが RPN操作として解釈される場合は再計算を要求する
                    RequestRecalc();
                }

                scrollTo(_focusedIndex);
            }
#if DEBUG
            Console.WriteLine("SelectedIndex = " + _focusedIndex);
#endif
        }

        // 指定の ViewItem のインデックスを返す
        public int IndexOf(SheetViewItem viewItem) => (_sheet != null && viewItem != null) ? _sheet.Items.IndexOf(viewItem.SheetItem) : -1;

        // 指定のインデックスの ViewItem を返す
        public SheetViewItem GetViewItem(int index) {
            if (_sheet == null) return null;
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
            if (_sheet == null) return -1;
            if (_sheet.Items.Count == 0) return -1;
            int n = _sheet.Items.Count;
            for (int i = 0; i < n; i++) {
                var viewItem = GetViewItem(i);
                if (y < _innerBox.Top + viewItem.Bottom) {
                    return i;
                }
            }
            return _sheet.Items.Count - 1;
        }

        private ExprBoxCore getFocusedExprBox() {
            if (_sheet == null) return null;
            var viewItem = FocusedViewItem;
            if (viewItem == null) return null;
            if (viewItem.ExprBox.Focused) return viewItem.ExprBox;
            if (viewItem.AnsBox.Focused) return viewItem.AnsBox;
            return null;
        }

        private void scrollTo(int index) {
            if (_sheet == null) return;
            if (index < 0 || index >= _sheet.Items.Count) return;
            validateLayout();
            var client = ClientSize;
            var viewItem = GetViewItem(index);

            if (index >= _sheet.Items.Count -1) {
                setScrollBarValue(_scrollBar.Maximum - _scrollBar.LargeChange);
            }
            else if (_innerBox.Top + viewItem.Top < 0) {
                setScrollBarValue(viewItem.Top);
            }
            else if (_innerBox.Top + viewItem.Bottom > client.Height) {
                setScrollBarValue(viewItem.Bottom - client.Height);
            }
        }

        private void setScrollBarValue(int value) {
            int min = _scrollBar.Minimum;
            int max = _scrollBar.Maximum - _scrollBar.LargeChange;
            _scrollBar.Value = Math.Max(min, Math.Min(max, value));
        }

        private void validateLayout() {
            if (_sheet == null) return;
            if (_layoutValidated) return;
            using (var g = CreateGraphics()) {
                validateLayout(g);
            }
        }

        private void validateLayout(Graphics g) {
            if (_sheet == null) return;
            if (_layoutValidated) return;
#if DEBUG
            Console.WriteLine("Relayout");
#endif
            var equalSize = g.MeasureString("==", Font);
            _equalWidth = (int)equalSize.Width;
            var bottomPadding = (int)equalSize.Height;

            // イコールの位置を揃える
            var itemsHaveAns = _sheet.Items
                .Select(p => (SheetViewItem)p.Tag)
                .Where(p => p.AnsBox.Visible)
                .ToArray();
            var maxExprWidth = 0;
            var maxAnsWidth = 0;
            if (itemsHaveAns.Length > 0) {
                maxExprWidth = itemsHaveAns.Max(p => p.ExprBox.GetPreferredSize().Width);
                maxAnsWidth = itemsHaveAns.Max(p => p.AnsBox.GetPreferredSize().Width);
            }
            int minEqualPos = Math.Min(_equalWidth, _innerBox.Width / 5);
            int newEqualPos;
            if (maxExprWidth + maxAnsWidth + _equalWidth < _innerBox.Width) {
                newEqualPos = Math.Max(minEqualPos, maxExprWidth);
            }
            else {
                newEqualPos = Math.Max(minEqualPos, _innerBox.Width - maxAnsWidth - _equalWidth);
            }
            bool equalPosChanged = newEqualPos != EqualPosition;
            if (equalPosChanged) {
                EqualPosition = newEqualPos;
            }

            // 行の配置
            var client = ClientSize;
            int w = client.Width - _scrollBar.Width;
            int y = 0;
            int n = _sheet.Items.Count;
            _innerBox.SetBounds(0, _innerBox.Top, w, 0);
            for (int i = 0; i < n; i++) {
                var sheetItem = _sheet.Items[i];
                var viewItem = (SheetViewItem)sheetItem.Tag;
                var size = viewItem.GetPreferredSize();
                viewItem.SetBounds(0, y, w, size.Height);
                if (equalPosChanged) viewItem.Relayout();
                viewItem.TabIndex = i;
                y += viewItem.Height;
            }
            _innerBox.Height = y;

            y += bottomPadding;

            // スクロールバーの調整
            if (y <= client.Height) {
                _scrollBar.Value = 0;
                _scrollBar.Maximum = y;
                _scrollBar.LargeChange = y;
                _scrollBar.Visible = false;
                _innerBox.Top = 0;
            }
            else {
                bool scrollToBottom = _scrollBar.Value >= _scrollBar.Maximum - _scrollBar.LargeChange;
                _scrollBar.Visible = true;
                _scrollBar.Maximum = y;
                _scrollBar.LargeChange = client.Height;
                if (scrollToBottom) {
                    setScrollBarValue(_scrollBar.Maximum - _scrollBar.LargeChange);
                }
            }
            _layoutValidated = true;
        }

        private void processRecalcRequest() {
            if (_sheet == null) return;
            if (!_recalcRequested) return;
#if DEBUG
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
#endif
            var ctx = _sheet.Run();
            ctx.Undef(Sheet.LastAnsId, true);

            // 入力補完候補の列挙
            var list = new List<InputCandidate>();
            foreach (var f in BuiltInFuncDef.NativeFunctions) {
                list.Add(new InputCandidate(f.Name.Text, f.ToString(), f.Description, true));
            }
            foreach (var f in ExternalFuncDef.ExternalFunctions) {
                list.Add(new InputCandidate(f.Name.Text, f.ToString(), f.Description, true));
            }
            foreach (var v in ctx.EnumVars()) {
                if (v.Value is FuncVal fval) {
                    var f = (FuncDef)fval.Raw;
                    list.Add(new InputCandidate(v.Name.Text, v.Name.Text + f.GetArgListString(), f.Description, true));
                }
                else {
                    list.Add(new InputCandidate(v.Name.Text, v.Name.Text, v.Description, false));
                }
            }
            list.Add(new InputCandidate(Sheet.LastAnsId, Sheet.LastAnsId, "last answer", false));
            list.Add(new InputCandidate(BoolVal.TrueKeyword, BoolVal.TrueKeyword, "true value", false));
            list.Add(new InputCandidate(BoolVal.FalseKeyword, BoolVal.FalseKeyword, "false value", false));
            list.Add(new InputCandidate("def", "def", "user function definition", false));
            _inputCandidates = list.OrderBy(p => p.Id).ToArray();
            _recalcRequested = false;

            // グラフ描画リクエストの処理
            GraphForm.RequestPlot(_sheet, ctx.PlotCalls.ToArray());
#if DEBUG
            sw.Stop();
            Console.WriteLine("Recalc " + sw.ElapsedMilliseconds + "ms");
#endif
        }

    }
}
