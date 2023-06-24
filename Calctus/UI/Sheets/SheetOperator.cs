using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Sheets;

namespace Shapoco.Calctus.UI.Sheets {
    class SheetOperator : IDisposable {
        public const int UndoBufferDepth = 100;

        private SheetView _view;
        private List<UndoEntry> _undoBuffer = new List<UndoEntry>();
        private int _undoBufferIndex = 0;
        private bool _disposed = false;

        public SheetOperator(SheetView view) {
            _view = view;
        }

        ~SheetOperator() => Dispose(false);
        public void Dispose() => Dispose(true);

        private void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
                    _undoBuffer.Clear();
                }
                _disposed = true;
            }
        }

        /// <summary>アンドゥ</summary>
        public void Undo() {
            if (_undoBufferIndex <= 0) return;
            _undoBufferIndex--;
            var entry = _undoBuffer[_undoBufferIndex];
            entry.UndoAction.Apply(_view);
            entry.ViewState.Restore(_view);
        }

        /// <summary>リドゥ</summary>
        public void Redo() {
            if (_undoBufferIndex >= _undoBuffer.Count) return;
            _undoBuffer[_undoBufferIndex].RedoAction.Apply(_view);
            _undoBufferIndex++;
            if (_undoBufferIndex < _undoBuffer.Count) {
                _undoBuffer[_undoBufferIndex].ViewState.Restore(_view);
            }
        }

        /// <summary>新しい行の挿入</summary>
        public void Insert(int index, string expr, RadixMode radix, bool overwriteEmptyLine = false, InsertOptions opts = InsertOptions.None) {
            Insert(index, new string[] { expr }, radix, overwriteEmptyLine, opts);
        }

        /// <summary>新しい行の挿入</summary>
        public void Insert(int index, string[] expr, RadixMode radix, bool overwriteEmptyLine = false, InsertOptions opts = InsertOptions.None) {
            var b = new UndoEntryBuilder(_view);
            var sheet = _view.Sheet;
            for (int i = 0; i < expr.Length; i++) {
                int insertPos = index + i;
                Console.WriteLine(i + ", " + insertPos);
                if (i == 0 && insertPos < sheet.Items.Count && string.IsNullOrEmpty(sheet.Items[insertPos].ExprText) && overwriteEmptyLine) {
                    // overwriteEmptyLine が true かつ挿入箇所が空行の場合は上書き
                    b.ChangeExpression(insertPos, expr[i]);
                    b.ChangeRadixMode(insertPos, radix);
                }
                else {
                    b.Insert(insertPos, expr[i], radix,
                        (i == expr.Length - 1) ? opts : InsertOptions.None);
                }
            }
            commitAction(b.Compile());
        }

        /// <summary>行の削除</summary>
        public void Delete(int index, int count = 1) {
            var b = new UndoEntryBuilder(_view);
            bool allDeleted = (count == _view.Sheet.Items.Count);
            for (int i = count - 1; i >= 0; i--) {
                b.Delete(index + i);
            }
            if (allDeleted) {
                // リストが空になる場合は空行を1行追加する
                b.Insert(0, "", _view.ActiveRadixMode, InsertOptions.Focus);
            }
            commitAction(b.Compile());
        }

        /// <summary>シートのクリア</summary>
        public void Clear() {
            Delete(0, _view.Sheet.Items.Count);
        }

        /// <summary>式の移動</summary>
        public void Move(int indexFrom, int indexTo) {
            var b = new UndoEntryBuilder(_view);
            var item = _view.GetViewItem(indexFrom).SheetItem;
            b.Delete(indexFrom);
            b.Insert(indexTo, item.ExprText, item.RadixMode);
            commitAction(b.Compile());
        }

        /// <summary>式の変更</summary>
        public void ChangeExpression(int index, string expr) {
            var b = new UndoEntryBuilder(_view);
            b.ChangeExpression(index, expr);
            commitAction(b.Compile());
        }

        /// <summary>基数モードの変更</summary>
        public void ChangeRadix(int index, RadixMode radix) {
            var b = new UndoEntryBuilder(_view);
            b.ChangeRadixMode(index, radix);
            commitAction(b.Compile());
        }

        /// <summary>RPNコマンドの実行</summary>
        public void ExecuteRpn(RpnOperation rpn) {
            var viewItem = _view.FocusedViewItem;
            var ans = viewItem.AnsBox.Text;
            var radix = viewItem.SheetItem.RadixMode;

            if (rpn.Error != null) return;
            var b = new UndoEntryBuilder(_view);
            for (int i = rpn.EndIndex - 1; i >= rpn.StartIndex; i--) {
                b.Delete(i);
            }
            b.ChangeExpression(rpn.StartIndex, rpn.Expression);
            b.Insert(rpn.StartIndex + 1, ans, radix, InsertOptions.FreshAnswer | InsertOptions.Focus);
            commitAction(b.Compile());
        }

        private void commitAction(UndoEntry entry) {
            while(_undoBuffer.Count > _undoBufferIndex) {
                _undoBuffer.RemoveAt(_undoBuffer.Count - 1);
            }
            _undoBuffer.Add(entry);
            _undoBufferIndex++;
            if (_undoBuffer.Count > UndoBufferDepth) {
                _undoBuffer.RemoveAt(0);
                _undoBufferIndex--;
            }
            entry.RedoAction.Apply(_view);
        }
    }
}

