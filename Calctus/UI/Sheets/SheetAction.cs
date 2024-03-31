using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Sheets;

namespace Shapoco.Calctus.UI.Sheets {
    /// <summary>シートに対するアクション</summary>
    abstract class SheetAction {
        public abstract void Apply(SheetView view);
    }

    /// <summary>シートに対する複数のアクション</summary>
    class ActionArray : SheetAction {
        public readonly SheetAction[] Actions;

        public ActionArray(SheetAction[] actions) {
            Actions = actions;
        }

        public override void Apply(SheetView view) { 
            foreach(var action in Actions) {
                action.Apply(view);
            }
        }
    }

    /// <summary>行の挿入アクション</summary>
    class InsertAction : SheetAction {
        public readonly int Index;
        public readonly string Expression;
        public readonly InsertOptions Options;

        public InsertAction(int index, string expr,  InsertOptions opts = InsertOptions.None) {
            Index = index;
            Expression = expr;
            Options = opts;
        }

        public override void Apply(SheetView view) {
            var item = new SheetItem(Expression);
            view.Sheet.Items.Insert(Index, item);
            var viewItem = ((SheetViewItem)item.Tag);
            viewItem.IsFreshAnswer = Options.HasFlag(InsertOptions.FreshAnswer);
            if (Options.HasFlag(InsertOptions.Focus)) {
                view.FocusViewItem(Index);
            }
        }
    }

    /// <summary>行の削除アクション</summary>
    class DeleteAction : SheetAction {
        public readonly int Index;
        
        public DeleteAction(int index) {
            Index = index;
        }

        public override void Apply(SheetView view) {
            view.Sheet.Items.RemoveAt(Index);
        }
    }

    /// <summary>式の変更アクション</summary>
    class ExpressionChangeAction : SheetAction {
        public readonly int Index;
        public readonly string Expression;

        public ExpressionChangeAction(int index, string expr) {
            Index = index;
            Expression = expr;
        }

        public override void Apply(SheetView view) {
            view.Sheet.Items[Index].ExprText = Expression;
        }
    }
}
