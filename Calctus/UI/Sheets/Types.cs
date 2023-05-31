using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Shapoco.Calctus.UI.Sheets {
    delegate void QueryScreenCursorLocationEventHandler(object sender, QueryScreenCursorLocationEventArgs e);

    class QueryScreenCursorLocationEventArgs : EventArgs {
        public readonly int CursorPosition;
        public Point Result;

        public QueryScreenCursorLocationEventArgs(int cursorPosition) {
            CursorPosition = cursorPosition;
        }
    }

    class RpnOperation {
        public readonly int StartIndex;
        public readonly int EndIndex;
        public readonly string Expression;
        public readonly Exception Error;
        public RpnOperation(int start, int end, string expr, Exception err) {
            StartIndex = start;
            EndIndex = end;
            Expression = expr;
            Error = err;
        }
    }

}
