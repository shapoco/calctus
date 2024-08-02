using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.UI.Sheets {
    class SheetViewState {
        public readonly int FocusedLineIndex;
        public readonly int ExprSelStart;
        public readonly int ExprSelLength;

        public SheetViewState(SheetView view) {
            var focusedIndex = view.FocusedIndex;
            var selStart = 0;
            var selLength = 0;
            if (focusedIndex >= 0) {
                var exprBox = view.GetViewItem(focusedIndex).ExprBox;
                selStart = exprBox.SelectionStart;
                selLength = exprBox.SelectionLength;
            }
            FocusedLineIndex = focusedIndex;
            ExprSelStart = selStart;
            ExprSelLength = selLength;
        }

        public void Restore(SheetView view) {
            view.FocusedIndex = FocusedLineIndex;
            if (FocusedLineIndex >= 0) {
                var exprBox = view.GetViewItem(FocusedLineIndex).ExprBox;
                exprBox.SelectionStart = ExprSelStart;
                exprBox.SelectionLength = ExprSelLength;
            }
        }
    }
}
