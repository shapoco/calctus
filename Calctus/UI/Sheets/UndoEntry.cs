using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.UI.Sheets {
    class UndoEntry {
        public readonly SheetViewState ViewState;
        public readonly SheetAction UndoAction;
        public readonly SheetAction RedoAction;

        public UndoEntry(SheetViewState viewState, SheetAction undoAction, SheetAction redoAction) {
            ViewState = viewState;
            UndoAction = undoAction;
            RedoAction = redoAction;
        }
    }
}
