using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Sheets {
    delegate void PreviewExecuteEventHandler(Sheet sender, PreviewExecuteEventArgs e);

    class PreviewExecuteEventArgs {
        public readonly int Index;
        public readonly EvalContext Context;
        public readonly SheetItem Item;
        public bool Overrided = false;
        public Val Answer = null;
        public Exception SyntaxError = null;
        public Exception EvalError = null;

        public PreviewExecuteEventArgs(int address, EvalContext ctx, SheetItem item) {
            Index = address;
            Context = ctx;
            Item = item;
        }
    }

    public enum RadixMode {
        Auto, Dec, Hex, Bin, Oct,
        SiPrefix, BinaryPrefix, Char,
    }
}
