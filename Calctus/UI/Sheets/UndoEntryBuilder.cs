using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Sheets;

namespace Shapoco.Calctus.UI.Sheets {
    class UndoEntryBuilder {
        private SheetView _view;
        private List<SheetAction> _undoActions = new List<SheetAction>();
        private List<SheetAction> _redoActions = new List<SheetAction>();
        public UndoEntryBuilder(SheetView view) {
            _view = view;
        }

        public void Insert(int index, string expr, RadixMode radix, InsertOptions opts = InsertOptions.None) {
            _undoActions.Insert(0, new DeleteAction(index));
            _redoActions.Add(new InsertAction(index, expr, radix, opts));
        }

        public void Delete(int index) {
            var item = _view.GetViewItem(index).SheetItem;
            _undoActions.Insert(0, new InsertAction(index, item.ExprText, item.RadixMode));
            _redoActions.Add(new DeleteAction(index));
        }

        public void ChangeExpression(int index, string expr) {
            _undoActions.Insert(0, new ExpressionChangeAction(index, _view.Sheet.Items[index].ExprText));
            _redoActions.Add(new ExpressionChangeAction(index, expr));
        }

        public void ChangeRadixMode(int index, RadixMode radix) {
            _undoActions.Insert(0, new RadixChangeAction(index, _view.Sheet.Items[index].RadixMode));
            _redoActions.Add(new RadixChangeAction(index, radix));
        }

        public UndoEntry Compile() {
            return new UndoEntry(
                new SheetViewState(_view), 
                group(_undoActions.ToArray()), 
                group(_redoActions.ToArray()));
        }

        private static SheetAction group(SheetAction[] actions) {
            if (actions.Length <= 0) throw new ArgumentException();
            if (actions.Length == 1) return actions[0];
            return new ActionArray(actions);
        }

    }
}
